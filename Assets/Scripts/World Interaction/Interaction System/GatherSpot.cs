using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherSpot : MonoBehaviour
{
    // Gather Information
    public int gatherTime;
    public ResourceSet gatherResource;
    
    [Header("------ DO NOT EDIT BELOW ------")]

    // Call Back Character Icon Button
    public GameObject callBackButtonSprite;
    public GameObject characterSpriteHolder;
    
    // CircularUI
    public CircularUI gatherCircularUI;
    public CircularUI energyCircularUI;

    // Highlight Indication
    private GameObject highlightIndicator;
    
    // Active State
    public enum ActiveState {FREE, GATHERING}
    ActiveState activeState = ActiveState.FREE;
    Character gatheringCharacter = null;

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

    public void PlaceCharacter(Sprite sp, Character inCharacter)
    {
        gatheringCharacter = inCharacter;
        characterSpriteHolder.GetComponent<SpriteRenderer>().sprite = sp;
        activeState = ActiveState.GATHERING;
    }

    public void EndGathering()
    {
        gatheringCharacter = null;
        characterSpriteHolder.GetComponent<SpriteRenderer>().sprite = null;
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
    
    protected virtual void OnMouseEnter()
    {
        if (activeState != ActiveState.GATHERING) return;
        DisplayRecallButton();
    }

    protected virtual void OnMouseExit()
    {
        //if (activeState != ActiveState.GATHERING) return;
        CancelRecallButton();
    }

    private void DisplayRecallButton()
    {
        GetComponent<WorldSpaceButton>().SetButtonActive(true);
        callBackButtonSprite.gameObject.SetActive(true);
    }

    private void CancelRecallButton()
    {
        GetComponent<WorldSpaceButton>().SetButtonActive(false);
        callBackButtonSprite.gameObject.SetActive(false);
    }

    public void CancelGather()
    {
        gatheringCharacter.EndGather();
        CancelRecallButton();
    }

}
