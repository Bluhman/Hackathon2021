using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class CanidSwordEnemyController : WalkingController
{
	Animator animator;
	CharacterStat cs;
	AwarenessBehavior ab;
	public GameObject attackBox;
	public SpriteRenderer bodySprite;
	public string currentAction;

	float swordMirrorDistance;
	bool alerted;
	float actionTimer = 0;

	// Start is called before the first frame update
	void Start()
	{
		base.Start();

		animator = GetComponent<Animator>();
		cs = GetComponent<CharacterStat>();
		ab = GetComponent<AwarenessBehavior>();


		swordMirrorDistance = attackBox.transform.localPosition.x;
	}

	// Update is called once per frame
	void Update()
	{
		base.Update();

		alerted = ab.alerted;
		DetermineState();

		if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("uncn"))
		{
			DoAI();
		}

	}

	private void DoAI()
	{
		//Status checks for quick decisions recalculating:
		if (getIsAirborne()
			|| (getLeftLedge() && currentAction == "WalkLeft" && !getIsAirborne())
			|| (getRightLedge() && currentAction == "WalkRight" && !getIsAirborne())
			)
		{
			Debug.Log("RECALC!" + getIsAirborne() + (getLeftLedge() && currentAction == "WalkLeft") + (getRightLedge() && currentAction == "WalkRight"));
			actionTimer = 0;
		}

		actionTimer -= Time.deltaTime;

		if (actionTimer <= 0)
		{
			List<string> actionTags = new List<string> { 
				"DoIdle"
			};
			if (!getLeftLedge())
			{
				actionTags.Add("WalkLeft");
				actionTags.Add("WalkLeft");
			}
			if (!getRightLedge())
			{
				actionTags.Add("WalkRight");
				actionTags.Add("WalkRight");
			}
			string choice = actionTags[UnityEngine.Random.Range(0, actionTags.Count)];
			Invoke(choice, 0);
			currentAction = choice;
			Debug.Log("Decision: " + currentAction);
		}


		if (alerted)
		{
			//Angry
		}
		else
		{

		}
	}

	private void DoIdle()
	{
		directionalInput.x = 0;
		actionTimer = UnityEngine.Random.Range(1.0f, 7.5f);
	}

	private void WalkLeft()
	{
		directionalInput.x = -1;
		actionTimer = UnityEngine.Random.Range(1.0f, 7.5f);
	}

	private void WalkRight()
	{
		directionalInput.x = 1;
		actionTimer = UnityEngine.Random.Range(1.0f, 7.5f);
	}

	private void OnEnable()
	{
		//When you get enabled, don't have the attack hitbox enabled!
		attackBox.SetActive(false);
	}

	private void DetermineState()
	{
		animator.SetBool("isDead", cs.charMetrics.isDead);
		animator.SetBool("walking", directionalInput.x != 0);
		animator.SetFloat("walkingSpeed", Math.Abs(velocity.x));
		animator.SetBool("inAir", getIsAirborne());

		if (directionalInput.x != 0 && !CannotFlip)
		{
			//Debug.Log(wc.directionalInput.x);
			bodySprite.flipX = directionalInput.x > 0;
			var modifiedDistance = attackBox.transform.localPosition;
			modifiedDistance.x = swordMirrorDistance * -Mathf.Sign(directionalInput.x);
			attackBox.transform.localPosition = modifiedDistance;
		}
	}

	public bool CannotFlip
	{
		get =>
animator.GetCurrentAnimatorStateInfo(0).IsTag("atk")
|| animator.GetCurrentAnimatorStateInfo(0).IsTag("uncn")
|| animator.GetCurrentAnimatorStateInfo(0).IsTag("block");
	}

	public bool CannotMove
	{
		get =>
animator.GetCurrentAnimatorStateInfo(0).IsTag("atk")
|| animator.GetCurrentAnimatorStateInfo(0).IsTag("uncn");
	}

	public override void CalculateVelocity()
	{
		float targetVelocityX;
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stagger"))
		{
			//Moderate friction... None in air.
			targetVelocityX = velocity.x / (getIsAirborne() ? 1 : 2);
		}
		//Can't move voluntarily in these states:
		else if (CannotMove)
		{
			targetVelocityX = 0;
		}
		else
		{
			targetVelocityX = directionalInput.x * moveSpeed;
		}

		velocity.x = targetVelocityX;

		base.CalculateVelocity();
	}
}
