using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class CanidSwordEnemyBehavior : CharacterStat
{
	Animator animator;
	WalkingController wc;
	public GameObject attackBox;
	public SpriteRenderer bodySprite;

	float swordMirrorDistance;
	bool alerted;
	GameObject thePlayer;

	// Start is called before the first frame update
	void Start()
	{
		base.Start();

		animator = GetComponent<Animator>();
		wc = GetComponent<WalkingController>();
		thePlayer = GameObject.FindWithTag("Player");

		swordMirrorDistance = attackBox.transform.localPosition.x;
	}

	// Update is called once per frame
	void Update()
	{
		base.Update();

		DetermineState();
	}

	private void OnEnable()
	{
		//When you get enabled, don't have the attack hitbox enabled!
		attackBox.SetActive(false);
		alerted = false;
	}

	private void DetermineState()
	{
		animator.SetBool("isDead", charMetrics.isDead);
		animator.SetFloat("walkingSpeed", Math.Abs(wc.velocity.x));
		animator.SetBool("inAir", wc.getIsAirborne());

		if (wc.directionalInput.x != 0 && !CannotFlip)
		{
			Debug.Log(wc.directionalInput.x);
			bodySprite.flipX = wc.directionalInput.x > 0;
			var modifiedDistance = attackBox.transform.localPosition;
			modifiedDistance.x = swordMirrorDistance * -Mathf.Sign(wc.directionalInput.x);
			attackBox.transform.localPosition = modifiedDistance;
		}

		//Searching for the player
		if (!alerted)
		{
			SearchForThePlayer();
		}
	}

	public bool CannotFlip { get =>
			animator.GetCurrentAnimatorStateInfo(0).IsTag("atk")
			|| animator.GetCurrentAnimatorStateInfo(0).IsTag("uncn")
			|| animator.GetCurrentAnimatorStateInfo(0).IsTag("block");
			}

	public bool CannotMove { get =>
			animator.GetCurrentAnimatorStateInfo(0).IsTag("atk")
			|| animator.GetCurrentAnimatorStateInfo(0).IsTag("uncn");
			}

	private void SearchForThePlayer()
	{
		//Which way are we facing? We can only look ahead.
		bool facingRight = bodySprite.flipX;
		bool ahead = facingRight == transform.position.x < thePlayer.transform.position.x;

		if (ahead)
		{
			//Debug.Log("I might see you....");
			//We want to find collisions either with the player, or against obstacles.
			int layerMask = (1 << 9) + (1 << 8);
		}
		

	}

	protected override void OnStagger(int amount, Vector2 direction)
	{
		animator.SetTrigger("stagger");
		attackBox.SetActive(false);

		if (direction.x != 0)
		{
			bodySprite.flipX = direction.x < 0;
			var modifiedDistance = attackBox.transform.localPosition;
			modifiedDistance.x = swordMirrorDistance * Mathf.Sign(direction.x);
			attackBox.transform.localPosition = modifiedDistance;
		}

		base.OnStagger(amount, direction);

	}
}
