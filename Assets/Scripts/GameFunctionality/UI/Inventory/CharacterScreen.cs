using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScreen : MonoBehaviour
{
    public BoolReference boolReference;
    GameObject inventoryVisible;

    // Start is called before the first frame update
    void Start()
    {
        inventoryVisible = this.transform.GetChild(0).gameObject;
        inventoryVisible.SetActive(false);
        boolReference.value = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Pushed I");

            inventoryVisible.SetActive(!boolReference.value);
            boolReference.value = !boolReference.value;
        }    
    }
}
