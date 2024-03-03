using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalizableTextMeshPro : LocalizationAction
{
    public TextMeshPro targetTMP = null;

    public bool changeFontOnly = false;

    private float orgFontSize = -1;
    
    public Dictionary<LocalizationManager.LanguageOption, string> data = new Dictionary<LocalizationManager.LanguageOption, string>()
    {
        { LocalizationManager.LanguageOption.Chinese, "" },
        { LocalizationManager.LanguageOption.English, "" }
    };
    
    public override void Localize(LocalizationManager.LanguageOption targetLanguage)
    {
        if (targetTMP == null)
        {
            targetTMP = GetComponent<TextMeshPro>();
        }

        if (targetTMP == null)
        {
            Debug.LogError("No target TMP assigned on game object: " + gameObject.name);
        }
        
        //FIRST TIME
        if (orgFontSize == -1) orgFontSize = targetTMP.fontSize;
        //END
        
        if (!changeFontOnly)
        {
            targetTMP.text = data[targetLanguage];
        }

        LocalizationLanguageData targetLLD = LocalizationManager.i.GetLocalizationLanguageData(targetLanguage) ;
        if(targetLLD == null) Debug.LogError("NO SUCH LANGUAGE");

        targetTMP.font = targetLLD.tmpFontAsset;
        targetTMP.UpdateFontAsset();
        targetTMP.fontSize = orgFontSize * targetLLD.fontSizeRatio;
    }
}
