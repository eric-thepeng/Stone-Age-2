using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class UI_BLDWorkshop : MonoBehaviour
{
    [SerializeField] private GameObject uiGameObject = null;
    private BLDWorkshop workshop;

    private void Start()
    {
        workshop = GetComponent<BLDWorkshop>();
    }

    public void TurnOnUI()
    {
        uiGameObject.SetActive(true);
    }

    public void TurnOffUI()
    {
        uiGameObject.SetActive(false);
    }

    public void ExitButtonClicked()
    {
        TurnOffUI();
        workshop.ExitUI();
    }
}
