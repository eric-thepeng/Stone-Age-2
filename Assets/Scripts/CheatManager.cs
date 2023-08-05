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
    }

    void AddMoney()
    {
        SpiritPoint.i.Add(500);
    }

    void AddItem()
    {
        foreach (ItemScriptableObject item in allISOList.list)
        {
            Inventory.i.AddInventoryItem(item,5);
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
