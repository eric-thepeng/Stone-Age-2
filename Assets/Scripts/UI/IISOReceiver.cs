using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IISOReceiver
{
    public void ReceiveISO(ItemScriptableObject iso)
    {
     ReceiveISOWithIndex(iso, 0);   
    }

    public void ReceiveISOWithIndex(ItemScriptableObject iso, int index);

    public void CancelISO(int index);
}
