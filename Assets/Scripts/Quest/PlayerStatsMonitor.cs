using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStat
{
    private UnityEvent<int> broadcastStatChange;
    private UnityEvent<int> broadcastStatDelta;

    private int amount;
    private int accumulatedAmount;
    public PlayerStat(int amount = 0)
    {
        this.amount = amount;
        accumulatedAmount = amount;
        broadcastStatChange = new UnityEvent<int>();
        broadcastStatDelta = new UnityEvent<int>();
    }

    public int GetAmount()
    {
        return amount;
    }

    /// <summary>
    /// Change delta amount of the stat
    /// </summary>
    /// <param name="delta"></param>
    public void ChangeAmount(int delta)
    {
        amount += delta;
        if (delta > 0) accumulatedAmount += delta;
        broadcastStatChange.Invoke(amount);
        broadcastStatDelta.Invoke(delta);
    }
    
    public void AssignAmount(int newAmount)
    {
        int delta = newAmount - amount;
        amount = newAmount;
        broadcastStatChange.Invoke(amount);
        broadcastStatDelta.Invoke(amount);
    }

    /// <summary>
    /// broadcasted int is exact amount after each change
    /// </summary>
    /// <param name="newUnityAction"></param>
    public void SubscribeStatChange(UnityAction<int> newUnityAction)
    {
        broadcastStatChange.AddListener(newUnityAction);
    }

    public void UnsubscribeStatChange(UnityAction<int> newUnityAction)
    {
        broadcastStatChange.RemoveListener(newUnityAction);
    }
    
    /// <summary>
    /// broadcasted int is delta amount changed during each change
    /// </summary>
    /// <param name="newUnityAction"></param>
    public void SubscribeStatDelta(UnityAction<int> newUnityAction)
    {
        broadcastStatDelta.AddListener(newUnityAction);
    }
    
    public void UnsubscribeStatDelta(UnityAction<int> newUnityAction)
    {
        broadcastStatDelta.RemoveListener(newUnityAction);
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
        ISOInStockAmount
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
        if (pst == PlayerStatType.BISOTotalBuilt) return Inventory.i.GetInBuildPlayerStat(iso);
        if (pst == PlayerStatType.ISOInStockAmount) return Inventory.i.GetISOInstockPlayerStat(iso);
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
