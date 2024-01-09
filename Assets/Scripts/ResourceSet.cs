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
        public ResourceAmount(ItemScriptableObject iso, int amount)
        {
            this.iso = iso;
            this.amount = amount;
        }
        public ItemScriptableObject iso;
        public int amount;
    }

    /// <summary>
    /// Fully create ResourceSet with "spirit point amount" and "resource and amount list"
    /// </summary>
    /// <param name="spiritPointAmount"></param>
    /// <param name="resourceAndAmountList"></param>
    public ResourceSet(int spiritPointAmount, List<ResourceAmount> resourceAndAmountList)
    {
        spiritPoint = spiritPointAmount;
        resources = resourceAndAmountList;
    }

    /// <summary>
    /// Create an empty ResourceSet
    /// </summary>
    public ResourceSet()
    {
        spiritPoint = 0;
        resources = new List<ResourceAmount>();
    }
    
    /// <summary>
    /// Create a ResourceSet with only SpiritPointAmount
    /// </summary>
    /// <param name="spiritPointAmount"></param>
    public ResourceSet(int spiritPointAmount)
    {
        spiritPoint = spiritPointAmount;
        resources = new List<ResourceAmount>();
    }

    public int spiritPoint;
    public List<ResourceAmount> resources = null;

    public void AddResource(ItemScriptableObject iso, int amount)
    {
        resources.Add(new ResourceAmount(iso,amount));
    }
    
    //------------------------------------------------------------

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
            //UnityEngine.Debug.Log("gain resource " + ra.iso + "  x  " + ra.amount);
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
