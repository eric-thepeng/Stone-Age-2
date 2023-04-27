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
        DisplayGO.transform.position = WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.UI_BACKGROUND, true);
        RefreshDisplayInfo(es);
        
    }

    private void RefreshDisplayInfo(ExploreSpot es)
    {
        DisplayGO.SetActive(true);
        //DisplayGO.GetComponentInChildren<TextMeshPro>().text = DisplayingES.GetDisplayInfo();
        if (DisplayingES.isCanUnlock())
        {
            DisplayGO.transform.Find("Unlock Button").gameObject.SetActive(true);
        }
        else
        {
            DisplayGO.transform.Find("Unlock Button").gameObject.SetActive(false);
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
            DisplayingES.SetLockState(ExploreSpot.LockState.UNLOCKED);
            RefreshDisplayInfo(DisplayingES);
        }
        else
        {
            Debug.Log("Do not have enough resource");
        }
    }


}
