using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "References/Items/Item List Reference")]
public class ItemListReference : ScriptableObject
{
    [SerializeField] public List<Item> value;
}
