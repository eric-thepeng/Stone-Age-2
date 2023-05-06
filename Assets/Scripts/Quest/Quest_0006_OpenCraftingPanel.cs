using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_0006_OpenCraftingPanel : Quest
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

    void CompleteQuest(PlayerInputChannel.WorldButtons buttonPressed)
    {
        if (buttonPressed == PlayerInputChannel.WorldButtons.Crafting)
        {
            CompleteQuest();
        }
    }
}
