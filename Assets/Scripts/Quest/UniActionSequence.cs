using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IPerformableAction
{
    public void PerformAction();
    public bool IsAssigned();
    public UnityEvent onActionStarts { get;}
    public UnityEvent onActionCompletes { get;}

}

[Serializable]public class UniActionSequence : IPerformableAction
{
    public List<UniAction> allUniActions;
    private UnityEvent _onActionStarts = new UnityEvent();
    private UnityEvent _onActionCompletes = new UnityEvent();
    private UniQuest myUniQuest = null;
    
    public UnityEvent onActionStarts
    {
        get { return _onActionStarts; }
    }
    public UnityEvent onActionCompletes
    {
        get { return _onActionCompletes; }
    }
    
    public void SetUp(UniQuest tarUniQuest)
    {
        myUniQuest = tarUniQuest;
    }

    private int currentActionIndex;
    
    public void PerformAction()
    {
        currentActionIndex = 0;
        onActionStarts?.Invoke();
        PerformActionOneByOne();
    }

    public void PerformActionOneByOne()
    {
        if (currentActionIndex >= allUniActions.Count) //When Entire UniActionSequence Complete
        {
            onActionCompletes?.Invoke();
            myUniQuest.CompleteQuest();
            return;
        }
        
        currentActionIndex++;
        myUniQuest.currentUniAction = currentActionIndex;
        allUniActions[currentActionIndex-1].onActionCompletes.AddListener(PerformActionOneByOne);
        allUniActions[currentActionIndex-1].PerformAction(); //This will trigger onActionCompletes and cause recursion into PerformActionOneByOne
    }
    

    public bool IsAssigned()
    {
        foreach (var uniAction in allUniActions)
        {
            if (!uniAction.IsAssigned()) return false;
        }

        return true;
    }
}
