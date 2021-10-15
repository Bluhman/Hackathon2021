using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwarenessBehavior : MonoBehaviour
{
	public float maxSightDistance;

	[HideInInspector]
	public bool alerted;
	bool facingRight;

	public SpriteRenderer spriteFacingSource;
	GameObject thePlayer;

	// Start is called before the first frame update
	void Start()
	{
		thePlayer = GameObject.FindWithTag("Player");
	}

	private void OnEnable()
	{
		alerted = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (!alerted)
		{
			SearchForPlayer();
		}
	}

	public float playerDistance
	{
		get => (transform.position - thePlayer.transform.position).magnitude;
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
			if (alerted) Debug.Log("I SEE YOU.");
		}
	}

	public void FacePlayer()
	{
		spriteFacingSource.flipX = playerToRight;
	}
}
