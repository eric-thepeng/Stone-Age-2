using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using Hypertonic.GridPlacement;

public class BLDTrashToClear : LevelUp
{
    [Header("---DO NOT EDIT CurrentInteraction above---")]
    [Header("Only assign timeToClear, unlockGain, unlockCost")]
    [SerializeField]private float timeToClear = 3f;
    [SerializeField] private ResourceSet gainResourceSet;
    private float pressedTime = 0f;
    bool logPressing = false;
    UI_BLDTrashToClear ui;

    private ResourceSet unlockResourceSet;

    private void Start()
    {
        ui = GetComponent<UI_BLDTrashToClear>();
        unlockResourceSet = GetCurrentUnlockState().unlockCost;
        SetCurrentInteraction(new InteractionType(InteractionType.TypeName.LongPress,TryClearTrash,timeToClear));
    }

    protected override void BeginMouseHover()
    {
        base.BeginMouseHover();
        TurnOnUI();
    }

    protected override void EndMouseHover()
    {
        base.EndMouseHover();
        TurnOffUI();
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
        PlayerStatsMonitor.trashTotalClearedPlayerStat.ChangeAmount(1);
        Destroy(gameObject);
        
        /*
        // temp
        GridManagerAccessor.GridManager.ObjectToPlace.GetComponent<GridValidator>().collisionCount--;
        if (GridManagerAccessor.GridManager.ObjectToPlace.GetComponent<GridValidator>().collisionCount == 0)
        {
            GridManagerAccessor.GridManager.ObjectToPlace.GetComponent<GridValidator>().HandleExitedWallArea();
        }*/
        

    }


    public void TryClearTrash()
    {
        if (base.UnlockToNextState())
        {
            return;
        }
        else
        {
            SetCurrentInteraction(new InteractionType(InteractionType.TypeName.LongPress,TryClearTrash,timeToClear));
        }
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
