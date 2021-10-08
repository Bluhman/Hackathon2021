using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class WalkingController : MonoBehaviour
{
   public float maxJumpHeight = 3;
   public float minJumpHeight = 0.5f;
   public float timeToJumpApex = 0.35f;
   public float moveSpeed = 6;
   public float terminalVelocity = 0f;

   protected float gravity;
   protected float minJumpVelocity;
   protected float maxJumpVelocity;

   [HideInInspector]
   public Vector3 velocity;
   protected Controller2D controller;
   protected BoxCollider2D boxCollider;
   [HideInInspector]
   public Vector2 directionalInput;

   // Start is called before the first frame update
   public virtual void Start()
    {
      controller = GetComponent<Controller2D>();
      boxCollider = GetComponent<BoxCollider2D>();

      gravity = -(2 * maxJumpHeight / Mathf.Pow(timeToJumpApex, 2));
      maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

      if (terminalVelocity > 0 && maxJumpVelocity > terminalVelocity)
		{
         terminalVelocity = maxJumpVelocity;
		}

      minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
   }

   public bool getIsAirborne()
   {
      return (!controller.collisions.below);
   }

   public virtual void CalculateVelocity()
   {
      velocity.y += gravity * Time.deltaTime;
      if (terminalVelocity > 0 && velocity.magnitude > terminalVelocity)
		{
         velocity.Normalize();
         velocity *= terminalVelocity;
		}
   }

	// Update is called once per frame
	protected virtual void Update()
    {
      CalculateVelocity();

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
}
