using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Cinemachine;
using UnityEngine;

public class UniQuest : MonoBehaviour
{
    #region SubClasses

    [Serializable]public class TriggerQuestCondition
    {
        [Header("DO NOT EDIT")]
        public bool hasBeenTrigged = false;
        [Header("Edit Conditions")]
        public bool uponGameStart = false;
        public bool duringTutorial = false;
        public BLDExploreSpot uponDiscoverExploreSpot = null;
        public BLDExploreSpot uponUnlockExploreSpot = null;
        public BLDExploreSpot uponStartGatherExploreSpot = null;
        public ItemScriptableObject uponObtainISO = null;
        public int uponObtainISOAmount = 0;
        public BuildingISO uponBuildBISO = null;
        public int uponBuildBISOAmount = 0;

        private bool triggeredByUniAction = false;

        public bool isTriggered()
        {
            if (hasBeenTrigged) return false;
            //determin if is has been triggered, remember to change hasBeenTriggered.
            return false;
        }

        public void TriggerByUniAction()
        {
            triggeredByUniAction = true;
        }
    }
    
    #endregion

    [SerializeField]public TriggerQuestCondition triggerQuestCondition;
    [SerializeField]public UniActionSequence beginQuestUniActionSequence;
    [SerializeField]public UniActionSequence endQuestUniActionSequence;
    
    private void Start()
    {
        if(triggerQuestCondition.uponGameStart) beginQuestUniActionSequence.PerformAction();
    }

    public void QueQuest(){}
    public void LoadDescription(){}
}
