using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class CanidEnemyStat : CharacterStat
{
   Animator animator;
   WalkingController wc;
   public SpriteRenderer bodySprite;

   void Start()
   {
      base.Start();

      animator = GetComponent<Animator>();
      wc = GetComponent<WalkingController>();
      Debug.Log(wc);
   }
	protected override void OnStagger(int amount, Vector2 direction)
	{
      if (amount <= 0)
      {
         //Probably shouldn't be zero... but just in case.
         return;
      }

      float staggerTime = (amount / 1.5f) / charMetrics.balance;
      animator.SetFloat("staggerAnimSpeed", 1 / staggerTime);
      animator.SetTrigger("stagger");
      //wc.directionalInput.x = Mathf.Sign(-direction.x);
      wc.directionalInput.x = 0;
      wc.velocity = Vector3.zero;
      Debug.Log(direction);
      wc.velocity = direction * Mathf.Abs(charMetrics.currentFooting);
      Debug.Log(wc.velocity);

      if (direction.x != 0)
		{
         bodySprite.flipX = -direction.x > 0;
      }

      base.OnStagger(amount, direction);
      //Debug.Break();

	}
}


