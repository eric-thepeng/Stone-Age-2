using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatsMonitor : MonoBehaviour
{
    private int trashTotalClearedAmount = 0;
    private Dictionary<ItemScriptableObject, int> isoTotalGainedAmount = new Dictionary<ItemScriptableObject, int>();
    private Dictionary<BuildingISO, int> bisoTotalBuiltAmount = new Dictionary<BuildingISO, int>();

    public UnityEvent<int> trashCleared = new UnityEvent<int>();
    public UnityEvent<ItemScriptableObject, int> isoGained = new UnityEvent<ItemScriptableObject, int>();
    public UnityEvent bisoBuilt = new UnityEvent();
    
    

}
