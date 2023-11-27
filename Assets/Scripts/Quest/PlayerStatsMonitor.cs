using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats<T>
{

    private Dictionary<T, int> statsCategoryAndAmount = new Dictionary<T, int>();

    public UnityEvent<T,int> broadcastStatsChange = new UnityEvent<T,int>();

    public void TriggerStatsChange(T statsCategory, int deltaAmount)
    {
        if (statsCategoryAndAmount.ContainsKey(statsCategory)) statsCategoryAndAmount[statsCategory] += deltaAmount;
        else statsCategoryAndAmount.Add(statsCategory,0+deltaAmount);
        broadcastStatsChange.Invoke(statsCategory, statsCategoryAndAmount[statsCategory]);
    }

    public int GetCurrentStats(T targetCategroy)
    {
        if (statsCategoryAndAmount.ContainsKey(targetCategroy)) return statsCategoryAndAmount[targetCategroy];
        else return 0;
    }
}

interface IPlayerStatIdentifier
{
    public string identifingName { get; set; }
}

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
        if(!data.ContainsKey(index)) data.Add(index, new PlayerStat());
        data[index].ChangeAmount(delta);
    }

    public int GetAmount(T index)
    {
        if (!data.ContainsKey(index)) return 0;
        return data[index].GetAmount();
    }
}

public static class PlayerStatsMonitor
{
    public enum PlayerStatsType
    {
        TrashTotalClear,
        ISOTotalGain,
        ISOTotalSpend,
        BISOBuild
    }
    
    static public PlayerStats<PlayerStatsType> trashTotalClear = new PlayerStats<PlayerStatsType>();
    static public PlayerStats<ItemScriptableObject> isoTotalGainPlayerStat = new PlayerStats<ItemScriptableObject>();
    //static public PlayerStats<ItemScriptableObject> isoTotalSpendPlayerStat = new PlayerStats<ItemScriptableObject>();
    static public PlayerStats<BuildingISO> bisoTotalBuildPlayerStat = new PlayerStats<BuildingISO>();
    
}
