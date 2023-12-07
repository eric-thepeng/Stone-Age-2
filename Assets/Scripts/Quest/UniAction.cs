using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]public class UniAction : IPerformableAction
{
    [SerializeField, Header("Delay an amount of time when executing action.")] private float delayTime;

    public enum UniActionType
    {
        NoAction,
        GameObjectAction,
        NarrativeSequenceAction,
        UniQuestAction,
        PlayerStatAction,
        ButtonAction,
        LevelUpAction,
        GamePanelAction
    }

    public UniActionType uniActionType = UniActionType.NoAction;
    
    public GameObjectAction gameObjectAction;
    public NarrativeSequenceAction narrativeSequenceAction;
    public UniQuestAction uniQuestAction;
    public PlayerStatAction playerStatAction;
    public ButtonAction buttonAction;
    public LevelUpAction levelUpAction;
    public GamePanelAction gamePanelAction;
    
    private UnityEvent _onActionStarts = new UnityEvent();
    private UnityEvent _onActionCompletes = new UnityEvent();
    public UnityEvent onActionStarts { get { return _onActionStarts; } }
    public UnityEvent onActionCompletes { get { return _onActionCompletes; } }


    public void PerformAction()
    {
        IPerformableAction pAction = null;

        switch (uniActionType)
        {
            case UniActionType.NoAction:
                Debug.LogError("UniAction type is set to NoAction");
                return;
            case UniActionType.GameObjectAction:
                pAction = gameObjectAction;
                break;
            case UniActionType.NarrativeSequenceAction: 
                pAction = narrativeSequenceAction;
                break;
            case UniActionType.UniQuestAction: 
                pAction = uniQuestAction;
                break;
            case UniActionType.PlayerStatAction: 
                pAction = playerStatAction;
                break;
            case UniActionType.ButtonAction: 
                pAction = buttonAction;
                break;
            case UniActionType.LevelUpAction: 
                pAction = levelUpAction;
                break;
            case UniActionType.GamePanelAction:
                pAction = gamePanelAction;
                break;
        }
        
        if (pAction.IsAssigned())
        {
            onActionStarts?.Invoke();
            DialogueManager.i.StartCoroutine(DelayAndPerformAction(pAction));
        }
        else
        {
            Debug.LogError("SubUniAction is not assigned.");
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
