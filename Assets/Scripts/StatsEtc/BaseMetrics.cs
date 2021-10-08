using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseMetrics
{
	public int health;
	public int stamina;
	int _currentHealth;
	int _currentStamina;
	public int currentHealth
	{
		get => _currentHealth;
		set
		{
			_currentHealth = Mathf.Clamp(value, 0, health);
		}
	}
	public int currentStamina
	{
		get => _currentStamina;
		set
		{
			_currentStamina = Mathf.Clamp(value, int.MinValue, stamina);
		}
	}
	public int equipLoad;
	public float weight;
	public int footing;
	int _currentFooting;
	public int currentFooting
	{
		get => _currentFooting; set
		{
			_currentFooting = value;
		}
	}
	public int balance;
	public DamageResistances damageResistances;
	public Ailments statusResistances;
	Ailments statusBuildup;
	//Timer: if above 0, the target is affected.
	Ailments statusTimers;

	public float percentLoaded { get => weight / equipLoad; }

	public void InitCurrents()
	{
		_currentHealth = health;
		_currentStamina = stamina;
		_currentFooting = footing;
	}

	public T ReflectByName<T>(string propertyName)
	{
		return (T)GetType().GetField(propertyName).GetValue(this);
	}
}
