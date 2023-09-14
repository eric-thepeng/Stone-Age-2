using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class UI_ISOIconDisplayBox : MonoBehaviour
{
    private SpriteRenderer sr;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Display(ItemScriptableObject iso)
    {
        sr.sprite = iso.iconSprite;
    }
}
