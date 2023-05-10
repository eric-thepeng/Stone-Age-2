using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Quest_0006_OpenCraftingPanel : Quest
{
    [SerializeField] Transform craftButton;
    [SerializeField] GameObject arrow;
    Vector3 orgLocalPos;
    private void Start()
    {
        orgLocalPos = craftButton.localPosition;
        craftButton.localPosition -= new Vector3(0f, 2f, 0f);
    }

    protected override void StartQuest()
    {
        base.StartQuest();
        craftButton.DOLocalMoveY(orgLocalPos.y,0.5f);
        PlayerInputChannel.onPlayerPressWorldButton += CompleteQuest;
        arrow.SetActive(true);

    }

    protected override void CompleteQuest()
    {
        arrow.SetActive(false);
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
