using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_0003_OpenResearchPanel : Quest
{
    protected override void StartQuest()
    {
        base.StartQuest();
        PlayerInputChannel.onPlayerPressWorldButton += CompleteQuest;
    }
    protected override void CompleteQuest()
    {
        PlayerInputChannel.onPlayerPressWorldButton -= CompleteQuest;
        base.CompleteQuest();
    }
    public void CompleteQuest(PlayerInputChannel.WorldButtons wb)
    {
        if (wb == PlayerInputChannel.WorldButtons.Research) CompleteQuest();
    }
}
