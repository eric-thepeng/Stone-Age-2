using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.XR;

public class ExploreSpot : MonoBehaviour
{
    [Serializable]
    public class SpotResourceInfo
    {
        public ItemScriptableObject item;
        public int amount;
    }

    public List<SpotResourceInfo> exploreSpotResource = new List<SpotResourceInfo>();

    public static Dictionary<string, string[]> exploreSpotUnveilDic = new Dictionary<string, string[]>() {
        { "11", new string[]{"21", "22", "23"} },
        { "23", new string[]{"31" } },
        { "31", new string[]{"41", "42"} },
        { "41", new string[]{"61" } },
        { "42", new string[]{"51", "71"} },
    };
    public static Dictionary<string, ExploreSpot> allExploreSpots = new Dictionary<string, ExploreSpot>();

    public string spotName; // 00: area 00:level 00:position
    public int spiritPoint = 1;
    //public ItemScriptableObject[] resource = new ItemScriptableObject[0];
    //public int[] weight = new int[0];
    //public int totalWeight;
    public int gatherTime;

    public enum LockState {UNLOCKED, CAN_UNLOCK, CANNOT_UNLOCK}
    [SerializeField] LockState lockState = LockState.CANNOT_UNLOCK;

    public enum ActiveState {FREE, GATHERING }
    ActiveState activeState = ActiveState.FREE;
    Character gatheringCharacter = null;

    public Color32 unlockedColor;
    public Color32 canUnlockColor;
    public Color32 cannotUnlockColor;

    public int unlockSpiritPoint = 0 ;
    // The 
    public ItemScriptableObject[] unlockResource = new ItemScriptableObject[0];
    public int[] unlockResrouceAmount = new int[0];

    // CircularUI
    public CircularUI gatherCircularUI;
    public CircularUI energyCircularUI;

    ExploreSpotIndicator myExploreSpotIndicator;

    ExploreSpotUnlock myExploreSpotUnlock;

    private GameObject highlightIndicator;

    private void Awake()
    {
        allExploreSpots.Add(spotName, this);
    }

    private void Start()
    {
        highlightIndicator = transform.Find("Highlight Indicator").gameObject;
        gatherCircularUI = transform.Find("Gathering Circular UI").GetComponent<CircularUI>();
        energyCircularUI = transform.Find("Energy Circular UI").GetComponent<CircularUI>();
        myExploreSpotIndicator = transform.Find("Explore Spot Indicator").GetComponent<ExploreSpotIndicator>();
        myExploreSpotUnlock = transform.Find("Explore Spot Unlock").GetComponent<ExploreSpotUnlock>();
        SetUpAccordingToLockState();
    }

    private void OnEnable()
    {
        CharacterIcon.onCharacterPickedUp += ShowHighlightIndicator;
        CharacterIcon.onCharacterQuitPickUp += CloseHighlightIndicator;
    }

    private void OnDisable()
    {
        CharacterIcon.onCharacterPickedUp -= ShowHighlightIndicator;
        CharacterIcon.onCharacterQuitPickUp -= CloseHighlightIndicator;
    }

    public void ShowHighlightIndicator()
    {
        if(lockState == LockState.UNLOCKED) highlightIndicator.SetActive(true);
    }

    public void CloseHighlightIndicator()
    {
        highlightIndicator.SetActive(false);
    }

    private void SetUpAccordingToLockState()
    {

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
        

        //pass info for resource indicator and unlock panel, then disable them temporarily.
        myExploreSpotIndicator.PassInResourceInfo(spiritPoint, exploreSpotResource);
        myExploreSpotUnlock.PassInResourceInfo(unlockResource, unlockResrouceAmount, unlockSpiritPoint);
        myExploreSpotIndicator.gameObject.SetActive(false);
        myExploreSpotUnlock.gameObject.SetActive(false);


        if (isUnlocked())
        {
            // Set up resource indicator
            myExploreSpotIndicator.CreatResourceIndicator();
        }
        else if (isCanUnlock())
        {
            // Set up unlock panel
            myExploreSpotUnlock.CreatResourceIndicator();
        }
        
        CloseHighlightIndicator();
    }

    public void SetGatheringProgress(float gatherPercentage, float energyPercentage, bool isLerp)
    {
        gatherCircularUI.SetCircularUIPercentage(gatherPercentage, false);
        energyCircularUI.SetCircularUIPercentage(energyPercentage, isLerp);
    }

    public void SetCircularUIState(CircularUI.CircularUIState circularUIState)
    {
        gatherCircularUI.SetCircularUIState(circularUIState);
        energyCircularUI.SetCircularUIState(circularUIState);
    }

    private void DiscoverAdjacent()
    {
        if (!exploreSpotUnveilDic.ContainsKey(spotName)) return;
        foreach (string esName in exploreSpotUnveilDic[spotName])
        {
            if (!allExploreSpots.ContainsKey(esName)) continue;
            allExploreSpots[esName].SetLockState(LockState.CAN_UNLOCK);

            allExploreSpots[esName].SetUnlockPanel(true);
        }
    }

    public void PlaceCharacter(Sprite sp, Character inCharacter)
    {
        gatheringCharacter = inCharacter;
        transform.Find("Character Sprite").GetComponent<SpriteRenderer>().sprite = sp;
        activeState = ActiveState.GATHERING;
    }

    public void EndGathering()
    {
        gatheringCharacter = null;
        transform.Find("Character Sprite").GetComponent<SpriteRenderer>().sprite = null;
        activeState = ActiveState.FREE;
    }

    /*
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
    }*/

    public void SetLockState(LockState newLockState)
    {
        if(newLockState == LockState.UNLOCKED)
        {
            // A temp code here to set the resource indicator up after this spot being unlock;
            myExploreSpotIndicator.CreatResourceIndicator();

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
        SetUpAccordingToLockState();
    }

    public void SpotUnlock()
    {
        //check if enough resource
        if (SpiritPoint.i.GetAmount() < unlockSpiritPoint) {
            UnlockFailed();
            return;
        } 
        for(int i = 0; i<unlockResource.Length; i++)
        {
            if (Inventory.i.ItemInStockAmount(unlockResource[i]) < unlockResrouceAmount[i])
            {
                UnlockFailed();
                return;
            }
        }

        //use resource
        SpiritPoint.i.Use(unlockSpiritPoint);
        for (int i = 0; i < unlockResource.Length; i++)
        {
            for(int m = 0; m< unlockResrouceAmount[i]; m++)
            {
                Inventory.i.UseItemFromStock(unlockResource[i]);
            }
        }

        //unlock
        SetLockState(LockState.UNLOCKED);
        SetUnlockPanel(false);
    }

    private void UnlockFailed()
    {
        Debug.Log("Do not have enough resource");
    }

    public void SetUnlockPanel(bool OnOff)
    {
        myExploreSpotUnlock.gameObject.SetActive(OnOff);
    }

    public bool isUnlocked() { return lockState == LockState.UNLOCKED; }
    public bool isCanUnlock() { return lockState == LockState.CAN_UNLOCK; }
    public bool isCannotUnlock() { return lockState == LockState.CANNOT_UNLOCK; }


    private void OnMouseEnter()
    {
        if (activeState != ActiveState.GATHERING) return;
        DisplayRecallButton();
    }

    private void OnMouseExit()
    {
        if (activeState != ActiveState.GATHERING) return;
        CancelRecallButton();
    }

    private void DisplayRecallButton()
    {
        GetComponent<WorldSpaceButton>().SetButtonActive(true);
        transform.Find("Call Back Button").gameObject.SetActive(true);
    }

    private void CancelRecallButton()
    {
        GetComponent<WorldSpaceButton>().SetButtonActive(false);
        transform.Find("Call Back Button").gameObject.SetActive(false);
    }

    public void CancelGather()
    {
        gatheringCharacter.EndGather();
        CancelRecallButton();
    }

}
