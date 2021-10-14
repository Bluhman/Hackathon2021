using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Item item;
    public ItemListReference playerInventory;
    public WeaponDatabase weaponDb;
    public IntReference weaponId;

    private Transform parent;
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
            weaponId.value = item.id;
        }
    }

    public void SetUpItem(Item newItem)
    {
        parent = this.transform.parent;
        image = parent.Find("Image").gameObject;

        item = newItem;

        var sprite = Sprite.Create(item.inventoryImage, new Rect(new Vector2(), new Vector2(200f, 200f)), new Vector2());
        image.GetComponent<Image>().sprite = sprite;

    }
}
