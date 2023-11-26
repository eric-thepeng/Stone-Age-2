using System.Collections;
using System.Collections.Generic;
using Live2D.Cubism.Framework.Json;
using UnityEngine;
using UnityEngine.Events;

public interface IUniActionTrigger<T>
{
    UnityEvent<T> uniActionEventToTrigger { get; set; }

    public void TriggerUniAction(T index)
    {
        uniActionEventToTrigger?.Invoke(index);
    }

    public void ActivateIUniActionTrigger(UnityAction<T> eventToTrigger)
    {
        uniActionEventToTrigger.AddListener(eventToTrigger);
    }
}
