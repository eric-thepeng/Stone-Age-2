using System.Collections;
using System.Collections.Generic;
using Live2D.Cubism.Framework.Json;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Used when a class can trigger UniAction Change.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IUniActionTrigger<T>
{
    UnityEvent<T> uniActionEventToTrigger { get; set; }

    /// <summary>
    /// Class action that triggers the UniAction with a index of type T.
    /// </summary>
    /// <param name="index"></param>
    public void TriggerUniAction(T index)
    {
        uniActionEventToTrigger?.Invoke(index);
    }

    /// <summary>
    /// UniAction action that initialize this interface.
    /// </summary>
    /// <param name="eventToTrigger">UniAction Event to be Triggered by this interface and class.</param>
    public void ActivateIUniActionTrigger(UnityAction<T> eventToTrigger)
    {
        uniActionEventToTrigger.AddListener(eventToTrigger);
    }
}

public interface IUniActionInteraction
{
    public void TriggerInteractionByUniAction(int index);
}
