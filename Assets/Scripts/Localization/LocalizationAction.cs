using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LocalizationAction : SerializedMonoBehaviour
{
    public virtual void Localize(LocalizationManager.LanguageOption targetLanguage)
    {
        throw new NotImplementedException();
    }

    private void OnEnable()
    {
        Localize(LocalizationManager.currentLanguage);
    }
}
