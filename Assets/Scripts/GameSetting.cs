using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    public enum LanguageOption
    {
        English,
        Chinese
    }

    public static LanguageOption language = LanguageOption.English;
}
