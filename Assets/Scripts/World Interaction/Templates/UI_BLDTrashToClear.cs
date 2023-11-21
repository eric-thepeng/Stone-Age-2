using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_BLDTrashToClear : MonoBehaviour
{
    [SerializeField] private GameObject uiGameObject;
    [SerializeField] private GameObject objectGameObject;

    public void TurnOnUI()
    {
        uiGameObject.SetActive(true);
    }

    public void TurnOffUI()
    {
        uiGameObject.SetActive(false);
    }

    public void TurnOnHighlight()
    {
        
    }

    public void TurnOffHighlight()
    {
        
    }

}
