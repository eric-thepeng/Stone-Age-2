using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class UniQuest : MonoBehaviour
{
    #region SubClasses

    [Serializable]public class TriggerQuestCondition
    {
        public bool uponGameStart = false;
        public BLDExploreSpot uponDiscoverExploreSpot = null;
        public BLDExploreSpot uponUnlockExploreSpot = null;
        public BLDExploreSpot uponStartGatherExploreSpot = null;
        public ItemScriptableObject uponFirstObtainISO = null;
        public BuildingISO uponFirstBuildBISO = null;

        public bool isTriggered()
        {
            return false;
        }
    }
    
    [Serializable]
    public class QuestProgressAction
    {
        public virtual void BeginAction(){}
    }
    
    [Serializable] public class NarrativeDialogue : QuestProgressAction
    {
        public List<string> allLines;
        public int currentLineIndex = 0;

        public override void BeginAction()
        {
            base.BeginAction();
        }

        public string GetCurrentLine()
        {
            return allLines[currentLineIndex];
        }

        public bool AdvanceLine()
        {
            currentLineIndex += 1;
            return currentLineIndex < allLines.Count;
        }
    }

    #endregion


    [SerializeField]public TriggerQuestCondition triggerQuestCondition;
    
}
