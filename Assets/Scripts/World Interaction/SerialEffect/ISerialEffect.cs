using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerialEffect
{
    public void SendSerialEffect()
    {
        mySEI.SendSerialEffect();
    }
    public void ReceiveSerialEffect();
    public SO_SerialEffectIdentifier mySEI { get;}
}
