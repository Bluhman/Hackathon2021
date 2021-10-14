using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public ItemReference item;
    public WeaponDatabase weaponDb;
    public IntReference equippedWeapon;

    Text itemName;
    Image itemImage;
    Text itemDescription;

    int currentItemId;

    void Start()
    {
        itemName = this.transform.GetChild(0).gameObject.GetComponent<Text>();
        itemImage = this.transform.GetChild(1).gameObject.GetComponent<Image>();
        itemDescription = this.transform.GetChild(3).gameObject.GetComponent<Text>();        
    }

    // Update is called once per frame
    void Update()
    {
        if (item.value.id != currentItemId)
        {
            SetItemInfo();
            currentItemId = item.value.id;
        }    
    }

    private void OnEnable()
    {
        item.value = weaponDb.value.FirstOrDefault(a => a.id == equippedWeapon.value);
        currentItemId = item.value.id;
        SetItemInfo();
    }

    void SetItemInfo()
    {
        itemName.text = item.value.name;
        itemImage.sprite = Sprite.Create(item.value.inventoryImage, new Rect(new Vector2(), new Vector2(200f, 200f)), new Vector2());
        itemDescription.text = item.value.description_long;
    }
}
