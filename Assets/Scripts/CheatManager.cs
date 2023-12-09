using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    [SerializeField]
    ItemSOListScriptableObject allISOList;

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

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            AddResourceForTrash();
        }
    }

    void AddMoney()
    {
        SpiritPoint.i.Add(5);
    }

    void AddItem()
    {
        foreach (ItemScriptableObject item in allISOList.list)
        {
            Inventory.i.AddInventoryItem(item,5);
        }
    }

    void AddResourceForTrash()
    {
        foreach (ItemScriptableObject item in allISOList.list)
        {
            if (item.name == "ISO_Apple")
            {
                Inventory.i.AddInventoryItem(item, 4);
            }
        }
    
    }

    public void UnlockCurrentRecipe()
    {
        if (RecipeMapManager.i.DisplayBlock != null)
        {
            RecipeMapManager.i.CheckUnlock(RecipeMapManager.i.DisplayBlock.myISO);
        }  
    }
}
