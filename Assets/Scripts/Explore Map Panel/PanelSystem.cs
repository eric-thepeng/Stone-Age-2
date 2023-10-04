using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSystem : MonoBehaviour
{
    [SerializeField]private GameObject panelBody;
    private bool isPanelOpen = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(isPanelOpen) ClosePanel();
            else OpenPanel();
        }
    }

    public void OpenPanel()
    {
        panelBody.transform.localPosition = new Vector3(0, 0, 30);
        isPanelOpen = true;
    }

    public void ClosePanel()
    {
        panelBody.transform.localPosition = new Vector3(30, 0, 30);
        isPanelOpen = false;
    }
}
