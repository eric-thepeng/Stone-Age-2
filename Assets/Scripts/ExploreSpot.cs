using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreSpot : MonoBehaviour
{
    public static Dictionary<string, string[]> exploreSpotUnveilDic = new Dictionary<string, string[]>() {
        { "11", new string[]{"21","22","23"} },
        { "23", new string[]{"31" } },
        { "31", new string[]{"41", "42", "43", "44" } },
    };
    public static Dictionary<string, ExploreSpot> allExploreSpots = new Dictionary<string, ExploreSpot>();

    public string spotName; // 00: area 00:level 00:position
    public int spiritPoint = 1;
    public ItemScriptableObject[] resource = new ItemScriptableObject[0];
    public int[] weight = new int[0];
    public int totalWeight;
    public int gatherTime;

    public enum LockState {UNLOCKED, CAN_UNLOCK, CANNOT_UNLOCK}
    [SerializeField] LockState lockState = LockState.CANNOT_UNLOCK;

    public Color32 unlockedColor;
    public Color32 canUnlockColor;
    public Color32 cannotUnlockColor;

    public int unlockSpiritPoint = 0 ;
    public ItemScriptableObject[] unlockResource = new ItemScriptableObject[0];
    public string[] unlockResrouceAmount = new string[0];

    // CircularUI
    CircularUI circularUI;

    private void Awake()
    {
        allExploreSpots.Add(spotName, this);
    }

    private void Start()
    {
        SetUp();
    }

    private void SetUp()
    {
        circularUI = transform.Find("Circular UI").GetComponent<CircularUI>();

        if (lockState == LockState.UNLOCKED)
        {
            GetComponent<SpriteRenderer>().color = unlockedColor;
        }
        else if (lockState == LockState.CAN_UNLOCK)
        {
            GetComponent<SpriteRenderer>().color = canUnlockColor;
        }
        else //(lockState == LockState.CAN_UNLOCK)
        {
            GetComponent<SpriteRenderer>().color = cannotUnlockColor;
        }
        totalWeight = 0;
        foreach (int i in weight)
        {
            totalWeight += i;
        }
    }

    public void SetGatheringProgress(float percentage)
    {
        circularUI.SetCircularUIPercentage(percentage);
    }

    private void DiscoverAdjacent()
    {
        if (!exploreSpotUnveilDic.ContainsKey(spotName)) return;
        foreach (string esName in exploreSpotUnveilDic[spotName])
        {
            if (!allExploreSpots.ContainsKey(esName)) continue;
            allExploreSpots[esName].SetLockState(LockState.CAN_UNLOCK);
        }
    }

    public void PlaceCharacter(Sprite sp)
    {
        transform.Find("Character Sprite").GetComponent<SpriteRenderer>().sprite = sp;
        circularUI.SetCircularUIType(CircularUI.CircularUIType.Gathering);
    }

    public void EndGathering()
    {
        transform.Find("Character Sprite").GetComponent<SpriteRenderer>().sprite = null;
        circularUI.SetCircularUIType(CircularUI.CircularUIType.Null);
    }

    public string GetDisplayInfo()
    {
        string text = spotName + "<br> <br>";
        if (lockState == LockState.UNLOCKED)
        {
            text += "Gather reward: " + spiritPoint + " Spirit Points<br>";
            if (resource.Length == 0) return text;
            text += "Possible resources: <br>";
            for(int i =0; i<resource.Length; i++)
            {
                text += "    " +  resource[i].tetrisHoverName + "  " + (int)((weight[i]/1f)/totalWeight * 100) + " %" + "<br>";
            }
        }
        else if (lockState == LockState.CAN_UNLOCK)
        {
            text += "Unlock Cost: " + unlockSpiritPoint + " Spirit Points<br>";
            if (unlockResource.Length == 0) return text;
            text += "And: <br>";
            for(int i =0; i<unlockResource.Length; i++)
            {
                text += "    " + unlockResrouceAmount[i] + "x" + unlockResource[i].name + "<br>";
            }
        }
        else //(lockState == LockState.CAN_UNLOCK)
        {
            text = "??";
        }
        return text;
    }

    public void SetLockState(LockState newLockState)
    {
        if(newLockState == LockState.UNLOCKED)
        {
            if (lockState == LockState.UNLOCKED) { Debug.LogError("This Explore Spot is already UNLOCKED"); return; }
            if (lockState == LockState.CANNOT_UNLOCK) { Debug.LogError("This Explore Spot is still CANNOT_UNLOCK"); return; }
            lockState = LockState.UNLOCKED;
            DiscoverAdjacent();
        }
        else if(newLockState == LockState.CAN_UNLOCK)
        {
            if (lockState == LockState.UNLOCKED) { Debug.LogError("This Explore Spot is already unlocked"); return; }
            if (lockState == LockState.CAN_UNLOCK) { Debug.LogError("This Explore Spot is already CAN_UNLOCK"); return; }
            lockState = LockState.CAN_UNLOCK;
        }
        else //newLockState == LockState.CANNOT_UNLOCK
        {
            lockState = LockState.CANNOT_UNLOCK;
        }
        SetUp();
    }

    public bool isUnlocked() { return lockState == LockState.UNLOCKED; }
    public bool isCanUnlock() { return lockState == LockState.CAN_UNLOCK; }
    public bool isCannotUnlock() { return lockState == LockState.CANNOT_UNLOCK; }
}
