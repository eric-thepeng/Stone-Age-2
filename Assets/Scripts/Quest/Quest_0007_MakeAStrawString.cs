using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_0007_MakeAStrawString : Quest
{
    [SerializeField] GameObject indications;
    [SerializeField] ItemScriptableObject isoNeedToCraft;

    protected override void StartQuest()
    {
        base.StartQuest();
        indications.SetActive(true);
        RecipeCombinator.onNewItemCrafted += CheckComplete;
    }

    protected override void CompleteQuest()
    {
        RecipeCombinator.onNewItemCrafted -= CheckComplete;
        indications.SetActive(false);
        base.CompleteQuest();
    }

    void CheckComplete(ItemScriptableObject iso)
    {
        if (iso == isoNeedToCraft) CompleteQuest();
    }
}
