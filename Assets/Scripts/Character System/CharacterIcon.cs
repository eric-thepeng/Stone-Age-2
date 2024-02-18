using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using DG.Tweening;
using UnityEngine.UI;

public class CharacterIcon : MonoBehaviour
{
    enum IconState { Home, Dragging, Gathering }
    IconState iconState = IconState.Home;

    public Character character;

    Vector3 targetPosition;
    float moveSpeed = 15;

    Vector3 homePosition;
    // Vector3 placeholderPosition;

    [SerializeField]
    Color32 gatherColor;
    [SerializeField]
    Color32 homeColor;

    
    public Slider gatherCircularUI;
    public Slider energyCircularUI;

    public delegate void OnCharacterStartGathering();
    public static event OnCharacterStartGathering onCharacterStartGathering;

    public delegate void OnCharacterPickedUp();
    public static event OnCharacterPickedUp onCharacterPickedUp;

    public delegate void OnCharacterQuitPickUp();
    public static event OnCharacterQuitPickUp onCharacterQuitPickUp;
    


    //private void Awake()
    //{
    //    character.SetUp(this);
    //}

    private void Start()
    {
        character.SetUp(this);
        //gatherCircularUI = transform.Find("Gathering Circular UI").GetComponent<CircularUI>();
        //energyCircularUI = transform.Find("Energy Circular UI").GetComponent<CircularUI>();
    }

    private void Update()
    {
        if (iconState == IconState.Dragging)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if(onCharacterQuitPickUp!=null)onCharacterQuitPickUp();
                UI_FullScreenUIDragCollider.i.Close();
                if (WorldUtility.TryMouseHitPoint(WorldUtility.LAYER.EXPLORATION_SPOT, true)) // DRAGGING -> find a explore spot
                {
                        GatherSpot toGather = WorldUtility.GetMouseHitObject(WorldUtility.LAYER.EXPLORATION_SPOT, true).GetComponent<GatherSpot>();
                        toGather.PlaceCharacter(gameObject.GetComponent<SpriteRenderer>().sprite, character);
                        character.StartGatherUI(toGather, this);
                        //transform.localPosition = placeholderPosition;
                        transform.localPosition = homePosition;
                        ChangeIconColor(gatherColor);
                        iconState = IconState.Gathering;
                        if(onCharacterStartGathering != null)onCharacterStartGathering();
                        return;
                }
                // DRAGGING -> HOME
                if(onCharacterQuitPickUp!=null)onCharacterQuitPickUp();
                iconState = IconState.Home;
                transform.localPosition = homePosition;
                return;
            }
            try
            {
                targetPosition = WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.UI_BACKGROUND, true);
            }
            catch {  return; }
            if (transform.position != targetPosition)
            {
                transform.position += (targetPosition - transform.position) * Time.deltaTime * moveSpeed;
            }
        }

    }

    public void SetGatheringProgress(float gatherPercentage, float energyPercentage, bool isLerp)
    {
        if (gatherCircularUI == null || energyCircularUI == null) return;

        gatherCircularUI.value = gatherPercentage/100;
        energyCircularUI.value = energyPercentage/100;
    }

    public void SetEnergyUIVisibility(bool visibility)
    {
        energyCircularUI.gameObject.SetActive(visibility);
    }
    
    public void SetGatherUIVisibility(bool visibility)
    {
        gatherCircularUI.gameObject.SetActive(visibility);
    }

    private void OnMouseDown() // HOME -> DRAGGING
    {
        if (iconState == IconState.Home && (PlayerState.IsBrowsing() || PlayerState.IsExploreMap()) && character.GetHomeStatus().getCurrentHomeState() != CharacterBehaviors.HomeState.Resting)
        {
            UI_FullScreenUIDragCollider.i.Open(this);
            homePosition = transform.localPosition;
            // placeholderPosition = homePosition + new Vector3(-10, 0, 0);

            iconState = IconState.Dragging;
            if(onCharacterPickedUp!=null)onCharacterPickedUp();
        }
        else if(iconState == IconState.Gathering)
        {

        }
    }

    private void OnMouseEnter()
    {
        if(iconState == IconState.Gathering)
        {
            DisplayRecallButton();
        }
    }

    private void OnMouseExit()
    {
        if(iconState == IconState.Gathering)
        {
            CancelRecallButton();
        }
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

    public void ResetHome()
    {
        iconState = IconState.Home;

        // transform.localPosition = homePosition;

        ChangeIconColor(homeColor);
    }

    public void CancelGather()
    {
        character.EndGatherUI();
        CancelRecallButton();
    }

    void ChangeIconColor(Color32 color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    public void ChangeIconColorToHome()
    {
        ChangeIconColor(homeColor);
    }

    public void ChangeIconColorToGather()
    {
        ChangeIconColor(gatherColor);
    }
}
