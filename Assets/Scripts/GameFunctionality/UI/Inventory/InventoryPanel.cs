using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    public BoolReference isVisible;
    public ItemListReference playerInventory;
    public WeaponDatabase weaponDatabase;
    public GameObject inventoryItemPrefab;

    List<GameObject> renderedInventoryItems;
    bool wasRendered;

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
                    var item = Instantiate(inventoryItemPrefab, this.transform);
                    item.transform.Find("ButtonManager").gameObject.GetComponent<InventoryItem>().SetUpItem(items[i]);
                    renderedInventoryItems.Add(item);
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
