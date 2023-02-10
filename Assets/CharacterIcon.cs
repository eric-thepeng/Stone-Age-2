using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIcon : MonoBehaviour
{
    enum IconState {Home, Dragging, Placed}
    IconState iconState = IconState.Home;

    public Character character;

    Vector3 targetPosition;
    float moveSpeed = 15;

    Vector3 homePosition;
    Vector3 placeholderPosition;

    private void Update()
    {
        if(iconState == IconState.Dragging)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if(WorldUtility.TryMouseHitPoint(20, true)) // DRAGGING -> PLACED
                {
                    ExploreSpot toExplore = WorldUtility.GetMouseHitObject(20, true).GetComponent<ExploreSpot>();
                    toExplore.PlaceCharacter(gameObject.GetComponent<SpriteRenderer>().sprite);
                    character.StartGather(toExplore, this);
                    iconState = IconState.Placed;
                    transform.localPosition = placeholderPosition;
                    return;
                }
                else // DRAGGING -> HOME
                {
                    iconState = IconState.Home;
                    transform.localPosition = homePosition;
                    return;
                }
            }

            targetPosition = WorldUtility.GetMouseHitPoint(9, true);
            if (transform.position != targetPosition)
            {
                transform.position += (targetPosition - transform.position) * Time.deltaTime * moveSpeed;
            }
        }
    }

    private void OnMouseDown() // HOME -> DRAGGING
    {
        homePosition = transform.localPosition;
        placeholderPosition = homePosition + new Vector3(-10, 0, 0);
        iconState = IconState.Dragging;
    }

    public void ResetHome()
    {
        iconState = IconState.Home;
        transform.localPosition = homePosition;
    }


}
