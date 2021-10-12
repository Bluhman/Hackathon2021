using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	public IntReference memoryCount;
    public IntReference equippedWeaponId;
	[HideInInspector]
	public Weapon equippedWeapon;
	GameObject weaponPrefab;

	private void Start()
	{
		var weaponDB = GameObject.FindGameObjectWithTag("GameController")
			.GetComponent<WeaponDatabase>().weapons;

		equippedWeapon = weaponDB.FirstOrDefault(w => w.id == equippedWeaponId.value);

		weaponPrefab = equippedWeapon.prefab;
	}

	public GameObject weaponEffect { get => weaponPrefab; }
}
