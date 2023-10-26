using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]public class UniAction : IPerformableAction
{
    [SerializeField, Header("Delay an amount of time when executing action.")] private float delayTime;
    
    public GameObjectAction gameObjectAction;
    public NarrativeSequenceAction narrativeSequenceAction;
    public UniQuestAction uniQuestAction;
    public WaitForPlayerStatsAchieveAction waitForPlayerStatsAchieveAction;
    public ButtonAction buttonAction;
    
    private UnityEvent _onActionStarts = new UnityEvent();
    private UnityEvent _onActionCompletes = new UnityEvent();
    public UnityEvent onActionStarts { get { return _onActionStarts; } }
    public UnityEvent onActionCompletes { get { return _onActionCompletes; } }


    public void PerformAction()
    {
        IPerformableAction[] allIUniActions = {gameObjectAction,  narrativeSequenceAction, uniQuestAction, waitForPlayerStatsAchieveAction, buttonAction};
        int assignCount = 0;
        foreach (IPerformableAction pAction in allIUniActions)
        {
            if (pAction.IsAssigned()) assignCount++;
        }
        if(assignCount == 0) Debug.LogError("UniAction is not assigned any action");
        else if (assignCount != 1)
        {
            string debugLog = " ";
            int i = 0;
            foreach (IPerformableAction pAction in allIUniActions)
            {
                if (pAction.IsAssigned())
                {
                    assignCount++;
                    debugLog += i + " ";
                }
                i++;
            }
            
            Debug.LogError("UniAction need to be assigned only 1 action, "+debugLog);
        }
        foreach (IPerformableAction pAction in allIUniActions)
        {
            if (pAction.IsAssigned())
            {
                onActionStarts?.Invoke();
                DialogueManager.i.StartCoroutine(DelayAndPerformAction(pAction));
                //DelayAndPerformAction(uniAction);
                return;
            }
        }
    }

    public bool IsAssigned()
    {
        return gameObjectAction.IsAssigned() || narrativeSequenceAction.IsAssigned();
    }
    
    IEnumerator DelayAndPerformAction(IPerformableAction uniActionToPerform)
    {
        float timeCount = 0;
        while (timeCount<delayTime)
        {
            timeCount += Time.deltaTime;
            yield return new WaitForSeconds(0);
        }
        uniActionToPerform.onActionCompletes.AddListener(onActionCompletes.Invoke);
        uniActionToPerform.PerformAction();
    }
    
    
}
