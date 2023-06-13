using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLDTrashToClear : LevelUp
{
    [SerializeField]private GameObject UI;
    [SerializeField] private float timeToClear = 0.5f;
    private float pressedTime = 0f;
    bool logPressing = false;
    protected override void BeginMouseHover()
    {
        base.BeginMouseHover();
        TurnOnUI();
    }

    protected override void EndMouseHover()
    {
        TurnOffUI();
        base.EndMouseHover();
    }

    protected override void BeginMousePress()
    {
        base.BeginMousePress();
        logPressing = true;
    }

    protected override void WhileMousePress()
    {
        base.WhileMousePress();
        if (logPressing)
        {
            pressedTime += Time.deltaTime;
            if (pressedTime > timeToClear)
            {
                UnlockToNextState();
                pressedTime = 0;
                logPressing = false;
            }
        }
    }

    protected override void EndMousePress()
    {
        base.EndMousePress();
        pressedTime = 0;
    }

    private void TurnOnUI()
    {
        UI.SetActive(true);
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
