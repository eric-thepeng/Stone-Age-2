using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class UI_ISOIconDisplayBox : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private int index;
    public MonoBehaviour monoBehaviourWithIISOReceiver;
    public enum ClickEffect{Reset}
    
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Display(ItemScriptableObject iso)
    {
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
        Clear();
        if(monoBehaviourWithIISOReceiver != null) ((IISOReceiver)monoBehaviourWithIISOReceiver).CancelISO(index);
    }
}
