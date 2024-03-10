using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using UnityEngine;

public class csvManager : MonoBehaviour
{
    private void Awake()
    {
        LocalizationManager.Read();
        LocalizationManager.Language = "BASIC DESCRIPTION";
        Debug.Log("okk" + LocalizationManager.Localize("Egg Fried Rice"));
    }
}
