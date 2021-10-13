using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	public IntReference memoryCount;
    public IntReference equippedWeaponId;
    public WeaponDatabase weaponDB;

    [HideInInspector]
    public Weapon equippedWeapon;
	GameObject weaponPrefab;

    private void Start()
    {
        equippedWeapon = weaponDB.value.FirstOrDefault(w => w.id == equippedWeaponId.value);
        weaponPrefab = equippedWeapon.prefab;
    }

    public GameObject weaponEffect { get => weaponPrefab; }
}
