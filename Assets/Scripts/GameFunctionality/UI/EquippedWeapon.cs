using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EquippedWeapon : MonoBehaviour
{
    // Start is called before the first frame update

    public WeaponDatabase weaponDatabase;
    public IntReference equippedWeapon;

    int currentWeapon;
    GameObject weaponImage;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWeapon != equippedWeapon.value)
        {
            SetWeaponImage();
        }
    }

    private void OnEnable()
    {
        weaponImage = this.gameObject;
        SetWeaponImage();
    }

    void SetWeaponImage()
    {
        // get the current equiped weapon from DB.
        var weapon = weaponDatabase.value.FirstOrDefault(a => a.id == equippedWeapon.value);     
        var sprite = Sprite.Create(weapon.inventoryImage, new Rect(new Vector2(), new Vector2(200f, 200f)), new Vector2());
        weaponImage.GetComponent<Image>().sprite = sprite;
        currentWeapon = equippedWeapon.value;
    }
}
