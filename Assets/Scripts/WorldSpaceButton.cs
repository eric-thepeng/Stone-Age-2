using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class WorldSpaceButton : MonoBehaviour, IPerformableAction
{
    [SerializeField] UnityEvent clickEvent;

    [SerializeField] protected Color32 normalColor;
    [SerializeField] protected Color32 hoverColor;
    [SerializeField] protected Color32 pressColor;

    [SerializeField] bool autoTint;

    [SerializeField] bool buttonActive = true;
    [SerializeField] SpriteRenderer affectSR = null;
    [SerializeField] string customClickSoundID = "Default Sound";



    private UnityEvent _onActionStarts = new UnityEvent();
    private UnityEvent _onActionComplets = new UnityEvent();
    
    string buttonClickSoundID {
        get
        {
            if (!customClickSoundID.Equals("Default Sound")) { return customClickSoundID; }
            else if (clickEvent.GetPersistentEventCount()>0) { return clickEvent.GetPersistentMethodName(0); }
            else { return "Default Sound"; }
        }
    }

    SpriteRenderer targetSR = null;

    private void Awake()
    {
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

    private void Start()
    {
        //print(gameObject.name + "  " + transform.parent.gameObject.name + " " + buttonActive);

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

    private void OnDisable()
    {
        targetSR.color = normalColor;
    }

    private void OnEnable()
    {
        targetSR.color = normalColor;
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
        onActionStarts.Invoke();
    }

    private void OnMouseUpAsButton()
    {
        if (!buttonActive) return;
        clickEvent.Invoke();
        targetSR.color = hoverColor;
        AudioChannel.i.PlayButtonSound(buttonClickSoundID);
        // Check button event type and play sound - Will
        string a = gameObject.name;
        string b = gameObject.transform.parent.name;
        onActionCompletes.Invoke();
    }

    public void AddClickAction(UnityAction actionToAdd)
    {
        clickEvent.AddListener(actionToAdd);
    }
    
    public void ResetClickEvent(UnityEvent newEvent)
    {
        clickEvent = newEvent;
    }

    public void SetButtonActive(bool toState)
    {
        buttonActive= toState;
    }

    public void SetCustomClickSoundID(string newCustomSoundID)
    {
        customClickSoundID = newCustomSoundID;
    }

    public bool IsActive()
    {
        return buttonActive;
    }

    public void PerformAction()
    {
        //leaves blank
        //NEED TO IMPLEMENT IPerformableAction Better
    }

    public bool IsAssigned()
    {
        return IsActive();
    }

    public UnityEvent onActionStarts { get { return _onActionStarts; } }
    public UnityEvent onActionCompletes { get { return _onActionComplets; } }
}
