using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerialEffect
{
    public void SendSerialEffect();
    public void SetUpSerialEffectIdentifier();
    public void ReceiveSerialEffect(SO_SerialEffectIdentifier origionSEI);
    public SO_SerialEffectIdentifier mySEI { get;}
}
