using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class UI_ISOIconDisplayBox : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private int index;
    [SerializeField] private bool receiveDrop = true;
    [SerializeField] private bool clickToCancel = true;
    [SerializeField] private GameObject textAmount;
    
    [Header("Leaves below empty if not notifying anything")]
    public MonoBehaviour monoBehaviourWithIISOReceiver;
    
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Display(ItemScriptableObject iso, bool triggerByDroppingISOIcon, int amount = -1, bool sendNotification = true)
    {
        //Check for if dropping iso icon is allowed
        if(!receiveDrop && triggerByDroppingISOIcon) return;
        
        //Display Amount
        DisplayAmount(amount != -1, amount);
        
        //Display Sprite
        if (iso == null)
        {
            Clear();
        }
        else
        {
            sr.sprite = iso.iconSprite;
        }
        
        //Notify receiver
        if (monoBehaviourWithIISOReceiver != null && sendNotification)
        {
            ((IISOReceiver)monoBehaviourWithIISOReceiver).ReceiveISOWithIndex(iso, index);
        }

    }

    public void DisplayAmount(bool display, int amount)
    {
        DisplayAmount(display);
        textAmount.GetComponent<TextMeshPro>().text = "" + amount;
    }

    public void DisplayAmount(bool display)
    {
        if (display)
        {
            textAmount.SetActive(true);
        }
        else
        {
            textAmount.SetActive(false);
        }
    }

    public void Clear()
    {
        sr.sprite = null;
        DisplayAmount(false,0);
    }

    private void OnMouseUpAsButton()
    {
        if(!clickToCancel) return;
        Clear();
        if(monoBehaviourWithIISOReceiver != null) ((IISOReceiver)monoBehaviourWithIISOReceiver).CancelISO(index);
    }
}
