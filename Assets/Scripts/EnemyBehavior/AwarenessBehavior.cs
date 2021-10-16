using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwarenessBehavior : MonoBehaviour
{
	public float maxSightDistance;

	[HideInInspector]
	public bool alerted;
	bool facingRight;

	public AudioSource alertNoise;
	public SpriteRenderer spriteFacingSource;
	GameObject thePlayer;
	Animator playerAnimationState;

	// Start is called before the first frame update
	void Start()
	{
		thePlayer = GameObject.FindWithTag("Player");
		playerAnimationState = thePlayer.GetComponent<Animator>();
	}

	private void OnEnable()
	{
		alerted = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (!alerted && !playerAnimationState.GetBool("isDead"))
		{
			SearchForPlayer();
		}

		if (playerAnimationState.GetBool("isDead"))
		{
			alerted = false;
		}
	}

	public float playerDistance
	{
		get { 
			if (playerAnimationState.GetBool("isDead"))
			{
				return 9999;
			}
			return (transform.position - thePlayer.transform.position).magnitude; 
		}
	}

	public bool playerToRight
	{
		get => transform.position.x < thePlayer.transform.position.x;
	}

	private void SearchForPlayer()
	{
		facingRight = spriteFacingSource.flipX;
		bool ahead = facingRight == playerToRight;
		if (ahead)
		{
			int layerMask = (1 << 9) + (1 << 8);
			var thingHit = Physics2D.Linecast(transform.position, thePlayer.transform.position, layerMask);
			alerted = thingHit.transform == thePlayer.transform 
				&& thingHit.distance <= maxSightDistance;
			if (alerted && alertNoise != null)
			{
				alertNoise.Play();
			}
		}
	}

	public void FacePlayer()
	{
		spriteFacingSource.flipX = playerToRight;
	}
}
