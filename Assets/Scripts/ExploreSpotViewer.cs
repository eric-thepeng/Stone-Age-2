using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExploreSpotViewer : MonoBehaviour
{
    static ExploreSpotViewer instance;
    public static ExploreSpotViewer i
    {
        get {
            if(instance == null)
            {
                instance = FindObjectOfType<ExploreSpotViewer>();
            }
            return instance; 
        }
    }

    GameObject DisplayGO;
    ExploreSpot DisplayingES;

    private void Start()
    {
        DisplayGO = transform.Find("Explore Spot Viewer").gameObject;
    }

    public void DisplayExploreSpot(ExploreSpot es)
    {
        DisplayingES= es; 
        DisplayGO.transform.position = WorldUtility.GetMouseHitPoint(9, true);
        RefreshDisplayInfo(es);
        
    }

    private void RefreshDisplayInfo(ExploreSpot es)
    {
        DisplayGO.GetComponentInChildren<TextMeshPro>().text = DisplayingES.GetDisplayInfo();
        DisplayGO.SetActive(true);
        if (DisplayingES.isUnlocked())
        {
            DisplayGO.transform.Find("Unlock Button").gameObject.SetActive(false);
        }
        else
        {
            DisplayGO.transform.Find("Unlock Button").gameObject.SetActive(true);
        }
    }

    public void CancelDisplay()
    {
        DisplayGO.SetActive(false);
    }

    public void UnlockSpot()
    {
        if(SpiritPoint.i.Use(DisplayingES.unlockSpiritPoint) == true)
        {
            DisplayingES.Unlock();
            RefreshDisplayInfo(DisplayingES);
        }
        else
        {
            Debug.Log("Do not have enough resource");
        }
    }


}
