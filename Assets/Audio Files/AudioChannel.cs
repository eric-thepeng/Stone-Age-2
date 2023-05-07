using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioChannel : MonoBehaviour
{
    static AudioChannel instance = null;
    public static AudioChannel i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioChannel>();
            }
            return instance;
        }
    }

    void Start()
    {
        JSAM.AudioManager.PlayMusic(JSAM.MusicStoneAge2.Music_01);
    }

    public void PlayButtonSound(string buttonID)
    {
        if (buttonID == "CraftingPanelOpenButton")
        {
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Button_Click_01);
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Menu_Open_01);
        }
        else if (buttonID == "RecipeMapOpenButton")
        {
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Button_Click_01);
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Menu_Open_01);
        }
        else if (buttonID == "RecipeViewerPanelOpenButton")
        {
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Button_Click_01);
        }
        else if (buttonID == "BuildingSystemOpenButton")
        {
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Button_Click_01);
        }
        else if (buttonID == "CameraBackHomeButton")
        {
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Button_Click_01);
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Menu_Open_01);
        }
        else if (buttonID == "HomeReturnButton")
        {
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Button_Click_01);
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Menu_Open_01);
        }
        else if (buttonID == "InventoryPanelOpenButton")
        {
            JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Button_Click_01);
        }
    }
}
