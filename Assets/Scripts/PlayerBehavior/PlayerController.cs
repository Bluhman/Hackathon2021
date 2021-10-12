using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(PlayerStatTracker))]
public class PlayerController : WalkingController
{
	//public enum PlayerAnimationState
	//{
	//	Standing,
	//	Crouching,
	//	Walking,
	//	Jumping,
	//	Attacking,
	//	AttackingCrouch,
	//	Stagger
	//}
	public float accelerationTimeAirborne = 1;

	public float accelerationTimeGround = 0.1f;
	public float crouchHeightModifier = 0.5f;

	//public Vector2 weaponOffsetStanding;
	public Vector2 weaponOffsetCrouching;
	float xMovementSmoothing;
	public int maxJumps = 1;

	int jumpsLeft;
	[HideInInspector]
	public Animator animator;
	PlayerStatTracker stats;
	CharacterStat basicStats;
	Inventory equipmentState;

	public SpriteRenderer bodySprite;
	public SpriteRenderer clothingSprite;

	// Start is called before the first frame update
	public override void Start()
	{
		base.Start();

		stats = GetComponent<PlayerStatTracker>();
		basicStats = GetComponent<CharacterStat>();
		equipmentState = GetComponent<Inventory>();
		animator = GetComponent<Animator>();

		//Initialize jumps left to 0, in case player is spawning in midair.
		jumpsLeft = 0;
	}

	protected override void Update()
	{

		DeterminePlayerState();
		CalculateVelocity();
		//fall through if the player is holding down the down axis while airborne.
		if (directionalInput.y == -1 && !controller.collisions.below)
		{
			InitiateFallThrough();
		}
		controller.Move(velocity * Time.deltaTime, directionalInput);

		if (controller.collisions.above || controller.collisions.below)
		{
			if (controller.collisions.slidingDownMaxSlope)
			{
				velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
			}
			else
			{
				velocity.y = 0;
			}
		}
	}

	public void DeterminePlayerState()
	{
		animator.SetBool("isDead", basicStats.charMetrics.isDead);
		animator.SetBool("isWalking", directionalInput.x != 0);
		animator.SetFloat("walkingSpeed", Math.Abs(velocity.x));
		animator.SetBool("inAir", getIsAirborne());
		if (getIsAirborne())
		{
			animator.SetFloat("fallSpeed", -velocity.y);
		}
		animator.SetBool("ducking", directionalInput.y < 0);

		if (!CannotFlip
			&& directionalInput.x != 0)
		{
			//Mirrored = moving to the right.
			bodySprite.flipX = clothingSprite.flipX = directionalInput.x > 0;
		}
	}

	public bool UnresponsiveToInput { get => animator.GetCurrentAnimatorStateInfo(0).IsTag("uncn"); }
	public bool CannotFlip { get => 
			UnresponsiveToInput
			|| animator.GetCurrentAnimatorStateInfo(0).IsTag("atk")
			|| animator.GetCurrentAnimatorStateInfo(0).IsTag("air"); 
	}
	public bool CannotMove { get => animator.GetCurrentAnimatorStateInfo(0).IsTag("atk")
			|| animator.GetBool("ducking")
			|| UnresponsiveToInput;
	}

	public bool CannotAttack { get => animator.GetCurrentAnimatorStateInfo(0).IsTag("atk")
			|| UnresponsiveToInput;
	}

	public void SetDirectionalInput(Vector2 input)
	{
		directionalInput = input;
	}

	public bool facingRight { get => bodySprite.flipX; }

	internal void OnAttack()
	{
		//Check stamina levels.
		if (stats.charMetrics.currentStamina < 0
			|| CannotAttack
			)
		{
			return;
		}
		var wpnToUse = equipmentState.equippedWeapon;

		stats.DrainStamina(wpnToUse.staminaCost, 1);

		animator.SetFloat("attackSpeed", wpnToUse.attackSpeed);
		animator.SetTrigger("attack");

		var attack = Instantiate(equipmentState.weaponEffect, gameObject.transform);

		if (facingRight)
		{
			attack.transform.Rotate(0, 180, 0);
		}
		if (animator.GetBool("ducking"))
		{
			Vector3 move = new Vector3(wpnToUse.spawnPositionCrouching.x, wpnToUse.spawnPositionCrouching.y);
			attack.transform.Translate(move);
		}
		//Stupid hardcoded spear.
		attack.transform.Rotate(0, 0, -46.459f);
	}

	public void OnJumpInputDown()
	{
		var isAirborne = !controller.collisions.below;

		if (!isAirborne)
		{
			jumpsLeft = maxJumps;

			//Fall-through platform button combination: jump while pressing down.
			if (animator.GetBool("ducking"))
			{
				InitiateFallThrough();
				jumpsLeft--;
				//Doing this also doesn't trigger a jump.
				return;
			}

			if (controller.collisions.slidingDownMaxSlope)
			{
				//If you're not trying to jump up the very steep slope:
				if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
				{
					velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
					velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
				}
			}
		}

		//Aerial jumps.
		if (jumpsLeft > 0 && !CannotMove)
		{
			//Reset jump anim.
			animator.Play("Jump", 0, 0f);
			velocity.y = maxJumpVelocity;
			xMovementSmoothing = 0;
			velocity.x = directionalInput.x * moveSpeed;
			jumpsLeft--;
		}
	}

	private void InitiateFallThrough()
	{
		controller.collisions.fallingThroughPlatform = true;
		Invoke("ResetFallingThroughPlatform", 0.5f);
	}

	public void OnJumpInputUp()
	{
		if (velocity.y > minJumpVelocity)
		{
			velocity.y = minJumpVelocity;
		}
	}



	void ResetFallingThroughPlatform()
	{
		controller.collisions.fallingThroughPlatform = false;
	}

	public override void CalculateVelocity()
	{
		float targetVelocityX;
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
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

		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref xMovementSmoothing, (controller.collisions.below) ? accelerationTimeGround : accelerationTimeAirborne);
		base.CalculateVelocity();
	}
}
