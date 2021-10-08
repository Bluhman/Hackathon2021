using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemyController : WalkingController
{
	public float timeToStop1Unit = 0;
	public float timeToStop1UnitAir = 0;

	float frictionFactor;
	float aerialFrictionFactor;

	public override void Start()
	{
		base.Start();

		if (timeToStop1Unit != 0)
		{
			frictionFactor = 1 / Mathf.Pow(timeToStop1Unit, 2);
		}
		if (timeToStop1UnitAir != 0)
		{
			aerialFrictionFactor = 1 / Mathf.Pow(timeToStop1UnitAir, 2);
		}
	}

	public override void CalculateVelocity()
	{
		base.CalculateVelocity();
		float usedFrictionFactor = getIsAirborne() ? aerialFrictionFactor : frictionFactor;

		if (velocity.x > 0)
		{
			velocity.x = Mathf.Max(velocity.x - usedFrictionFactor, 0);
		}
		else if (velocity.x < 0)
		{
			velocity.x = Mathf.Min(velocity.x + usedFrictionFactor, 0);
		}
	}
}
