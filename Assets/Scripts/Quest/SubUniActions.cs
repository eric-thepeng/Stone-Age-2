using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

/// <summary>
/// Need to override PerformAction() and IsAssigned() from IPerformableAction
/// </summary>
[Serializable]
public class SubUniAction : IPerformableAction
{
    protected UnityEvent _onActionStarts = new UnityEvent();
    protected UnityEvent _onActionCompletes = new UnityEvent();
    public virtual void PerformAction()
    {
        throw new NotImplementedException();
    }

    public virtual bool IsAssigned()
    {
        throw new NotImplementedException();
    }

    public UnityEvent onActionStarts { get { return _onActionStarts; } }
    public UnityEvent onActionCompletes { get { return _onActionCompletes; } }
    
    public string note = "leave a note for editor";
    public bool triggerAndQueNext = false;
}


[Serializable]public class GameObjectAction : SubUniAction
{
    public enum ActionType{NoAction, LocalMoveAmount, GlobalMoveAmount, LocalMoveTo, GlobalMoveTo, SetActive, SetInactive}
    public ActionType actionType = ActionType.NoAction;
    
    [Header("Assign targetGameObject OR enter setUpIdentifierID")]public GameObject targetGameObject = null;
    public string targetGameObjectSetUpIdentifierID;
    public Vector3 targetVector;
    public float moveTime = 1;

    public override void PerformAction()
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

    public override bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}

[Serializable]public class NarrativeSequenceAction : SubUniAction
{
    public enum ActionType{NoAction, PlayNarrativeSequence}
    public ActionType actionType = ActionType.NoAction;
    
    public NarrativeSequenceScriptableObject targetNSSO;

    public override void PerformAction()
    {
        if(actionType == ActionType.NoAction) Debug.LogError("NarrativeSequenceAction SubUniAction has action type NoAction");
        else
        {
            onActionStarts?.Invoke();
            DialogueManager.i.QueueNarrativeSequence(this);
        }
    }

    public override bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}

[Serializable]public class UniQuestAction : SubUniAction
{
    public enum ActionType{NoAction, TriggerQuest}
    public ActionType actionType = ActionType.NoAction;
    public UniQuest targetUniQuest = null;

    public override void PerformAction()
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

    public override bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}


[Serializable]public class PlayerStatAction : SubUniAction
{
    public enum ActionType {
        NoAction, StatsChangeAmount, StatsReachAmount
    }
    public ActionType actionType = ActionType.NoAction;

    public PlayerStatsMonitor.PlayerStatType targetPlayerStatType;
    [Header("Leave Blank if not a ISO related stat")]public ItemScriptableObject targetISO;
    public int targetAmount = 0;

    private int statsAmountAtActionStats = 0;

    private PlayerStat targetPlayerStat;

    public override void PerformAction()
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

    public override bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}


[Serializable] public class ButtonAction : SubUniAction
{
    public enum ActionType{NoAction,WaitForClickButton, SetActive, SetInactive}
    public ActionType actionType = ActionType.NoAction;

    [Header("Assign targetGameObject OR enter setUpIdentifierID")]public GameObject targetGameObject = null;
    public string targetGameObjectSetUpIdentifierID;


    public override void PerformAction()
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

    public override bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}

[Serializable] public class LevelUpAction : SubUniAction
{
    public enum ActionType{NoAction, WaitUntilUnlockLevel}
    public ActionType actionType = ActionType.NoAction;

    [Header("Assign targetGameObject OR enter setUpIdentifierID")]public LevelUp targetLevelUp = null;
    public string targetGameObjectSetUpIdentifierID;

    public int targetUnlockLevel = 0;

    public override void PerformAction()
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

    public override bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}

[Serializable] public class GamePanelAction : SubUniAction
{
    public enum ActionType{NoAction, GoToPanel}
    public ActionType actionType = ActionType.NoAction;

    public PlayerInputChannel.GamePanel targetPanel = PlayerInputChannel.GamePanel.Home;
    
    public override void PerformAction()
    {
        onActionStarts.Invoke();

        if (actionType == ActionType.GoToPanel)
        {
            PlayerInputChannel.GoToPanel(targetPanel);
            onActionCompletes.Invoke();
        }
    }

    public override bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}

[Serializable] public class CameraAction : SubUniAction
{
    public enum ActionType{NoAction, CameraAction}
    public ActionType actionType = ActionType.NoAction;

    public CameraManager.IUAIType targetAction = CameraManager.IUAIType.FreezeCamera;
    public int i;
    
    public override void PerformAction()
    {
        onActionStarts.Invoke();

        if (actionType != ActionType.NoAction)
        {
            CameraManager.i.TriggerInteractionByUniAction(targetAction);
            onActionCompletes.Invoke();
        }
    }

    public override bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}

[Serializable] public class IUniActionInteractionAction : SubUniAction
{
    public enum ActionType{NoAction, GeneralIUniActionInteractionAction}
    public ActionType actionType = ActionType.NoAction;

    [Header("Assign targetGameObject OR enter setUpIdentifierID")]public GameObject targetGameObject = null;
    public string targetGameObjectSetUpIdentifierID;

    public int actionIndex;


    public override void PerformAction()
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

    public override bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}

[Serializable] public class CharacterGatherAction : SubUniAction
{
    public enum ActionType{NoAction, UponGatherStarts, UponGatherEnds}
    public ActionType actionType = ActionType.NoAction;

    [Header("Leave as Null to indicate \" any. \"")]
    public SO_ExploreSpotSetUpInfo targetGatherSpot = null;
    public CharacterBasicStats targetCharacter = null;


    public override void PerformAction()
    {
       
        onActionStarts?.Invoke();

        if (actionType == ActionType.UponGatherStarts)
        {
            Character.CharacterGatherUnityEvent.AddListener(CheckFinish);
        }
        else if (actionType == ActionType.UponGatherEnds)
        {
            Character.CharacterGatherUnityEvent.AddListener(CheckFinish);
        }
        else
        {
            Debug.LogError("Character Gather Action is set as No Action");
        }
    }

    private void CheckFinish(SO_ExploreSpotSetUpInfo essui, CharacterBasicStats cbs, int i)
    {
        if(i == 0 && actionType == ActionType.UponGatherStarts) return;       
        if(i == 1 && actionType == ActionType.UponGatherEnds) return;

            
        bool correctExploreSpot = false;
        bool correctCharacter = false;

        //Check correct explore spot
        if (targetGatherSpot == null)
        {
            correctExploreSpot = true;
        }
        else if(targetGatherSpot == essui)
        {
            correctExploreSpot = true;
        }

        //Check correct character
        if (targetCharacter == null)
        {
            correctCharacter = true;
        }else if (targetCharacter == cbs)
        {
            correctCharacter = true;
        }

        //Trigger
        if (correctExploreSpot && correctCharacter)
        {
            onActionCompletes.Invoke();
            Character.CharacterGatherUnityEvent.RemoveListener(CheckFinish);
        }
    }

    public override bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}

[Serializable] public class BlueprintAction : SubUniAction
{
    public enum ActionType{NoAction, ObtainBlueprints}
    public ActionType actionType = ActionType.NoAction;

    public List<ItemCraftScriptableObject> blueprints = null;

    public override void PerformAction()
    {
        onActionStarts.Invoke();
        
        BlueprintManager.i.ObtainBlueprints(blueprints);
        
        onActionCompletes.Invoke();
    }

    public override bool IsAssigned()
    {
        return actionType != ActionType.NoAction && blueprints != null;
    }
}

[Serializable]public class TimelineAnimationAction : SubUniAction
{
    public enum ActionType{NoAction, StartTimeline, WaitForTimeline, StartAndWaitForTimeline, StartAnimation, WaitForAnimation, StartAndWaitForAnimation}
    public ActionType actionType = ActionType.NoAction;
    
    [Header("Assign targetGameObject OR enter setUpIdentifierID")]public GameObject targetGameObject = null;
    public string targetGameObjectSetUpIdentifierID;
    
    PlayableDirector myPD;
    Animator myAMT;

    public override void PerformAction()
    {
        // Routine
        
        if(actionType == ActionType.NoAction) Debug.LogError("TimelineAnimation Game SubUniAction has action type NoAction");

        if (targetGameObject == null)
            targetGameObject = GameObjectSetUpIdentifier.GetGameObjectByID(targetGameObjectSetUpIdentifierID);
        
        if(targetGameObject == null) Debug.LogError("TimelineAnimation can not find game object by id");
        
        onActionStarts.Invoke();
        
        // Perform According to Action Type
        
        if (TargetIsAnimator()) // ---- ANIMATOR ----
        {
            myAMT = targetGameObject.GetComponent<Animator>();
            if (actionType == ActionType.StartAnimation || actionType == ActionType.WaitForAnimation)
            {
                
            }



        }else if (TargetIsTimeline()) // ---- TIMELINE ----
        {
            myPD = targetGameObject.GetComponent<PlayableDirector>();

            if (actionType == ActionType.StartTimeline || actionType == ActionType.StartAndWaitForTimeline)
            {
                myPD.Play();
                if(actionType == ActionType.StartTimeline) FinishUniAction();
            }

            if (actionType == ActionType.WaitForTimeline || actionType == ActionType.StartAndWaitForTimeline)
            {
                myPD.stopped += OnPlayableDirectorStopped;
            }
        }
        
        

        /*
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
        }*/
    }

    void OnPlayableDirectorStopped(PlayableDirector myPlayableDirector)
    {
        if(myPlayableDirector == myPD) FinishUniAction();
    }
    
    public void FinishUniAction()
    {
        onActionCompletes.Invoke();
    }

    private bool TargetIsAnimator()
    {
        return actionType == ActionType.StartAnimation || actionType == ActionType.WaitForAnimation ||
               actionType == ActionType.StartAndWaitForAnimation;
    }

    private bool TargetIsTimeline()
    {
        return actionType == ActionType.StartTimeline || actionType == ActionType.WaitForTimeline ||
               actionType == ActionType.StartAndWaitForTimeline;
    }

    public override bool IsAssigned()
    {
        return actionType != ActionType.NoAction;
    }
}