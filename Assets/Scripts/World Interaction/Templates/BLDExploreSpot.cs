using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State 0: Hidden
/// State 1: Locked
/// State 2: Unlocked
/// </summary>
public class BLDExploreSpot : LevelUp, ISerialEffect
{
    [Header("---Assign only [Set Up Info] and nothing else---")]
    [SerializeField] private SO_ExploreSpotSetUpInfo setUpInfo = null;
    private int startState = 0;
    //private SO_SerialEffectIdentifier serialEffectIdentifier;
    bool mouseOver = false;

    private void Awake()
    {
        if (setUpInfo != null)
        {
            GetUnlockState(1).SetUpUnlockCost(setUpInfo.unlockResourceSet);
            GatherSpot gatherSpot = GetComponentInChildren<GatherSpot>(includeInactive: true);
            gatherSpot.gatherTime = setUpInfo.gatherTime;
            gatherSpot.gatherResource = setUpInfo.gatherResource;
            SetUpSerialEffectIdentifier();
        }
        else
        {
            Debug.LogError(name + " does not have a Explore Spot Set Up Info");
        }
    }

    public SO_ExploreSpotSetUpInfo GetSetUpInfo()
    {
        return setUpInfo;
    }

    private void Start()
    {
        if (setUpInfo.startInLockedState)
        {
            unlockToState_1_Locked();
        }
    }

    public void unlockToState_1_Locked()
    {
        if(GetCurrentState()!=0) return;
        if(!UnlockToNextState()) return;
    }

    public void unlockToState_2_Unlocked()
    {
        if(GetCurrentState()!=1)return;
        if(!UnlockToNextState()) return;
        SendSerialEffect();
    }

    #region ISerialEffect

    public void SendSerialEffect()
    {
        mySEI.SendSerialEffect();
    }

    public void SetUpSerialEffectIdentifier()
    {
        mySEI.SetUpSerialEffectInterface(this);
    }

    public void ReceiveSerialEffect(SO_SerialEffectIdentifier origionSEI)
    {
        unlockToState_1_Locked();
        UI_ExploreSpotsConnection.i.UnlockLine(origionSEI, mySEI);
    }

    public SO_SerialEffectIdentifier mySEI { get => setUpInfo.serialEffectIdentifier; }

    #endregion
    private void OnMouseEnter()
    {
        if (setUpInfo == null) return;
        TooltipManager.i.ShowMapTip(GetSetUpInfo(), TooltipManager.ToolMode.INVENTORYRECRAFT);
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        if (setUpInfo == null) return;
        TooltipManager.i.DisableTip();
        mouseOver = false;
    }


}
