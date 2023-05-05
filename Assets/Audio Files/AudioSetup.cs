using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSetup : MonoBehaviour
{
    void Start()
    {
        JSAM.AudioManager.PlayMusic(JSAM.MusicStoneAge2.Music_01);
    }
}
