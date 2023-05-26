using System;
using System.Collections;
using System.Collections.Generic;
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
        //TODO: actually spend resource
        return true;
    }
}
