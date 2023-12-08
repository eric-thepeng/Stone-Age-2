using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStat
{
    private UnityEvent<int> broadcastStatChange;
    private int amount;
    private int accumulatedAmount;
    public PlayerStat(int amount = 0)
    {
        this.amount = amount;
        accumulatedAmount = amount;
        broadcastStatChange = new UnityEvent<int>();
    }

    public int GetAmount()
    {
        return amount;
    }

    public void ChangeAmount(int delta)
    {
        amount += delta;
        if (delta > 0) accumulatedAmount += delta;
        broadcastStatChange.Invoke(amount);
        
    }
    
    public void AssignAmount(int newAmount)
    {
        amount = newAmount;
        broadcastStatChange.Invoke(amount);
    }

    public void SubscribeStatChange(UnityAction<int> newUnityAction)
    {
        broadcastStatChange.AddListener(newUnityAction);
    }

    public void UnsubscribeStatChange(UnityAction<int> newUnityAction)
    {
        broadcastStatChange.RemoveListener(newUnityAction);
    }

}

public class PlayerStatCollection<T>
{
    public Dictionary<T, PlayerStat> data;
    
    public PlayerStatCollection()
    {
        data = new Dictionary<T, PlayerStat>();
    }
    
    public void ChangeAmount(T index, int delta)
    {
        GetPlayerStat(index).ChangeAmount(delta);
    }

    public PlayerStat GetPlayerStat(T index)
    {
        if(!data.ContainsKey(index)) data.Add(index, new PlayerStat());
        return data[index];
    }
}

public static class PlayerStatsMonitor
{
    public enum PlayerStatType
    {
        TrashTotalCleared,
        SpiritPointCurrent,
        //SpiritPointTotalGained,
        
        //ISO RELATED
        ISOTotalGained,
        BISOTotalBuilt,
    }
    
    static public PlayerStat GetPlayerStat(PlayerStatType pst, ItemScriptableObject iso = null)
    {
        if (pst == PlayerStatType.TrashTotalCleared) return trashTotalClearedPlayerStat;
        if (pst == PlayerStatType.SpiritPointCurrent) return SpiritPoint.i.GetPlayerStat();
        if (iso == null)
        {
            Debug.LogError("Need to assign ISO parameter for GetPlayerStat");
            return null;
        }
        if (pst == PlayerStatType.ISOTotalGained) return isoTotalGainedPlayerStatCollection.GetPlayerStat(iso);
        if (pst == PlayerStatType.BISOTotalBuilt) return bisoTotalBuiltPlayerStatCollection.GetPlayerStat(iso);
        Debug.LogError("Cannot find PlayerStat");
        return null;
    }

    //New PlayerStatSystem
    static public PlayerStat trashTotalClearedPlayerStat = new PlayerStat();
    
    static public PlayerStatCollection<ItemScriptableObject> isoTotalGainedPlayerStatCollection =
        new PlayerStatCollection<ItemScriptableObject>();
    static public PlayerStatCollection<ItemScriptableObject> bisoTotalBuiltPlayerStatCollection =
        new PlayerStatCollection<ItemScriptableObject>();

}
