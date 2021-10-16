using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class CanidSwordEnemyController : WalkingController
{
	Animator animator;
	CharacterStat baseCharacterStats;
	AwarenessBehavior ab;
	public GameObject attackBox;
	public SpriteRenderer bodySprite;
	public string currentAction;
	public float attackRange;

	float swordMirrorDistance;
	float xMovementSmoothing;
	bool alerted;
	bool _block;
	int originalStaminaRegen;

	float actionTimer = 0;

	bool blocking
	{
		get => _block;
		set {
			_block = value;
			if (_block)
			{
				//Regen Stamina very slowly.
				baseCharacterStats.staminaRegenPerSec = 1;
			}
			else
			{
				//Do regen stamina
				baseCharacterStats.staminaRegenPerSec = originalStaminaRegen;
			}
		}
	}

	// Start is called before the first frame update
	public override void Start()
	{
		base.Start();

		animator = GetComponent<Animator>();
		baseCharacterStats = GetComponent<CharacterStat>();
		ab = GetComponent<AwarenessBehavior>();

		swordMirrorDistance = attackBox.transform.localPosition.x;
		originalStaminaRegen = baseCharacterStats.staminaRegenPerSec;
	}

	// Update is called once per frame
	protected override void Update()
	{
		base.Update();

		if (!alerted)
		{
			blocking = false;
		}

		//If just recently alerted to the existence of the player, take action immediately!
		if (!alerted && ab.alerted)
		{
			actionTimer = 0;
		}

		alerted = ab.alerted;
		DetermineState();
		var currentAnimInfo = animator.GetCurrentAnimatorStateInfo(0);

		if (!currentAnimInfo.IsTag("uncn") && !currentAnimInfo.IsTag("atk"))
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
		if (alerted)
		{
			//choose actions twice as fast while alerted.
			actionTimer -= Time.deltaTime;
		}

		if (alerted && ab.playerDistance < attackRange)
		{
			if (UnityEngine.Random.value < 0.8f)
			{
				Invoke("Attack", 0);
				currentAction = "Attack";
				return;
			}
		}

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
			if (alerted)
			{
				//Battle options:
				actionTags.Add("Block");
				if (!blocking && baseCharacterStats.charMetrics.stamina > 5)
				{
					actionTags.Add("Block");
					actionTags.Add("Block");
					actionTags.Add("Block");
					actionTags.Add("Block");
				}
				else if (ab.playerToRight && bodySprite.flipX)
				{
					actionTags.Add("WalkRight");
				}
				else if (!ab.playerToRight && !bodySprite.flipX)
				{
					actionTags.Add("WalkLeft");
				}
			}
			string choice = actionTags[UnityEngine.Random.Range(0, actionTags.Count)];

			//Outside of normal probability cycles:
			if (alerted)
			{
				if (ab.playerToRight != bodySprite.flipX)
				{
					//We're not facing the player!!
					if (UnityEngine.Random.value < 0.95f)
					{
						choice = "Block";
					}
				}
			}

			Invoke(choice, 0);
			currentAction = choice;
			//Debug.Log("Decision: " + currentAction);
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

	private void Block()
	{
		//Face towards player and raise shield.
		directionalInput.x = 0;
		ab.FacePlayer();
		blocking = true;
		actionTimer = UnityEngine.Random.Range(1.0f, 1.4f);
	}

	private void Attack()
	{
		blocking = false;
		ab.FacePlayer();
		animator.SetTrigger("attack");
	}

	private void OnEnable()
	{
		//When you get enabled, don't have the attack hitbox enabled!
		attackBox.SetActive(false);
	}

	private void DetermineState()
	{
		animator.SetBool("isDead", baseCharacterStats.charMetrics.isDead);
		animator.SetBool("walking", directionalInput.x != 0);
		animator.SetFloat("walkingSpeed", Math.Abs(velocity.x));
		animator.SetBool("inAir", getIsAirborne());
		animator.SetBool("facingRight", bodySprite.flipX);

		if (CannotMove)
		{
			_block = false;
		}

		animator.SetBool("blocking", _block);

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
