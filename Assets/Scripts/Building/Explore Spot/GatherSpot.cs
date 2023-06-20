using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.XR;

public class GatherSpot : MonoBehaviour
{
    // Explore Spot Information
    public static Dictionary<string, string[]> exploreSpotUnveilDic = new Dictionary<string, string[]>() {
        { "11", new string[]{"21", "22", "23"} },
        { "23", new string[]{"31" } },
        { "31", new string[]{"41", "42"} },
        { "41", new string[]{"61" } },
        { "42", new string[]{"51", "71"} },
    };
    public static Dictionary<string, GatherSpot> allExploreSpots = new Dictionary<string, GatherSpot>();
    public string spotName; // 00: area 00:level 00:position
    
    // Gather Information
    public int gatherTime;
    public ResourceSet gatherResource;

    // Active State
    public enum ActiveState {FREE, GATHERING}
    ActiveState activeState = ActiveState.FREE;
    Character gatheringCharacter = null;

    // CircularUI
    public CircularUI gatherCircularUI;
    public CircularUI energyCircularUI;

    // Highlight Indication
    private GameObject highlightIndicator;

    private void Awake()
    {
        allExploreSpots.Add(spotName, this);
    }

    private void Start()
    {
        highlightIndicator = transform.Find("Highlight Indicator").gameObject;
        gatherCircularUI = transform.Find("Circular UI").Find("Gathering Circular UI").GetComponent<CircularUI>();
        energyCircularUI = transform.Find("Circular UI").Find("Energy Circular UI").GetComponent<CircularUI>();
        CloseHighlightIndicator();
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
        highlightIndicator.SetActive(true);
    }

    public void CloseHighlightIndicator()
    {
        highlightIndicator.SetActive(false);
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
            
            //TODO: SET UNLOCK STATE
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
