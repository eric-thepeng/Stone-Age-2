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
        GamePanelAction,
        CameraAction,
        IUniActionInteractionAction,
        CharacterGatherAction,
        BlueprintAction,
        TimelineAnimationAction,
        CharacterInteractionAction,
        ScreenNotificationAction,
        CharacterStatAction,
    }

    public UniActionType uniActionType = UniActionType.NoAction;
    
    public GameObjectAction gameObjectAction;
    public NarrativeSequenceAction narrativeSequenceAction;
    public UniQuestAction uniQuestAction;
    public PlayerStatAction playerStatAction;
    public ButtonAction buttonAction;
    public LevelUpAction levelUpAction;
    public GamePanelAction gamePanelAction;
    public CameraAction cameraAction;
    public IUniActionInteractionAction iuaiAction;
    public CharacterGatherAction characterGatherAction;
    public BlueprintAction blueprintAction;
    public TimelineAnimationAction timelineAnimationAction;
    public CharacterInteractionAction characterInteractionAction;
    public ScreenNotificationAction screenNotificationAction;
    public CharacterStatAction characterStatAction;

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
            case UniActionType.CameraAction:
                pAction = cameraAction;
                break;
            case UniActionType.IUniActionInteractionAction:
                pAction = iuaiAction;
                break;
            case UniActionType.CharacterGatherAction:
                pAction = characterGatherAction;
                break;
            case UniActionType.BlueprintAction:
                pAction = blueprintAction;
                break;
            case UniActionType.TimelineAnimationAction:
                pAction = timelineAnimationAction;
                break;
            case UniActionType.CharacterInteractionAction:
                pAction = characterInteractionAction;
                break;
            case UniActionType.ScreenNotificationAction:
                pAction = screenNotificationAction;
                break;
            case UniActionType.CharacterStatAction:
                pAction = characterStatAction;
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
