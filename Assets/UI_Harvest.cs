using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Harvest : MonoBehaviour
{
    static UI_Harvest instance;
    public static UI_Harvest i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_Harvest>();
            }
            return instance;
        }
    }

    class HarvestInfo
    {
        ItemScriptableObject iso;
        int amount;
        float disappearTime = 3;
        float disappearTimeLeft = 3f;

        public HarvestInfo(ItemScriptableObject newIso, int initialAmount)
        {
            iso = newIso;
            amount = initialAmount;
        }

        public void AddAmount(int am)
        {
            amount += am;
        }

        public void ResetTime()
        {
            disappearTimeLeft = disappearTime;
        }

        public bool SpendTime(float delta)
        {
            disappearTimeLeft -= delta;
            if (disappearTime < 0) return false;
            return true;
        }

        public bool IsISO(ItemScriptableObject checkIso)
        {
            return iso == checkIso;
        }

    }

    List<HarvestInfo> harvestInfoList = new List<HarvestInfo>();

    public void AddItem(ItemScriptableObject iso, int amount)
    {
        foreach(HarvestInfo hi in harvestInfoList)
        {
            if (hi.IsISO(iso))
            {
                hi.AddAmount(amount);
                return;
            }
        }
        harvestInfoList.Add(new HarvestInfo(iso, amount));
    }

    private void Update()
    {
        for(int i = harvestInfoList.Count - 1; i >=0 ; i--)
        {
            if(harvestInfoList[i].SpendTime(Time.deltaTime) == false)
            {
                harvestInfoList.Remove(harvestInfoList[i]);
            }
        }
    }

}
