using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Narrative Sequence Scriptable Object", menuName = "ScriptableObjects/Narrative/Narrative Sequence Scriptable Object")]
public class NarrativeSequenceScriptableObject : ScriptableObject
{
    public NarrativeSequence NS_English;
    public NarrativeSequence NS_Chinese;

    public NarrativeSequence GetLocalizedNarrativeSequence()
    {
        switch (LocalizationManager.currentLanguage)
        {
            case LocalizationManager.LanguageOption.English:
                return NS_English;
            case LocalizationManager.LanguageOption.Chinese:
                return NS_Chinese;
        }

        return NS_English;
    }
}
