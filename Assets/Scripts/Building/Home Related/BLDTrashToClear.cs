using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLDTrashToClear : LevelUp
{
    [SerializeField]private GameObject UI;
    [SerializeField] private float timeToClear = 0.5f;
    private float pressedTime = 0f;
    bool logPressing = false;
    UI_BLDUnlock unlockUI;

    enum State {Idle, Selected}
    State state = State.Idle;

    private void Start()
    {
    }

    void ChangeStateTo(State newState)
    {
        if(newState == State.Selected)
        {
            state = State.Selected;
            TurnOnUI();
            unlockUI.SetProgress(pressedTime/timeToClear);
        }
        else if(newState == State.Idle)
        {
            state = State.Idle;
            TurnOffUI();
        }
    }

    /*
    protected override void 
    {
        base.MouseClick();
        if (state == State.Idle) ChangeStateTo(State.Selected);
    }*/


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
            unlockUI.SetProgress(Mathf.Clamp(pressedTime / timeToClear,0,1));
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
        unlockUI.SetProgress(0);
    }

    protected override void EndMousePress()
    {
        base.EndMousePress();
        if (state == State.Selected)
        {
            pressedTime = 0;
        }
    }

    private void TurnOnUI()
    {
        UI.SetActive(true);
        unlockUI = GetComponentInChildren<UI_BLDUnlock>();

        IPassResourceSet iprs = GetComponentInChildren<IPassResourceSet>();
        
        if(iprs != null)iprs.PassResourceSet(GetCurrentUnlockState().unlockCost);
    }

    public void TurnOffUI()
    {
        UI.SetActive(false);
    }

    protected override void ReachFinalState()
    {
        base.ReachFinalState();
        Destroy(gameObject);
    }
}
