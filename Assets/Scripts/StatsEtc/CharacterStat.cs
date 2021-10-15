using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterStat : MonoBehaviour
{
	public BaseMetrics charMetrics;
	public int staminaRegenPerSec = 34;
	public int footingRegenPerSec = 10;

	float speedMultiplier = 1;
	float jumpMultiplier = 1;
	float gravityMultiplier = 1;
	protected float staminaRegenMultiplier = 1;
	float damageMultiplier = 1;
	float healingMultiplier = 1;

	public DamageResistances blockResistances;
	public float blockFootingScalar = 0.5f;

	BoxCollider2D hitbox;

	// Start is called before the first frame update
	public virtual void Start()
	{
		Debug.Log(charMetrics);

		hitbox = gameObject.GetComponent<BoxCollider2D>();
		charMetrics.InitCurrents();
		verifyLoad();
	}

	//Run to assert the current equip load conditions of the character.
	public void verifyLoad()
	{
		var load = charMetrics.percentLoaded;
		if (load < 0.3f)
		{
			Debug.Log("Light");
		}
		else if (load < 0.7f)
		{
			Debug.Log("Unencumbered");
		}
		else if (load <= 1)
		{
			Debug.Log("Encumbered");
		} else
		{
			Debug.Log("Overloaded");
		}
	}

	public virtual void Update()
	{

	}

	public virtual void DrainStamina(int amount, float pauseBeforeRegen) {
		charMetrics.currentStamina -= amount;
		CancelInvoke();
		InvokeRepeating("RegenerateStamina", pauseBeforeRegen, 1f / staminaRegenPerSec * staminaRegenMultiplier);
	}

	public virtual void ConstantStaminaDrain(int amountPerSec)
	{
		CancelInvoke();
		InvokeRepeating("DrainStaminaOverTime", 0f, 1f / amountPerSec);
	}

	protected virtual void DrainStaminaOverTime()
	{
		charMetrics.currentStamina -= 1;
	}

	public virtual void DrainFooting(int amount, Vector2 direction)
	{
		charMetrics.currentFooting -= amount;

		if (charMetrics.currentFooting < 0)
		{
			OnStagger(amount, direction);
		} 
		else
		{
			InvokeRepeating("RegenerateFooting", 1f / footingRegenPerSec, 1f / footingRegenPerSec);
		}
		OnHit();
	}

	protected virtual void OnHit()
	{
		
	}

	protected virtual void OnStagger(int amount, Vector2 direction)
	{
		var physicsSim = gameObject.GetComponent<WalkingController>();
		//Add some physics to the target.
		if (physicsSim != null)
		{
			physicsSim.velocity += (Vector3)direction * Mathf.Abs(charMetrics.currentFooting);
		}

		//Debug.Log(physicsSim.velocity);

		//Immediately restore all footing.
		charMetrics.currentFooting = charMetrics.footing;

		//Pause stamina regen.
		CancelInvoke("RegenerateStamina");
		InvokeRepeating("RegenerateStamina", 1, 1f / staminaRegenPerSec * staminaRegenMultiplier);
	}

	public virtual void RegenerateStamina()
	{
		//Debug.Log("Regenerating.");
		charMetrics.currentStamina += 1;
		if (charMetrics.currentStamina == charMetrics.stamina)
		{
			//Debug.Log("stop");
			//We can stop now.
			CancelInvoke("RegenerateStamina");
		}
	}

	public virtual void RegenerateFooting()
	{
		charMetrics.currentFooting += 1;
		if (charMetrics.currentFooting == charMetrics.footing)
		{
			CancelInvoke("RevenerateFooting");
		}
	}
	
}
