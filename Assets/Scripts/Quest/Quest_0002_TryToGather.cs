using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_0002_TryToGather : Quest
{
    public Quest_0002_TryToGather()
    {
        questName = "Try to gather";
        questDescription = "Drag Mr. Birdy's character icon from top-left to the Meadow Explore Spot";
        questID = "Quest_0002";
    }

    public override void StartQuest()
    {
        base.StartQuest();
        CharacterIcon.onCharacterStartGathering.AddListener(SetComplete);
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        CharacterIcon.onCharacterStartGathering.RemoveListener(SetComplete);
    }

    public void SetComplete()
    {
        CompleteQuest();
    }


}
