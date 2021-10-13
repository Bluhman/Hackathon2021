using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
	Weapon,
	Armor,
	Consumable,
}

[Serializable]
public class Item
{
	public ItemType itemType;
	public int id;
	public string name;
	public string description_short;
	public string description_long;
}
