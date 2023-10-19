using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class UniQuest : MonoBehaviour
{
    [Serializable]public class TriggerQuestCondition
    {
        public bool uponGameStart = false;
        public BLDExploreSpot uponDiscoverExploreSpot = null;
        public BLDExploreSpot uponUnlockExploreSpot = null;
        public BLDExploreSpot uponStartGatherExploreSpot = null;
        public ItemScriptableObject uponFirstObtainISO = null;
        public BuildingISO uponFirstBuildBISO = null;
    }
    
    

    [SerializeField]public TriggerQuestCondition triggerQuestCondition;
  
}
