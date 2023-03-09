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

    bool waitingSecondClick = false;
    float waitTime = 0.2f;

    protected void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().color = hoverColor;
    }

    private void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().color = normalColor;
    }

    private void OnMouseDown()
    {
        GetComponent<SpriteRenderer>().color = pressColor;
    }

    private void OnMouseUpAsButton()
    {
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
        GetComponent<SpriteRenderer>().color = normalColor;

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
}
