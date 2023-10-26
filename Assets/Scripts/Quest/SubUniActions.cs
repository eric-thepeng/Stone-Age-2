using System;
using System.Collections;
using System.Collections.Generic;
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
    public enum ActionType{NoAction, TriggerQuest,LoadDescription}
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
        else if (actionType == ActionType.LoadDescription)
        {
            targetUniQuest.LoadDescription();
        }
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
        NoAction, WaitForPlayerStatsAchieve
    }
    public ActionType actionType = ActionType.NoAction;
    
    private UnityEvent _onActionStarts = new UnityEvent();
    private UnityEvent _onActionCompletes = new UnityEvent();
    public UnityEvent onActionStarts { get { return _onActionStarts; } }
    public UnityEvent onActionCompletes { get { return _onActionCompletes; } }

    
    public PlayerStatsMonitor.PlayerStatsType targetPlayerStats;
    [Header("Leave Blank if not a ISO related stat")]public ItemScriptableObject targetISO;
    public int targetAmount = 0;

    public void PerformAction()
    {
        onActionStarts?.Invoke();
        if (targetPlayerStats == PlayerStatsMonitor.PlayerStatsType.TrashTotalClear)
        {
            PlayerStatsMonitor.trashTotalClear.broadcastStatsChange.AddListener(CheckStatsReach);
        }else if (targetPlayerStats == PlayerStatsMonitor.PlayerStatsType.ISOTotalGain)
        {
            PlayerStatsMonitor.isoTotalGainPlayerStat.broadcastStatsChange.AddListener(CheckStatsReach);
        }else if (targetPlayerStats == PlayerStatsMonitor.PlayerStatsType.ISOTotalSpend)
        {
            PlayerStatsMonitor.isoTotalSpendPlayerStat.broadcastStatsChange.AddListener(CheckStatsReach);
        }else if (targetPlayerStats == PlayerStatsMonitor.PlayerStatsType.BISOBuild)
        {
            PlayerStatsMonitor.bisoTotalBuildPlayerStat.broadcastStatsChange.AddListener(CheckStatsReach);
        }
    }

    public void CheckStatsReach(ItemScriptableObject iso, int amount)
    {
        if(iso == targetISO && amount == targetAmount)     onActionCompletes?.Invoke();
    }

    public void CheckStatsReach(PlayerStatsMonitor.PlayerStatsType statType, int amount)
    {
        if(amount == targetAmount)     onActionCompletes?.Invoke();
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


