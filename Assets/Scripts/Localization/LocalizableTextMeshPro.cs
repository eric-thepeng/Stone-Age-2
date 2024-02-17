using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalizableTextMeshPro : LocalizationAction
{
    public TextMeshPro targetTMP = null;

    public bool changeFontOnly = false;
    
    public Dictionary<GameSetting.LanguageOption, string> data = new Dictionary<GameSetting.LanguageOption, string>()
    {
        { GameSetting.LanguageOption.Chinese, "" },
        { GameSetting.LanguageOption.English, "" }
    };
    
    public override void Localize(GameSetting.LanguageOption targetLanguage)
    {
        if (targetTMP == null)
        {
            targetTMP = GetComponent<TextMeshPro>();
        }

        if (targetTMP == null)
        {
            Debug.LogError("No target TMP assigned on game object: " + gameObject.name);
        }
        
        if (!changeFontOnly)
        {
            targetTMP.text = data[targetLanguage];
        }
        
        targetTMP.font = GameSetting.i.GetFontAsset();
        targetTMP.UpdateFontAsset();
    }
}
