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
    public List<ResourceAmount> resources = null;

    public bool SpendResource()
    {
        if (IsEmpty()) return true;
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

    public bool IsEmpty()
    {
        return spiritPoint == 0 && (resources == null || resources.Count == 0);
    }

    public override string ToString()
    {
        string stringToPrint = "Spirit Points: " + spiritPoint + " ";
        foreach (ResourceAmount ra in resources)
        {
            stringToPrint += ra.iso + " " + ra.amount + " ";
        }
        return stringToPrint;
    }
}
