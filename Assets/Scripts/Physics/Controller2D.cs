﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Controller2D : RaycastController
{
	public float maxSlopeAngle = 55;

	public CollisionInfo collisions;
	[HideInInspector]
	public Vector2 playerInput;

	public override void Start()
	{
		base.Start();
		collisions.faceDir = 1;

	}

	public void Move(Vector2 moveAmount, bool standingOnPlatform)
	{
		Move(moveAmount, Vector2.zero, standingOnPlatform);
	}

	public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
	{
		UpdateRaycastOrigins();

		collisions.Reset();
		collisions.moveAmountOld = moveAmount;

		playerInput = input;

		if (moveAmount.y < 0)
		{
			DescendSlope(ref moveAmount);
		}
		if (moveAmount.x != 0)
		{
			collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
		}
		HorizontalCollisions(ref moveAmount);

		if (moveAmount.y != 0)
		{

			VerticalCollisions(ref moveAmount);
		}

		transform.Translate(moveAmount);

		if (standingOnPlatform)
		{
			collisions.below = true;
		}
	}



	void VerticalCollisions(ref Vector2 moveAmount)
	{
		float directionY = Mathf.Sign(moveAmount.y);
		float directionX = Mathf.Sign(moveAmount.x);
		float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector2 rayOrigin = directionY == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

			if (hit)
			{
				if (hit.collider.tag == "ThruPlatform")
				{
					//Pass through a thru platform from below.
					if (directionY == 1 || hit.distance == 0)
					{
						continue;
					}
					//If currently attempting to fall through a platform, ignore the collision.
					if (collisions.fallingThroughPlatform)
					{
						continue;
					}
				}

				moveAmount.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				if (collisions.climbingSlope)
				{
					moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * directionX;
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}

		if (collisions.climbingSlope)
		{
			rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			if (hit)
			{
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if (slopeAngle != collisions.slopeAngle)
				{
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
					collisions.slopeNormal = hit.normal;
				}
			}
		}
	}

	void HorizontalCollisions(ref Vector2 moveAmount)
	{
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 rayOrigin = directionX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

			if (hit)
			{
				if (hit.distance == 0)
				{
					continue;
				}

				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				//Slope logic: only runs this on the bottom collision of the target.
				if (i == 0 && slopeAngle <= maxSlopeAngle)
				{
					if (collisions.descendingSlope)
					{
						collisions.descendingSlope = false;
						moveAmount = collisions.moveAmountOld;
					}
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld)
					{
						distanceToSlopeStart = hit.distance - skinWidth;
						moveAmount.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
					moveAmount.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
				{
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope)
					{
						moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}
			}
		}
	}

	private void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
	{
		float moveDistance = Mathf.Abs(moveAmount.x);
		float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (moveAmount.y <= climbVelocityY)
		{
			moveAmount.y = climbVelocityY;
			moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
			collisions.slopeNormal = slopeNormal;
		}
		
	}

	private void DescendSlope(ref Vector2 moveAmount)
	{
		RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
		RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);

		if (maxSlopeHitLeft ^ maxSlopeHitRight)
		{
			SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
			SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
		}

		if (!collisions.slidingDownMaxSlope)
		{
			float directionX = Mathf.Sign(moveAmount.x);
			Vector2 rayOrigin = directionX == -1 ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

			if (hit)
			{
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
				{
					if (Mathf.Sign(hit.normal.x) == directionX)
					{
						if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
						{
							float moveDistance = Mathf.Abs(moveAmount.x);
							float descentVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
							moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
							moveAmount.y -= descentVelocityY;

							collisions.slopeAngle = slopeAngle;
							collisions.descendingSlope = true;
							collisions.below = true;
						}
					}
				}
			}
		}
	}

	private void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
	{
		if (hit)
		{
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle > maxSlopeAngle)
			{
				moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

				collisions.slopeAngle = slopeAngle;
				collisions.slidingDownMaxSlope = true;
				collisions.slopeNormal = hit.normal;
			}
		}
	}

	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public bool slidingDownMaxSlope;

		public float slopeAngle, slopeAngleOld;
		public Vector2 slopeNormal;
		public Vector2 moveAmountOld;
		public int faceDir;
		public bool fallingThroughPlatform;

		public void Reset()
		{
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;
			slidingDownMaxSlope = false;
			slopeNormal = Vector2.zero;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}


}
