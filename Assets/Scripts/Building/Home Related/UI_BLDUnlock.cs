using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_BLDUnlock : MonoBehaviour
{
    [SerializeField] private GameObject uiGameObject;
    [SerializeField] private GameObject objectGameObject;
    [SerializeField] ResourceSetDisplayer costDisplayer, gainDisplayer;
    private ResourceSet costResourceSet, gainResourceSet;
    [SerializeField] TextMeshPro progressText;

    public void PassResourceSet(ResourceSet cost, ResourceSet gain)
    {
        costResourceSet = cost;
        gainResourceSet = gain;
        costDisplayer.Display(costResourceSet);
        gainDisplayer.Display(gainResourceSet);
    }

    public void SetProgress(float percent)
    {
        progressText.text = (int)(percent * 100) + "%";
    }

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
