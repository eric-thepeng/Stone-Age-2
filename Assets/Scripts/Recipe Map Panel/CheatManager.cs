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
            AddMoney();
            AddItem();
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            UnlockCurrentRecipe();
        }
    }

    void AddMoney()
    {
        SpiritPoint.i.Add(500);
    }

    void AddItem()
    {
        foreach (ItemScriptableObject item in allItems)
        {
            Inventory.i.AddInventoryItem(item);
            Inventory.i.AddInventoryItem(item);
            Inventory.i.AddInventoryItem(item);
            Inventory.i.AddInventoryItem(item);
            Inventory.i.AddInventoryItem(item);
        }
    }

    public void UnlockCurrentRecipe()
    {
        RecipeMapManager.i.CheckUnlock(RecipeMapManager.i.DisplayBlock.myISO);
    }
}
