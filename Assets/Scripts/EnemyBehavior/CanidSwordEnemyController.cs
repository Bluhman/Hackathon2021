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
	float xMovementSmoothing;
	bool alerted;
	float actionTimer = 0;

	// Start is called before the first frame update
	public override void Start()
	{
		base.Start();

		animator = GetComponent<Animator>();
		cs = GetComponent<CharacterStat>();
		ab = GetComponent<AwarenessBehavior>();


		swordMirrorDistance = attackBox.transform.localPosition.x;
	}

	// Update is called once per frame
	protected override void Update()
	{
		base.Update();

		alerted = ab.alerted;
		DetermineState();

		if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("uncn"))
		{
			DoAI();
		}
		else
		{
			actionTimer = 0;
		}
	}

	private void DoAI()
	{
		//Status checks for quick decisions recalculating:
		if (getIsAirborne()
			|| (getLeftLedge() && currentAction == "WalkLeft" && !getIsAirborne())
			|| (getRightLedge() && currentAction == "WalkRight" && !getIsAirborne())
			|| (currentAction == "WalkLeft" && controller.collisions.left)
			|| (currentAction == "WalkRight" && controller.collisions.right)
			)
		{
			//Debug.Log("RECALC!" + getIsAirborne() + (getLeftLedge() && currentAction == "WalkLeft") + (getRightLedge() && currentAction == "WalkRight"));
			actionTimer = 0;
		}

		actionTimer -= Time.deltaTime;

		if (actionTimer <= 0)
		{
			List<string> actionTags = new List<string> {
				"DoIdle"
			};
			if (!getLeftLedge() && !controller.collisions.left)
			{
				actionTags.Add("WalkLeft");
				actionTags.Add("WalkLeft");
			}
			if (!getRightLedge() && !controller.collisions.right)
			{
				actionTags.Add("WalkRight");
				actionTags.Add("WalkRight");
			}
			string choice = actionTags[UnityEngine.Random.Range(0, actionTags.Count)];
			Invoke(choice, 0);
			currentAction = choice;
			//Debug.Log("Decision: " + currentAction);
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
		}

		var modifiedDistance = attackBox.transform.localPosition;
		modifiedDistance.x = swordMirrorDistance * (bodySprite.flipX ? -1 : 1);
		attackBox.transform.localPosition = modifiedDistance;
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
		base.CalculateVelocity();
		float targetVelocityX;
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stagger"))
		{
			targetVelocityX = velocity.x / (getIsAirborne() ? 1 : 2);
		}
		else if (!CannotMove)
		{
			targetVelocityX = directionalInput.x * moveSpeed;
		}
		else
		{
			targetVelocityX = 0;
		}
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref xMovementSmoothing, 0.1f);

	}
}
