using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Ailments
{
	public int injury;
	public int exhaustion;
	public int poison;
	public int venom;
	public int curse;
	public int chill;
	public int petrify;
	public int banish;
	public T ReflectByName<T>(string propertyName)
	{
		return (T)GetType().GetField(propertyName).GetValue(this);
	}
}
