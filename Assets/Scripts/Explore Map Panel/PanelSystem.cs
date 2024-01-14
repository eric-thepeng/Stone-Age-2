using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSystem : MonoBehaviour
{
    [SerializeField]private GameObject panelBody;
    private bool isPanelOpen = false;

    public void OpenPanel()
    {
        panelBody.transform.localPosition -= new Vector3(70, 0, 0);
        isPanelOpen = true;
    }

    public void ClosePanel()
    {
        panelBody.transform.localPosition += new Vector3(70, 0, 0);
        isPanelOpen = false;
    }
}
