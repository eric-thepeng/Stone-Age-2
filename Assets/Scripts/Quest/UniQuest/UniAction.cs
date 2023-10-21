using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]public class UniAction
{
    #region SubActions

    

    #endregion
    public interface IUniAction
    {
        public void PerformAction();
        public bool IsAssigned();
    }

    [Serializable]public class GameObjectAction : IUniAction
    {
        public enum ActionType{NoAction, LocalMoveAmount, GlobalMoveAmount, Enable, Disable}
        public ActionType actionType = ActionType.NoAction;
        public GameObject targetGameObject;
        public Vector3 targetVector;

        public void PerformAction()
        {
            if (actionType == ActionType.LocalMoveAmount) targetGameObject.transform.localPosition += targetVector;
            else if (actionType == ActionType.LocalMoveAmount) targetGameObject.transform.position += targetVector;
            else if (actionType == ActionType.Enable) targetGameObject.SetActive(true);
            else if (actionType == ActionType.Disable) targetGameObject.SetActive(false);
        }

        public bool IsAssigned()
        {
            return actionType != ActionType.NoAction;
        }
    }
    
    [Serializable]public class NarrativeSequenceAction : IUniAction
    {
        public NarrativeSequence narrativeSequenceToPlay;

        public void PerformAction()
        {
            DialogueManager.i.QueueNarrativeSequence(narrativeSequenceToPlay);
        }

        public bool IsAssigned()
        {
            return narrativeSequenceToPlay != null;
        }
    }

    [Serializable]
    public class UniQuestAction : IUniAction
    {
        public UniQuest targetUniQuest;
        public enum ActionType{Trigger}
        public ActionType actionType = ActionType.Trigger;
        
        public void PerformAction()
        {
            
        }

        public bool IsAssigned()
        {
            return false;
        }
    }

    [SerializeField, Header("Delay an amount of time when executing action.")] private float delayTime;
    public GameObjectAction gameObjectAction;
    public NarrativeSequenceAction narrativeSequenceAction;
    
    public void PerformAction()
    {
        IUniAction[] allIUniActions = {gameObjectAction,  narrativeSequenceAction};
        int assignCount = 0;
        foreach (IUniAction uniAction in allIUniActions)
        {
            if (uniAction.IsAssigned()) assignCount++;
        }
        if(assignCount == 0) Debug.LogError("UniAction is not assigned any action");
        else if(assignCount != 1) Debug.LogError("UniAction need to be assigned only 1 action");
        foreach (IUniAction uniAction in allIUniActions)
        {
            if (uniAction.IsAssigned())
            {
                DialogueManager.i.StartCoroutine(DelayAndPerformAction(uniAction));
                //DelayAndPerformAction(uniAction);
                return;
            }
        }
    }

    IEnumerator DelayAndPerformAction(IUniAction uniActionToPerform)
    {
        float timeCount = 0;
        while (timeCount<delayTime)
        {
            timeCount += Time.deltaTime;
            yield return new WaitForSeconds(0);
        }
        uniActionToPerform.PerformAction();
    }
    
    
}
