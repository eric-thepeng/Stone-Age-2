using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    [SerializeField]
    GameObject allItemGO;

    List<ItemScriptableObject> allItems = new List<ItemScriptableObject>();

    private void Start()
    {
        foreach (Transform child in allItemGO.transform)
        {
            allItems.Add(child.GetComponent<Tetris>().itemSO);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            SpiritPoint.i.Add(100);
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            foreach (ItemScriptableObject item in allItems)
            {
                Inventory.i.AddInventoryItem(item);
            }
        }
    }
}
