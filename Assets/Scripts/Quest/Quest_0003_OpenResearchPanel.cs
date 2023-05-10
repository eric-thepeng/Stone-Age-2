using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Quest_0003_OpenResearchPanel : Quest
{
    [SerializeField] GameObject arrow;
    [SerializeField] Transform researchButton;
    Vector3 orgLocalPosition;
    private void Start()
    {
        orgLocalPosition = researchButton.localPosition;
        researchButton.localPosition -= new Vector3(0f, 1.7f, 0f);
    }

    protected override void StartQuest()
    {
        base.StartQuest();
        researchButton.DOLocalMoveY(orgLocalPosition.y,0.8f);
        arrow.SetActive(true);
        PlayerInputChannel.onPlayerPressWorldButton += CompleteQuest;

    }

    protected override void CompleteQuest()
    {
        arrow.SetActive(false);
        PlayerInputChannel.onPlayerPressWorldButton -= CompleteQuest;
        base.CompleteQuest();
    }
    void CompleteQuest(PlayerInputChannel.WorldButtons buttonPressed)
    {
        if (buttonPressed == PlayerInputChannel.WorldButtons.Research)
        {
            CompleteQuest();
        }
    }


}
