using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DamageResistances
{
	//Resistances scale as percentage reduction.
	public float crushing;
	public float slashing;
	public float piercing;
	public float mystical;
	public float fire;
	public float electric;
	public float frost;
	public float dark;

	public void clampAll()
	{
		crushing = Mathf.Clamp(crushing, -10, 1);
		slashing = Mathf.Clamp(crushing, -10, 1);
		piercing = Mathf.Clamp(crushing, -10, 1);
		mystical = Mathf.Clamp(crushing, -10, 1);
		crushing = Mathf.Clamp(crushing, -10, 1);
		crushing = Mathf.Clamp(crushing, -10, 1);
		crushing = Mathf.Clamp(crushing, -10, 1);
		crushing = Mathf.Clamp(crushing, -10, 1);
	}

	public T ReflectByName<T>(string propertyName)
	{
		return (T)GetType().GetField(propertyName).GetValue(this);
	}
}
