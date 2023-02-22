using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    public InventoryItemSO[] initialItems;
    private void Start()
    {
        foreach(InventoryItemSO iiso in initialItems)
        {
            Inventory.i.AddInventoryItem(iiso);
        }
    }
}
