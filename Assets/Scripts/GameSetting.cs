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
    
    public enum LanguageOption
    {
        English,
        Chinese
    }

    public static LanguageOption language = LanguageOption.English;
    
    [SerializeField]private Dictionary<LanguageOption, TMP_FontAsset> fonts = new Dictionary<LanguageOption, TMP_FontAsset>()
    {
        {LanguageOption.Chinese, null },
        {LanguageOption.English, null }
    };

    public TMP_FontAsset GetFontAsset()
    {
        return fonts[language];
    }

    public void SwitchLanguage(LanguageOption targetLanguage)
    {
        language = targetLanguage;
        foreach (var VARIABLE in FindObjectsOfType<LocalizableTextMeshPro>())
        {
            VARIABLE.Localize(targetLanguage);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SwitchLanguage(LanguageOption.English);
        }else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SwitchLanguage(LanguageOption.Chinese);
        }
    }
}
