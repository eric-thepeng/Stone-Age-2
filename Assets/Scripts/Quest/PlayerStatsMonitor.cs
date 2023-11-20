using System;
using System.Collections;
using System.Collections.Generic;
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
