using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class WorldSpaceButton : MonoBehaviour
{
    [SerializeField] UnityEvent clickEvent;
    [SerializeField] protected Color32 normalColor;
    [SerializeField] protected Color32 hoverColor;
    [SerializeField] protected Color32 pressColor;

    [SerializeField] bool buttonActive = true;
    [SerializeField] SpriteRenderer affectSR = null;

    SpriteRenderer targetSR = null;

    private void Start()
    {
        //print(gameObject.name + "  " + transform.parent.gameObject.name + " " + buttonActive);
        if(affectSR == null)
        {
            targetSR = GetComponent<SpriteRenderer>();
        }
        else
        {
            targetSR = affectSR;
        }
    }

    protected void OnMouseEnter()
    {
        if (!buttonActive) return;
        targetSR.color = hoverColor;
    }

    private void OnMouseExit()
    {
        if (!buttonActive) return;
        targetSR.color = normalColor;
    }

    private void OnMouseDown()
    {
        if (!buttonActive) return;
        targetSR.color = pressColor;
    }

    private void OnMouseUpAsButton()
    {
        if (!buttonActive) return;
        clickEvent.Invoke();
        targetSR.color = hoverColor;

    }

    public void SetClickEvent(UnityEvent newEvent)
    {
        clickEvent = newEvent;
    }

    public void SetButtonActive(bool toState)
    {
        buttonActive= toState;
    }

    public bool IsActive()
    {
        return buttonActive;
    }
}
