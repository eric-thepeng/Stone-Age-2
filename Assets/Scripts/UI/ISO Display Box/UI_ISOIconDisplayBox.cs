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
    
    public MonoBehaviour monoBehaviourWithIISOReceiver;
    
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Display(ItemScriptableObject iso, bool triggerByDroppingISOIcon, int amount = -1)
    {
        if(!receiveDrop && triggerByDroppingISOIcon) return;
        if (amount == -1)
        {
            textAmount.SetActive(false);
        }
        else
        {
            textAmount.SetActive(true);
            textAmount.GetComponent<TextMeshPro>().text = "" + amount;
        }
        if (iso == null)
        {
            Clear();
        }
        else
        {
            sr.sprite = iso.iconSprite;
            if(monoBehaviourWithIISOReceiver != null) ((IISOReceiver)monoBehaviourWithIISOReceiver).ReceiveISOWithIndex(iso, index);
        }
    }

    public void Clear()
    {
        sr.sprite = null;
    }

    private void OnMouseUpAsButton()
    {
        if(!clickToCancel) return;
        Clear();
        if(monoBehaviourWithIISOReceiver != null) ((IISOReceiver)monoBehaviourWithIISOReceiver).CancelISO(index);
    }
}
