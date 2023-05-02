using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_0002_TryToGather : Quest
{

    protected override void StartQuest()
    {
        base.StartQuest();
        CharacterIcon.onCharacterStartGathering += SetComplete;//AddListener(SetComplete);
    }

    protected override void CompleteQuest()
    {
        CharacterIcon.onCharacterStartGathering -= SetComplete;//RemoveListener(SetComplete);
        base.CompleteQuest();
    }

    public void SetComplete()
    {
        CompleteQuest();
    }


}
