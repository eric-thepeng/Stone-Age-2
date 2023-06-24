using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

[Serializable]public class ResourceSet
{
    [Serializable]
    public class ResourceAmount
    {
        public ItemScriptableObject iso;
        public int amount;
    }
    public int spiritPoint;
    public List<ResourceAmount> resources;

    public bool SpendResource()
    {
        return Inventory.i.SpendResourceSet(this);
    }

    public void GainResource()
    {
        
        SpiritPoint.i.Add(spiritPoint);
        foreach ( ResourceAmount ra in resources)
        {
            Inventory.i.AddInventoryItem(ra.iso, ra.amount);
            UnityEngine.Debug.Log("gain resource " + ra.iso + "  x  " + ra.amount);
        }
    }
}
