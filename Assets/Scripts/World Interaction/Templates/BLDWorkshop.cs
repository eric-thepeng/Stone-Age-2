using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLDWorkshop : WorldInteractable
{
    private UI_BLDWorkshop ui;

    enum State
    {Idle, Assigning
    }

    private State state = State.Idle;

    private bool isWorking = false;

    private void Start()
    {
        ui = GetComponent<UI_BLDWorkshop>();
    }

    protected override void BeginMousePress()
    {
        if (state == State.Idle)
        {
            ui.TurnOnUI();
            state = State.Assigning;
        }
        base.BeginMousePress();
    }

    protected override void TurnOnHighlight()
    {
        if(state == State.Assigning) return;
        base.TurnOnHighlight();
    }


    /*
    protected override void EndMouseHover()
    {
        if (state == State.Assigning)
        {
            ui.TurnOffUI();
            state = State.Idle;
        }
        base.EndMouseHover();
    }*/

    public void ExitUI()
    {
        if (state == State.Assigning)
        {
            ui.TurnOffUI();
            state = State.Idle;
        }
        else
        {
            Debug.LogError("Exit workshop UI while UI is not opened?");
        }
    }
}
