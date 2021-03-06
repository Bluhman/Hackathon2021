using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    public Item item;
    public ItemReference itemReference;
    public ItemListReference playerInventory;
    public WeaponDatabase weaponDb;
    public IntReference weaponId;
    public BoolReference needToRerenderInventory;

    private GameObject image;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick() 
    { 
        if (this.item.itemType == ItemType.Weapon)
        {
            int oldWeaponId = weaponId.value;
            weaponId.value = item.id;     
            playerInventory.value = playerInventory.value.Where(a => a.id != weaponId.value).ToList();
            playerInventory.value.Add(weaponDb.value.FirstOrDefault(a => a.id == oldWeaponId));
            needToRerenderInventory.value = true;
        }
    }

    public void SetUpItem(Item newItem)
    {
        image = this.transform.GetChild(0).gameObject;
        item = newItem;

        var sprite = Sprite.Create(item.inventoryImage, new Rect(new Vector2(), new Vector2(200f, 200f)), new Vector2());
        image.GetComponent<Image>().sprite = sprite;

    }

    public void OnSelect(BaseEventData eventData)
    {
        SetInventoryInfo();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetInventoryInfo();
    }

    void SetInventoryInfo()
    {
        itemReference.value = item;
    }
}
