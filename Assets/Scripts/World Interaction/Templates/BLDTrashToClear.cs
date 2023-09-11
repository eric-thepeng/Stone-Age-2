using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class BLDTrashToClear : LevelUp
{
    [SerializeField] private float timeToClear = 0.5f;
    [SerializeField] private ResourceSet gainResourceSet;
    private float pressedTime = 0f;
    bool logPressing = false;
    UI_BLDTrashToClear ui;

    enum State {Idle, Selected}
    State state = State.Idle;

    private ResourceSet unlockResourceSet;

    void ChangeStateTo(State newState)
    {
        if(newState == State.Selected)
        {
            state = State.Selected;
            TurnOnUI();
            ui.SetProgress(pressedTime/timeToClear);
        }
        else if(newState == State.Idle)
        {
            state = State.Idle;
            TurnOffUI();
        }
    }

    private void Start()
    {
        ui = GetComponent<UI_BLDTrashToClear>();
        unlockResourceSet = GetCurrentUnlockState().unlockCost;
    }
    

    protected override void EndMouseHover()
    {
        if (state == State.Selected) ChangeStateTo(State.Idle);
        base.EndMouseHover();
    }

    protected override void BeginMousePress()
    {
        base.BeginMousePress();
        if (state == State.Idle) ChangeStateTo(State.Selected);
        else if (state == State.Selected)logPressing = true;
    }

    protected override void WhileMousePress()
    {
        base.WhileMousePress();
        if (logPressing)
        {
            pressedTime += Time.deltaTime;
            ui.SetProgress(Mathf.Clamp(pressedTime / timeToClear,0,1));
            if (pressedTime > timeToClear)
            {
                UnlockToNextState();
                pressedTime = 0;
                logPressing = false;
            }
        }
    }

    protected override void NotEnoughResource()
    {
        base.NotEnoughResource();
        ui.SetProgress(0);
    }

    protected override void EndMousePress()
    {
        base.EndMousePress();
        if (state == State.Selected)
        {
            pressedTime = 0;
            ui.SetProgress(Mathf.Clamp(pressedTime / timeToClear,0,1));
        }
    }

    private void TurnOnUI()
    {
        ui.TurnOnUI();
    }

    public void TurnOffUI()
    {
        ui.TurnOffUI();
    }

    protected override void ReachFinalState()
    {
        base.ReachFinalState();
        gainResourceSet.GainResource();
        Destroy(gameObject);
    }

    public override ResourceSet ProvideResourceSet(int index = 0)
    {
        if (index == 0)
        {
            return unlockResourceSet;
        }
        else if (index == 1)
        {
            return gainResourceSet;
        }
        else
        {
            Debug.LogError("BLDTrashToClear receive non-existing index to provide resource set.");
            return null;
        }
    }
}
