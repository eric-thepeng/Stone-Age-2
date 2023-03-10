using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public CircularUI gatherCircularUI;
    public CircularUI energyCircularUI;

    private void Awake()
    {
        character.SetUp(this);
    }

    private void Start()
    {
        gatherCircularUI = transform.Find("Gathering Circular UI").GetComponent<CircularUI>();
        energyCircularUI = transform.Find("Energy Circular UI").GetComponent<CircularUI>();
    }

    private void Update()
    {
        if (iconState == IconState.Dragging)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (WorldUtility.TryMouseHitPoint(WorldUtility.LAYER.EXPLORATION_SPOT, true)) // DRAGGING -> find a explore spot
                {
                    transform.parent.Find("Background").gameObject.SetActive(false);
                    ExploreSpot toExplore = WorldUtility.GetMouseHitObject(WorldUtility.LAYER.EXPLORATION_SPOT, true).GetComponent<ExploreSpot>();
                    if (toExplore.isUnlocked()) // DRAGGING -> PLACED
                    {
                        toExplore.PlaceCharacter(gameObject.GetComponent<SpriteRenderer>().sprite, character);
                        character.StartGather(toExplore, this);
                        //transform.localPosition = placeholderPosition;
                        transform.localPosition = homePosition;
                        ChangeIconColor(gatherColor);
                        iconState = IconState.Gathering;

                        return;
                    }
                }
                // DRAGGING -> HOME
                iconState = IconState.Home;
                transform.localPosition = homePosition;
                return;
            }

            targetPosition = WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.UI_BACKGROUND, true);
            if (transform.position != targetPosition)
            {
                transform.position += (targetPosition - transform.position) * Time.deltaTime * moveSpeed;
            }
        }
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

    private void OnMouseDown() // HOME -> DRAGGING
    {
        if (iconState == IconState.Home && PlayerState.IsBrowsing())
        {
            transform.parent.Find("Background").gameObject.SetActive(true);
            homePosition = transform.localPosition;
            // placeholderPosition = homePosition + new Vector3(-10, 0, 0);

            iconState = IconState.Dragging;
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
        character.EndGather();
        CancelRecallButton();
    }

    void ChangeIconColor(Color32 color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
}
