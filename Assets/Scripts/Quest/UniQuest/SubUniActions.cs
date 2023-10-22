using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable]public class GameObjectAction : IPerformableAction
{
    public enum ActionType{NoAction, LocalMoveAmount, GlobalMoveAmount, Enable, Disable}
    public ActionType actionType = ActionType.NoAction;
    public GameObject targetGameObject;
    public Vector3 targetVector;
    private UnityEvent _onActionStarts = new UnityEvent();
    private UnityEvent _onActionCompletes = new UnityEvent();
    public UnityEvent onActionStarts { get { return _onActionStarts; } }
    public UnityEvent onActionCompletes { get { return _onActionCompletes; } }
    public void PerformAction()
    {
        onActionStarts.Invoke();
        if (actionType == ActionType.LocalMoveAmount) targetGameObject.transform.localPosition += targetVector;
        else if (actionType == ActionType.GlobalMoveAmount) targetGameObject.transform.position += targetVector;
        else if (actionType == ActionType.Enable) targetGameObject.SetActive(true);
        else if (actionType == ActionType.Disable) targetGameObject.SetActive(false);
        onActionCompletes.Invoke();
    }

    public bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}

[Serializable]public class NarrativeSequenceAction : IPerformableAction
{
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
        return narrativeSequenceToPlay != null;
    }
}

[Serializable] public class UniQuestAction : IPerformableAction
{
    public UniQuest targetUniQuest = null;
    public enum ActionType{TriggerQuest,LoadDescription}
    public ActionType actionType = ActionType.TriggerQuest;
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
        return targetUniQuest != null;
    }
}
