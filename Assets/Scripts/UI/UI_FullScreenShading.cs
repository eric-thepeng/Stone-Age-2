using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FullScreenShading : MonoBehaviour
{
    static UI_FullScreenShading instance;
    public static UI_FullScreenShading i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_FullScreenShading>();
            }
            return instance;
        }
    }
    
    [SerializeField]private GameObject dialogueShading;
    [SerializeField]private GameObject workshopShading;
    private List<GameObject> allShadings;

    private void Awake()
    {
        allShadings = new List<GameObject>(){dialogueShading, workshopShading};
    }

    public void ShowDialogueShading()
    {
        dialogueShading.SetActive(true);
    }

    public void ShowWorkshopShading()
    {
        workshopShading.SetActive(true);
    }

    public void HideShading()
    {
        foreach (var shading in allShadings)
        {
            shading.SetActive(false);
        }
    }
    
    
}
