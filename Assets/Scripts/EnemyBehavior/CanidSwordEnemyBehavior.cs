using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class CanidSwordEnemyBehavior : CharacterStat
{
	Animator animator;
	WalkingController wc;
	AwarenessBehavior ab;
	public GameObject attackBox;
	public SpriteRenderer bodySprite;

	float swordMirrorDistance;
	bool alerted;

	// Start is called before the first frame update
	void Start()
	{
		base.Start();

		animator = GetComponent<Animator>();
		wc = GetComponent<WalkingController>();
		ab = GetComponent<AwarenessBehavior>();
		

		swordMirrorDistance = attackBox.transform.localPosition.x;
	}

	// Update is called once per frame
	void Update()
	{
		base.Update();

		alerted = ab.alerted;
		DetermineState();
	}

	private void OnEnable()
	{
		//When you get enabled, don't have the attack hitbox enabled!
		attackBox.SetActive(false);
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
