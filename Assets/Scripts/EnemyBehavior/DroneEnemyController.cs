using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneEnemyController : CharacterStat
{
   //Height of oscillation in units:
   public float oscillationHeight;
   //Oscillations made in a second:
   public float oscillationFreq;
   public float horizontalSpeed;
   //TimeToFullSpeed is acceleration. Lower times equals more resilience to knockback.
   public float timeToFullSpeed;
   public bool goingLeft;
   public bool startAtFullSpeed;

   public GameObject propeller;
   public GameObject sprite;
   public GameObject attackHitBox;

   Vector3 velocity;
   float acceleration;
   float phase;

    // Start is called before the first frame update
    public override void Start()
    {
      base.Start();

      acceleration = 2 * horizontalSpeed / Mathf.Pow(timeToFullSpeed, 2);

      propeller.GetComponent<SpriteRenderer>().flipX = goingLeft;
      sprite.GetComponent<SpriteRenderer>().flipX = goingLeft;
      var box = gameObject.GetComponent<BoxCollider2D>();
      box.offset = new Vector2(box.offset.x * (goingLeft ? -1 : 1), box.offset.y);

      if (startAtFullSpeed)
		{
         velocity.x = horizontalSpeed * (goingLeft ? -1 : 1);

      }
    }

    // Update is called once per frame
    public override void Update()
    {
      phase += Time.deltaTime * (oscillationFreq);
      if (phase >= 1)
		{
         phase -= 1;
		}

      var derivativeOfSin = Mathf.Sin(phase * (Mathf.PI*2) );
      velocity.y = derivativeOfSin * (oscillationHeight*oscillationFreq);
      velocity.x += acceleration * (goingLeft ? -1 : 1) * Time.deltaTime;
      var totalXSpeed = Mathf.Abs(velocity.x);
      if (totalXSpeed > horizontalSpeed)
		{
         //Lerp back to normal.
         velocity.x = Mathf.Lerp(totalXSpeed, horizontalSpeed, 0.1f) * Mathf.Sign(velocity.x);
		}

      transform.Translate(velocity * Time.deltaTime);
      
    }

	protected override void OnStagger(int amount, Vector2 direction)
	{
      //Immediately restore all footing.
      charMetrics.currentFooting = charMetrics.footing;
      velocity += (Vector3)direction * Mathf.Abs(charMetrics.currentFooting);

      //Pause stamina regen...
      //Not that it matters since this enemy type doesn't really use stamina.
      CancelInvoke("RegenerateStamina");
      InvokeRepeating("RegenerateStamina", 1, 1f / staminaRegenPerSec * staminaRegenMultiplier);
   }
}
