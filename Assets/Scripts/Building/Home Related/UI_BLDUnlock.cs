using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BLDUnlock : MonoBehaviour,IPassResourceSet
{
    ResourceSetDisplayer rsd = null;
    [SerializeField] Transform rsdTemplate;
    [SerializeField] Vector3 displacement;
    ResourceSet unlockCost = null;
    public void PassResourceSet(ResourceSet rs)
    {
        unlockCost = rs;
        rsd = new ResourceSetDisplayer(unlockCost,rsdTemplate,displacement,this.transform);
    }

}
