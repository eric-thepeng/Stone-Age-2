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

    [SerializeField] bool autoTint;

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

        if (autoTint)
        {
            normalColor = targetSR.color;
            hoverColor = new Color(normalColor.r * 0.8f /255, normalColor.g * 0.8f / 255, normalColor.b * 0.8f / 255, normalColor.a);
            pressColor = new Color(normalColor.r * 0.6f /255, normalColor.g * 0.6f / 255, normalColor.b * 0.6f / 255, normalColor.a);
        }
    }

    protected void OnMouseEnter()
    {
        if (!buttonActive) return;
        targetSR.color = hoverColor;

        if (!JSAM.AudioManager.IsSoundPlaying(JSAM.SoundsStoneAge2.Button_Hover_01))
        {
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Button_Hover_01);
        }
  
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

        // Check button event type and play sound - Will
        if (clickEvent.GetPersistentEventCount() == 0) return;
        if (clickEvent.GetPersistentMethodName(0) == "CraftingPanelOpenButton")
        {
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Button_Click_01);
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Menu_Open_01);
        }
        else if (clickEvent.GetPersistentMethodName(0) == "RecipeMapOpenButton")
        {
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Button_Click_01);
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Menu_Open_01);
        }
        else if (clickEvent.GetPersistentMethodName(0) == "RecipeViewerPanelOpenButton")
        {
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Button_Click_01);
        }
        else if (clickEvent.GetPersistentMethodName(0) == "BuildingSystemOpenButton")
        {
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Button_Click_01);
        }
        else if (clickEvent.GetPersistentMethodName(0) == "CameraBackHomeButton")
        {
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Button_Click_01);
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Menu_Open_01);
        }
        else if (clickEvent.GetPersistentMethodName(0) == "HomeReturnButton")
        {
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Button_Click_01);
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Menu_Open_01);
        }
        else if (clickEvent.GetPersistentMethodName(0) == "InventoryPanelOpenButton")
        {
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Button_Click_01);
        }
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
