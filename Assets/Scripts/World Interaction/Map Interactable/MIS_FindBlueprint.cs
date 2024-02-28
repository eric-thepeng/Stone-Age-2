using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MIS_FindBlueprint : MonoBehaviour, ISerialEffect
{
    [SerializeField] private List<ItemCraftScriptableObject> blueprintsToObtain;
    [SerializeField] private int obtainAtLevelState = 2;
    [SerializeField] private SO_SerialEffectIdentifier serialEffectIdentifier;

    private LevelUp myLevelUp;

    private void Awake()
    {
        myLevelUp = GetComponent<LevelUp>();
        SetUpSerialEffectIdentifier();
        myLevelUp.GetCurrentStatePlayerStat().SubscribeStatChange(CheckToRecruit);
    }

    private void CheckToRecruit(int level)
    {
        if (level == obtainAtLevelState)
        {
            //CharacterManager.i.AddCharacter(characterToRecruit);
            BlueprintManager.i.ObtainBlueprints(blueprintsToObtain);
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
        if(myLevelUp.GetCurrentState()!=0) return;
        if(!myLevelUp.UnlockToNextState()) return;
        UI_ExploreSpotsConnection.i.UnlockLine(origionSEI, mySEI);
    }

    public SO_SerialEffectIdentifier mySEI { get => serialEffectIdentifier; }

    #endregion
}
