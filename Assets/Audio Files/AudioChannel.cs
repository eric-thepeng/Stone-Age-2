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

    /*
    public Dictionary<string, JSAM.SoundsStoneAge2[]> soundNames = new Dictionary<string, JSAM.SoundsStoneAge2[]>()
    {
        {"CraftingPanelOpenButton",new JSAM.SoundsStoneAge2[2]{JSAM.SoundsStoneAge2.Button_Click_01, JSAM.SoundsStoneAge2.Menu_Open_01}},
        {"RecipeMapOpenButton",new JSAM.SoundsStoneAge2[2]{JSAM.SoundsStoneAge2.Button_Click_01, JSAM.SoundsStoneAge2.Menu_Open_01}},

    };
    */

    void Start()
    {
        JSAM.AudioManager.PlayMusic(JSAM.MusicStoneAge2.Music_01);
    }

    public void PlayButtonSound(string buttonID)
    {
        JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Button_Click_01);

        switch (buttonID)
        {
            case "CraftingPanelOpenButton":
                JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Menu_Craft_01);
                break;
            case "RecipeMapOpenButton":
                JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Menu_Recipe_01);
                break;
            case "RecipeViewerPanelOpenButton":
                JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Menu_Recipe_01);
                break;
            case "BuildingSystemOpenButton":
                JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Menu_Building_01);
                break;
            case "CameraBackHomeButton":
                JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Menu_Camera_01);
                break;
            case "HomeReturnButton":
                JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Menu_Camera_01);
                break;
            case "InventoryPanelOpenButton":
                JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Menu_Inventory_01);
                break;
            case "MergeOne":
                break;
            case "MergeAuto":
                break;
            case "CloseMergePreviewWindow":
                break;
        }
    }

    public void PlayCraftSound(string ItemName)
    {
        switch (ItemName)
        {
            case "Straw String":
                JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Crafting_03);
                break;
            case "Stone Axe":
                JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Crafting_02);
                break;
            case "Baked Apple":
                JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Crafting_01);
                break;
            case "Herb Baked Egg":
                JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Crafting_01);
                break;
            case "Camp Fire":
                JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Crafting_04);
                break;
            case "Smoked Perch":
                JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Crafting_01);
                break;
            case "Hay Wall":
                JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Crafting_03);
                break;
            case "Simple Fishing Net":
                JSAM.AudioManager.PlaySound(JSAM.SoundsStoneAge2.Crafting_02);
                break;
        }
    }
}
