using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UI_DropReceiveISOIconDisplayBox : UI_ISOIconDisplayBox
{
    public void Display(ItemScriptableObject iso)
    {
        GetComponent<SpriteRenderer>().sprite = iso.iconSprite;
    }
}
