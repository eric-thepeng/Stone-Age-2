using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class WorldSpaceButton : MonoBehaviour
{
    [SerializeField] UnityEvent clickEvent;
    [SerializeField] UnityEvent doubleClickEvent;
    [SerializeField] protected Color32 normalColor;
    [SerializeField] protected Color32 hoverColor;
    [SerializeField] protected Color32 pressColor;

    [SerializeField] bool buttonActive = true;
    [SerializeField] SpriteRenderer affectSR = null;

    bool waitingSecondClick = false;
    float waitTime = 0.2f;

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

        if (waitingSecondClick) //execute double click
        {
            doubleClickEvent.Invoke();
            StopAllCoroutines();
            print("double click");
            afterClick();
        }
        else if(doubleClickEvent != null) //execute single click
        {
            clickEvent.Invoke();
        }
        else //start wait for double click
        {
            StartCoroutine(WaitForSecondClick());
        }
       targetSR.color = normalColor;

    }

    private void afterClick()
    {
        waitingSecondClick = false;
    }

    IEnumerator WaitForSecondClick()
    {
        waitingSecondClick = true;
        float timeCount = 0f;
        while(timeCount < waitTime)
        {
            timeCount += Time.deltaTime;
            yield return 0; 
        }
        afterClick();
        clickEvent.Invoke();
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
