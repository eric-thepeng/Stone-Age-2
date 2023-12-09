using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;


[Serializable]public class GameObjectAction : IPerformableAction
{
    public enum ActionType{NoAction, LocalMoveAmount, GlobalMoveAmount, LocalMoveTo, GlobalMoveTo, SetActive, SetInactive}
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
        if(actionType == ActionType.NoAction) Debug.LogError("GameObjectAction SubUniAction has action type NoAction");

        onActionStarts.Invoke();

        if (targetGameObject == null)
            targetGameObject = GameObjectSetUpIdentifier.GetGameObjectByID(targetGameObjectSetUpIdentifierID);
        
        Vector3 finalGlobalPosition = targetGameObject.transform.position + targetVector;
        Vector3 finalLocalPosition = targetGameObject.transform.localPosition + targetVector;

        if (actionType == ActionType.LocalMoveAmount) targetGameObject.transform.DOLocalMove(finalLocalPosition, moveTime).onComplete = onActionCompletes.Invoke;
        else if (actionType == ActionType.GlobalMoveAmount) targetGameObject.transform.DOMove(finalGlobalPosition, moveTime).onComplete = onActionCompletes.Invoke;
        
        else if (actionType == ActionType.LocalMoveTo) targetGameObject.transform.DOLocalMove(targetVector, moveTime).onComplete = onActionCompletes.Invoke;
        else if (actionType == ActionType.GlobalMoveTo) targetGameObject.transform.DOMove(targetVector, moveTime).onComplete = onActionCompletes.Invoke;
        
        else if (actionType == ActionType.SetActive)
        {
            targetGameObject.SetActive(true); 
            onActionCompletes.Invoke();
        }
        else if (actionType == ActionType.SetInactive)
        {
            targetGameObject.SetActive(false); 
            onActionCompletes.Invoke();
        }
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
        if(actionType == ActionType.NoAction) Debug.LogError("NarrativeSequenceAction SubUniAction has action type NoAction");
        else
        {
            onActionStarts?.Invoke();
            DialogueManager.i.QueueNarrativeSequence(this);
        }
    }

    public bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}

[Serializable]public class UniQuestAction : IPerformableAction
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


[Serializable]public class PlayerStatAction : IPerformableAction
{
    public enum ActionType {
        NoAction, StatsChangeAmount, StatsReachAmount
    }
    public ActionType actionType = ActionType.NoAction;
    
    private UnityEvent _onActionStarts = new UnityEvent();
    private UnityEvent _onActionCompletes = new UnityEvent();
    public UnityEvent onActionStarts { get { return _onActionStarts; } }
    public UnityEvent onActionCompletes { get { return _onActionCompletes; } }

    public PlayerStatsMonitor.PlayerStatType targetPlayerStatType;
    [Header("Leave Blank if not a ISO related stat")]public ItemScriptableObject targetISO;
    public int targetAmount = 0;

    private int statsAmountAtActionStats = 0;

    private PlayerStat targetPlayerStat;

    public void PerformAction()
    {
        onActionStarts?.Invoke();
        targetPlayerStat = PlayerStatsMonitor.GetPlayerStat(targetPlayerStatType, targetISO);
        targetPlayerStat.SubscribeStatChange(CheckStatsReach);
        if (actionType == ActionType.StatsChangeAmount)
            statsAmountAtActionStats = targetPlayerStat.GetAmount();
    }
    
    public void CheckStatsReach(int amount)
    {
        if (actionType == ActionType.StatsReachAmount)
        {
            if (amount >= targetAmount)
            {
                onActionCompletes?.Invoke();
                targetPlayerStat.UnsubscribeStatChange(CheckStatsReach);
            }
        }else if (actionType == ActionType.StatsChangeAmount)
        {
            if ((amount - statsAmountAtActionStats) >= targetAmount)
            {
                onActionCompletes?.Invoke();
                targetPlayerStat.UnsubscribeStatChange(CheckStatsReach);
            }
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
        if(wsb == null) Debug.LogError("Cannot find WorldSpaceButton from GameObjectSetUpIdentifier: " + targetGameObjectSetUpIdentifierID);
        
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
        if(actionType == ActionType.NoAction) Debug.LogError("LevelUp SubUniAction has action type NoAction");
        
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

[Serializable] public class CharacterGatherAction : IPerformableAction
{
    public enum ActionType{NoAction, WaitUntilCharacterGatherLocation}
    public ActionType actionType = ActionType.NoAction;

    private UnityEvent _onActionStarts = new UnityEvent();
    private UnityEvent _onActionCompletes = new UnityEvent();
    public UnityEvent onActionStarts { get { return _onActionStarts; } }
    public UnityEvent onActionCompletes { get { return _onActionCompletes; } }
    
    [Header("Assign targetGameObject OR enter setUpIdentifierID")]public GatherSpot targetGatherSpot = null;
    public string targetGameObjectSetUpIdentifierID;
    
    public void PerformAction()
    {
        if(actionType == ActionType.NoAction) Debug.LogError("SubUniAction has action type NoAction");
        
        if (targetGatherSpot == null)
            targetGatherSpot = GameObjectSetUpIdentifier.GetGameObjectByID(targetGameObjectSetUpIdentifierID).GetComponent<GatherSpot>();
        
        if(targetGatherSpot == null) Debug.LogError("Cannot find GatherSpot");
        
        onActionStarts?.Invoke();

        if (actionType == ActionType.WaitUntilCharacterGatherLocation)
        {
            //((IUniActionTrigger<int>)targetLevelUp).ActivateIUniActionTrigger(CheckUniActionComplete);
        }
    }

    public void CheckUniActionComplete()
    {
        
    }

    public bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}

[Serializable] public class GamePanelAction : IPerformableAction
{
    public enum ActionType{NoAction, GoToPanel}
    public ActionType actionType = ActionType.NoAction;

    private UnityEvent _onActionStarts = new UnityEvent();
    private UnityEvent _onActionCompletes = new UnityEvent();
    public UnityEvent onActionStarts { get { return _onActionStarts; } }
    public UnityEvent onActionCompletes { get { return _onActionCompletes; } }

    public PlayerInputChannel.GamePanel targetPanel = PlayerInputChannel.GamePanel.Home;
    
    public void PerformAction()
    {
        onActionStarts.Invoke();

        if (actionType == ActionType.GoToPanel)
        {
            PlayerInputChannel.GoToPanel(targetPanel);
            onActionCompletes.Invoke();
        }
    }

    public bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}

[Serializable] public class CameraAction : IPerformableAction
{
    public enum ActionType{NoAction, CameraAction}
    public ActionType actionType = ActionType.NoAction;

    private UnityEvent _onActionStarts = new UnityEvent();
    private UnityEvent _onActionCompletes = new UnityEvent();
    public UnityEvent onActionStarts { get { return _onActionStarts; } }
    public UnityEvent onActionCompletes { get { return _onActionCompletes; } }

    public CameraManager.IUAIType targetAction = CameraManager.IUAIType.FreezeCamera;
    public int i;
    
    public void PerformAction()
    {
        onActionStarts.Invoke();

        if (actionType != ActionType.NoAction)
        {
            CameraManager.i.TriggerInteractionByUniAction(targetAction);
            onActionCompletes.Invoke();
        }
    }

    public bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}

[Serializable] public class IUniActionInteractionAction : IPerformableAction
{
    public enum ActionType{NoAction, GeneralIUniActionInteractionAction}
    public ActionType actionType = ActionType.NoAction;

    private UnityEvent _onActionStarts = new UnityEvent();
    private UnityEvent _onActionCompletes = new UnityEvent();
    public UnityEvent onActionStarts { get { return _onActionStarts; } }
    public UnityEvent onActionCompletes { get { return _onActionCompletes; } }
    
    [Header("Assign targetGameObject OR enter setUpIdentifierID")]public GameObject targetGameObject = null;
    public string targetGameObjectSetUpIdentifierID;

    public int actionIndex;


    public void PerformAction()
    {
        if (targetGameObject == null)
            targetGameObject = GameObjectSetUpIdentifier.GetGameObjectByID(targetGameObjectSetUpIdentifierID);
        IUniActionInteraction iuai = targetGameObject.GetComponent<IUniActionInteraction>();
        if(iuai == null) Debug.LogError("Cannot find WorldSpaceButton from GameObjectSetUpIdentifier: " + targetGameObjectSetUpIdentifierID);
        
        onActionStarts?.Invoke();

        if (actionType != ActionType.NoAction)
        {
            iuai.TriggerInteractionByUniAction(actionIndex);
            onActionCompletes.Invoke();
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