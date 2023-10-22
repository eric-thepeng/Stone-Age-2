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
    public UnityEvent onActionStarts { get; }
    public UnityEvent onActionCompletes { get; }

    private int currentActionIndex;
    
    public void PerformAction()
    {
        currentActionIndex = 0;
        onActionStarts?.Invoke();
        PerformActionOneByOne();
    }

    public void PerformActionOneByOne()
    {
        if (currentActionIndex >= allUniActions.Count)
        {
            onActionCompletes?.Invoke();
            return;
        }
        allUniActions[currentActionIndex].PerformAction();
        allUniActions[currentActionIndex].onActionCompletes.AddListener(PerformActionOneByOne);
        currentActionIndex++;
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
