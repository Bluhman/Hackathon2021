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
		fire = Mathf.Clamp(crushing, -10, 1);
		electric = Mathf.Clamp(crushing, -10, 1);
		frost = Mathf.Clamp(crushing, -10, 1);
		dark = Mathf.Clamp(crushing, -10, 1);
	}

	public T ReflectByName<T>(string propertyName)
	{
		return (T)GetType().GetField(propertyName).GetValue(this);
	}

	public DamageResistances Clone()
	{
		return new DamageResistances()
		{
			crushing = this.crushing,
			slashing = this.slashing,
			piercing = this.piercing,
			mystical = this.mystical,
			fire = this.fire,
			electric = this.electric,
			frost = this.frost,
			dark = this.dark
		};
	}
}
