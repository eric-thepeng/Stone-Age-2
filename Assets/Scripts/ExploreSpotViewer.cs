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

    private void Start()
    {
        DisplayGO = transform.Find("Explore Spot Viewer").gameObject;
    }

    public void DisplayExploreSpot(ExploreSpot es)
    {
        DisplayGO.transform.position = WorldUtility.GetMouseHitPoint(9, true);
        DisplayGO.GetComponentInChildren<TextMeshPro>().text = es.GetDisplayInfo();
        DisplayGO.SetActive(true);
    }

    public void CancelDisplay()
    {
        DisplayGO.SetActive(false);
    }


}
