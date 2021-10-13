using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    public BoolReference isVisible;
    public bool wasRendered;
    public ItemListReference playerInventory;
    public WeaponDatabase weaponDatabase;

    public List<GameObject> renderedInventoryItems;

    // Start is called before the first frame update
    void Start()
    {
        //this needs to go at some point
        playerInventory.value = new List<Item>();
        foreach (var i in weaponDatabase.value)
        {
            playerInventory.value.Add(i);
        }
    }

    private void OnEnable()
    {
        renderedInventoryItems = new List<GameObject>();
    }

    private void OnDisable()
    {
        wasRendered = false;
        foreach (var i in renderedInventoryItems)
        {
            Destroy(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInventory();
    }

    private void UpdateInventory()
    {
        // is the panel open
        if (isVisible)
        {
            // has this panel already been rendered since opening the slot (so we dont loop this for no reason)
            if (!wasRendered)
            {
                var items = GetInventory();

                // iterate through the inventory
                for (int i = 0; i < items.Count; i++)
                {
                    var iconSprite = Sprite.Create(items[i].inventoryImage, new Rect(new Vector2(), new Vector2(200f, 200f)), new Vector2());

                    var panel = this.transform.GetChild(i);
                    GameObject icon = new GameObject();
                    var image = icon.AddComponent<Image>();
                    image.transform.localScale = new Vector3(.35f, .35f);
                    image.sprite = iconSprite;

                    renderedInventoryItems.Add(Instantiate(icon, parent: panel.transform));
                }

                // after we render this, we won't render it again.
                wasRendered = true;
            }
        }
    }

    private List<Item> GetInventory()
    {
        return playerInventory.value;   
    }


}
