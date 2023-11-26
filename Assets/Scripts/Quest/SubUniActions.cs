using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;


[Serializable]public class GameObjectAction : IPerformableAction
{
    public enum ActionType{NoAction, LocalMoveAmount, GlobalMoveAmount, LocalMoveTo, GlobalMoveTo, Enable, Disable}
    public ActionType actionType = ActionType.NoAction;
    
    [Header("Assign targetGameObject OR enter setUpIdentifierID")]public GameObject targetGameObject = null;
    public string targetGameObjectSetUpIdentifierID;
    public Vector3 targetVector;
    public float moveTime = 1;
    
    private UnityEvent _onActionStarts = new UnityEvent();
    private UnityEvent _onActionCompletes = new UnityEvent();
    public UnityEvent onActionStarts { get { return _onActionStarts; } }
    public UnityEvent onActionCompletes { get { return _onActionCompletes; } }
    
    public void PerformAction()
    {
        onActionStarts.Invoke();

        if (targetGameObject == null)
            targetGameObject = GameObjectSetUpIdentifier.GetGameObjectByID(targetGameObjectSetUpIdentifierID);
        
        Vector3 finalGlobalPosition = targetGameObject.transform.position + targetVector;
        Vector3 finalLocalPosition = targetGameObject.transform.localPosition + targetVector;

        if (actionType == ActionType.LocalMoveAmount) targetGameObject.transform.DOLocalMove(finalLocalPosition, moveTime).onComplete = onActionCompletes.Invoke;
        else if (actionType == ActionType.GlobalMoveAmount) targetGameObject.transform.DOMove(finalGlobalPosition, moveTime).onComplete = onActionCompletes.Invoke;
        
        else if (actionType == ActionType.LocalMoveTo) targetGameObject.transform.DOLocalMove(targetVector, moveTime).onComplete = onActionCompletes.Invoke;
        else if (actionType == ActionType.GlobalMoveTo) targetGameObject.transform.DOMove(targetVector, moveTime).onComplete = onActionCompletes.Invoke;
        
        else if (actionType == ActionType.Enable) {targetGameObject.SetActive(true); onActionCompletes.Invoke();}
        else if (actionType == ActionType.Disable) {targetGameObject.SetActive(false); onActionCompletes.Invoke();}

    }

    public bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}

[Serializable]public class NarrativeSequenceAction : IPerformableAction
{
    public enum ActionType{NoAction, PlayNarrativeSequence}
    public ActionType actionType = ActionType.NoAction;
    
    public NarrativeSequence narrativeSequenceToPlay;
    private UnityEvent _onActionStarts = new UnityEvent();
    private UnityEvent _onActionCompletes = new UnityEvent();
    public UnityEvent onActionStarts { get { return _onActionStarts; } }
    public UnityEvent onActionCompletes { get { return _onActionCompletes; } }

    public void PerformAction()
    {
        onActionStarts?.Invoke();
        DialogueManager.i.QueueNarrativeSequence(this);
    }

    public bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}

[Serializable] public class UniQuestAction : IPerformableAction
{
    public enum ActionType{NoAction, TriggerQuest}
    public ActionType actionType = ActionType.NoAction;
    public UniQuest targetUniQuest = null;
    
    private UnityEvent _onActionStarts = new UnityEvent();
    private UnityEvent _onActionCompletes = new UnityEvent();
    public UnityEvent onActionStarts { get { return _onActionStarts; } }
    public UnityEvent onActionCompletes { get { return _onActionCompletes; } }
    
    public void PerformAction()
    {
        onActionStarts?.Invoke();
        if (actionType == ActionType.TriggerQuest)
        {
            targetUniQuest.QueQuest();
        }
        /*
        else if (actionType == ActionType.LoadDescription)
        {
            targetUniQuest.LoadDescription();
        }*/
        onActionCompletes?.Invoke();
    }

    public bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}


[Serializable]public class WaitForPlayerStatsAchieveAction : IPerformableAction
{
    public enum ActionType {
        NoAction, StatsChangeAmount, StatsReachAmount
    }
    public ActionType actionType = ActionType.NoAction;
    
    private UnityEvent _onActionStarts = new UnityEvent();
    private UnityEvent _onActionCompletes = new UnityEvent();
    public UnityEvent onActionStarts { get { return _onActionStarts; } }
    public UnityEvent onActionCompletes { get { return _onActionCompletes; } }

    public PlayerStatsMonitor.PlayerStatsType targetPlayerStats;
    [Header("Leave Blank if not a ISO related stat")]public ItemScriptableObject targetISO;
    public int targetAmount = 0;

    private int statsAmountAtActionStats = 0;

    public void PerformAction()
    {
        onActionStarts?.Invoke();
        if (targetPlayerStats == PlayerStatsMonitor.PlayerStatsType.TrashTotalClear)
        {
            PlayerStatsMonitor.trashTotalClear.broadcastStatsChange.AddListener(CheckStatsReach);
            if (actionType == ActionType.StatsChangeAmount)
                statsAmountAtActionStats = PlayerStatsMonitor.trashTotalClear.GetCurrentStats(PlayerStatsMonitor.PlayerStatsType.TrashTotalClear);
        }else if (targetPlayerStats == PlayerStatsMonitor.PlayerStatsType.ISOTotalGain)
        {
            PlayerStatsMonitor.isoTotalGainPlayerStat.broadcastStatsChange.AddListener(CheckStatsReach);
            if (actionType == ActionType.StatsChangeAmount)
                statsAmountAtActionStats = PlayerStatsMonitor.isoTotalGainPlayerStat.GetCurrentStats(targetISO);
        }/*else if (targetPlayerStats == PlayerStatsMonitor.PlayerStatsType.ISOTotalSpend)
        {
            PlayerStatsMonitor.isoTotalSpendPlayerStat.broadcastStatsChange.AddListener(CheckStatsReach);
            if (actionType == ActionType.StatsChangeAmount)
                statsAmountAtActionStats = PlayerStatsMonitor.isoTotalSpendPlayerStat.GetCurrentStats(targetISO);
        }*/else if (targetPlayerStats == PlayerStatsMonitor.PlayerStatsType.BISOBuild)
        {
            if (!(targetISO is BuildingISO))
            {
                Debug.LogError("The assigned ISO for <Player Stats Reach - BISOBuildAmount> UniAction is not a BISO.");
            }
            PlayerStatsMonitor.bisoTotalBuildPlayerStat.broadcastStatsChange.AddListener(CheckStatsReach);
            if (actionType == ActionType.StatsChangeAmount)
                statsAmountAtActionStats = PlayerStatsMonitor.bisoTotalBuildPlayerStat.GetCurrentStats((BuildingISO)targetISO);
        } 
    }

    /// <summary>
    /// Being called by PlayerStatsMonitor of each stats when stats change occurs
    /// </summary>
    /// <param name="iso">ISO to track</param>
    /// <param name="amount">The new amount after stats change</param>
    public void CheckStatsReach(ItemScriptableObject iso, int amount) 
    {
        if(iso!= targetISO) return;
        if (actionType == ActionType.StatsReachAmount)
        {
            if(amount >= targetAmount) onActionCompletes?.Invoke();
        }else if (actionType == ActionType.StatsChangeAmount)
        {
            if((amount-statsAmountAtActionStats)>=targetAmount) onActionCompletes?.Invoke();
        }
    }

    /// <summary>
    /// Being called by PlayerStatsMonitor of each stats when stats change occurs
    /// </summary>
    /// <param name="iso">Non-ISO related PlayerStatsType to track</param>
    /// <param name="amount">The new amount after stats change</param>
    public void CheckStatsReach(PlayerStatsMonitor.PlayerStatsType statType, int amount)
    {
        if(statType != targetPlayerStats) return;
        if (actionType == ActionType.StatsReachAmount)
        {
            if(amount >= targetAmount) onActionCompletes?.Invoke();
        }else if (actionType == ActionType.StatsChangeAmount)
        {
            if((amount-statsAmountAtActionStats)>=targetAmount) onActionCompletes?.Invoke();
        }
    }

    public bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}


[Serializable] public class ButtonAction : IPerformableAction
{
    public enum ActionType{NoAction,WaitForClickButton, SetActive, SetInactive}
    public ActionType actionType = ActionType.NoAction;

    private UnityEvent _onActionStarts = new UnityEvent();
    private UnityEvent _onActionCompletes = new UnityEvent();
    public UnityEvent onActionStarts { get { return _onActionStarts; } }
    public UnityEvent onActionCompletes { get { return _onActionCompletes; } }
    
    [Header("Assign targetGameObject OR enter setUpIdentifierID")]public GameObject targetGameObject = null;
    public string targetGameObjectSetUpIdentifierID;


    public void PerformAction()
    {
        if (targetGameObject == null)
            targetGameObject = GameObjectSetUpIdentifier.GetGameObjectByID(targetGameObjectSetUpIdentifierID);
        WorldSpaceButton wsb = targetGameObject.GetComponent<WorldSpaceButton>();
        onActionStarts?.Invoke();

        if (actionType == ActionType.SetActive)
        {
            wsb.SetButtonActive(true);
            onActionCompletes.Invoke();
        }
        else if (actionType == ActionType.SetInactive)
        {
            wsb.SetButtonActive(false);
            onActionCompletes.Invoke();
        }else if (actionType == ActionType.WaitForClickButton)
        {
            wsb.onActionCompletes.AddListener(FinishClick);
        }
    }

    private void FinishClick()
    {
        onActionCompletes.Invoke();
    }

    public bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}

[Serializable] public class LevelUpAction : IPerformableAction
{
    public enum ActionType{NoAction, WaitUntilUnlockLevel}
    public ActionType actionType = ActionType.NoAction;

    private UnityEvent _onActionStarts = new UnityEvent();
    private UnityEvent _onActionCompletes = new UnityEvent();
    public UnityEvent onActionStarts { get { return _onActionStarts; } }
    public UnityEvent onActionCompletes { get { return _onActionCompletes; } }
    
    [Header("Assign targetGameObject OR enter setUpIdentifierID")]public LevelUp targetLevelUp = null;
    public string targetGameObjectSetUpIdentifierID;

    public int targetUnlockLevel = 0;

    public void PerformAction()
    {
        if(actionType == ActionType.NoAction) Debug.LogError("SubUniAction has action type NoAction");
        
        if (targetLevelUp == null)
            targetLevelUp = GameObjectSetUpIdentifier.GetGameObjectByID(targetGameObjectSetUpIdentifierID).GetComponent<LevelUp>();
        
        if(targetLevelUp == null) Debug.LogError("Cannot find LevelUp");
        
        onActionStarts?.Invoke();

        if (actionType == ActionType.WaitUntilUnlockLevel)
        {
            ((IUniActionTrigger<int>)targetLevelUp).ActivateIUniActionTrigger(CheckUniActionComplete);
        }
    }

    public void CheckUniActionComplete(int unlockLevel)
    {
        if (unlockLevel >= targetUnlockLevel)
        {
            onActionCompletes.Invoke();
        }
    }

    public bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}



