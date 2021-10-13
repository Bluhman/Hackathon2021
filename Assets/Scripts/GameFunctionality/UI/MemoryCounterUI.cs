using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryCounterUI : MonoBehaviour
{
    public IntReference intReference;
    private Text memories;

    // Start is called before the first frame update
    void Start()
    {
        memories = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        memories.text = intReference.value.ToString();
    }
}
