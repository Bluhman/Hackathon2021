using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "References/Items/Weapon Database")]
public class WeaponDatabase : ScriptableObject
{
    public List<Weapon> value;
}
