using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLDExploreSpot : LevelUp, ISerialEffect
{
    [Header("---Assign only [Set Up Info] and nothing else---")]
    [SerializeField] private SO_ExploreSpotSetUpInfo setUpInfo = null;
    private int startState = 0;
    private SO_SerialEffectIdentifier serialEffectIdentifier;

    private void Awake()
    {
        if (setUpInfo != null)
        {
            GetUnlockState(1).SetUpUnlockCost(setUpInfo.unlockResourceSet);
            serialEffectIdentifier = setUpInfo.serialEffectIdentifier;
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
        serialEffectIdentifier.SendSerialEffect();
    }

    public void SetUpSerialEffectIdentifier()
    {
        serialEffectIdentifier.SetUpSerialEffectInterface(this);
    }

    public void ReceiveSerialEffect()
    {
        unlockToState_1_Locked();
    }

    public SO_SerialEffectIdentifier mySEI { get => serialEffectIdentifier; }

    #endregion

}
