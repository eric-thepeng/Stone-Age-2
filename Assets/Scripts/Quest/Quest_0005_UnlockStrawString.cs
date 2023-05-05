using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_0005_UnlockStrawString : Quest
{
    [SerializeField] private ItemCraftScriptableObject icsoToCheck;
    protected override void StartQuest()
    {
        base.StartQuest();
        RecipeMapBlock.onRecipeMapBlockUpgrade += checkComplete;
    }

    protected override void CompleteQuest()
    {
        RecipeMapBlock.onRecipeMapBlockUpgrade -= checkComplete;
        base.CompleteQuest();
    }

    void checkComplete(ItemCraftScriptableObject icso, int level)
    {
        print(icso + " " + level);
        if(level == 4 && icso == icsoToCheck) CompleteQuest();
    }
}
