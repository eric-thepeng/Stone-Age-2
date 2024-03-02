using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIS_CharacterRecruit : MonoBehaviour, ISerialEffect
{
    [SerializeField] private CharacterBasicStats characterToRecruit;
    [SerializeField] private int recruitAtLevelState = 2;
    [SerializeField] private SO_SerialEffectIdentifier serialEffectIdentifier;


    private void Awake()
    {
        SetUpSerialEffectIdentifier();
        GetComponent<LevelUp>().GetCurrentStatePlayerStat().SubscribeStatChange(CheckToRecruit);
    }

    private void CheckToRecruit(int level)
    {
        if (level == recruitAtLevelState)
        {
            CharacterManager.i.AddCharacter(characterToRecruit);
            SendSerialEffect();
        }
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

    public void ReceiveSerialEffect(SO_SerialEffectIdentifier origionSEI)
    {
        //unlockToState_1_Locked();
        UI_ExploreSpotsConnection.i.UnlockLine(origionSEI, mySEI);
    }

    public SO_SerialEffectIdentifier mySEI { get => serialEffectIdentifier; }

    #endregion
}
