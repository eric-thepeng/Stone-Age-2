using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BLDUnlock : MonoBehaviour,IPassResourceSet
{
    ResourceSet unlockCost = null;
    public void PassResourceSet(ResourceSet rs)
    {
        unlockCost = rs;
    }
}
