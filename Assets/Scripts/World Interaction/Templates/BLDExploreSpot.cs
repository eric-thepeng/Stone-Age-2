using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLDExploreSpot : LevelUp, ISerialEffect
{
    [SerializeField] private int startState = 0;
    [SerializeField] private SO_SerialEffectIdentifier serialEffectIdentifier;

    private void Awake()
    {
        SetUpSerialEffectIdentifier();
    }

    private void Start()
    {
        if (startState == 1)
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
}
