using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GameSetting : SerializedMonoBehaviour
{
    static GameSetting instance;
    public static GameSetting i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameSetting>();
            }
            return instance;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            LocalizationManager.SwitchLanguage(LocalizationManager.LanguageOption.English);
        }else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            LocalizationManager.SwitchLanguage(LocalizationManager.LanguageOption.Chinese);
        }
    }
}
