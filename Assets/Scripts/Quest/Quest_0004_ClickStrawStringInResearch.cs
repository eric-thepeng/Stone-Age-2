using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Quest_0004_ClickStrawStringInResearch : Quest
{
    [SerializeField] private GameObject strawStringArrow;
    [SerializeField] private ItemCraftScriptableObject icsoToCheck;
    protected override void StartQuest()
    {
        base.StartQuest();
        strawStringArrow.SetActive(true);
        RecipeMapBlock.onRecipeMapBlockChosen += MakeComplete;
    }

    protected override void CompleteQuest()
    {
        Destroy(strawStringArrow);
        base.CompleteQuest();
    }

    private void MakeComplete(ItemCraftScriptableObject icso)
    {
        if(icso == icsoToCheck) CompleteQuest();
    }
}
