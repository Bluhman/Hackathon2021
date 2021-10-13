using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    public BoolReference isVisible;
    private int currentInventoryCount = 0;
    private bool wasRendered = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInventory();
    }

    public void onClose()
    {
        wasRendered = false;

    }

    private void UpdateInventory()
    {
        if (isVisible)
        {
            if (!wasRendered)
            {
                var items = GetInventory();

                foreach (var i in items)
                {
                    // add items to the inventory thing
                }

                wasRendered = true;
            }
        }
    }

    private Item[] GetInventory()
    {
        return new Item[0];
        // TODO: Get the inventory list from somewhere. Maybe this should be a scriptable object.    
    }

}
