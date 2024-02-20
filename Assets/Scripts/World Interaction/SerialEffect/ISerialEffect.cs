using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerialEffect
{
    /// <summary>
    /// Must be called when action is done
    /// </summary>
    public void SendSerialEffect();
    
    /// <summary>
    /// Must be called at awake
    /// </summary>
    public void SetUpSerialEffectIdentifier();
    public void ReceiveSerialEffect(SO_SerialEffectIdentifier origionSEI);
    public SO_SerialEffectIdentifier mySEI { get;}
}
