using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    public ItemScriptableObject[] initialItems;
    private void Start()
    {
        foreach(ItemScriptableObject iiso in initialItems)
        {
            Inventory.i.AddInventoryItem(iiso);
        }
    }
}
