using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    static LocalizationManager instance;
    public static LocalizationManager i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LocalizationManager>();
            }
            return instance;
        }
    }
    
    public List<LocalizationLanguageData> allLanguageData;
    
    public enum LanguageOption
    {
        English,
        Chinese,
        Russian,
        Spanish,
        French,
    }

    public static LanguageOption currentLanguage = LanguageOption.English;


    public static void SwitchLanguage(LanguageOption targetLanguage)
    {
        foreach (var VARIABLE in FindObjectsOfType<LocalizableTextMeshPro>())
        {
            VARIABLE.Localize(targetLanguage);
        }

        currentLanguage = targetLanguage;
    }

    public LocalizationLanguageData GetLocalizationLanguageData(LanguageOption targetOption)
    {
        foreach (var VARIABLE in allLanguageData)
        {
            if (VARIABLE.language == targetOption) return VARIABLE;
        }

        return null;
    }
}

[Serializable]
public class LocalizationLanguageData
{
    public LocalizationManager.LanguageOption language;
    public TMP_FontAsset tmpFontAsset;
    public float fontSizeRatio;
}
