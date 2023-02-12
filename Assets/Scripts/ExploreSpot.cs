using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreSpot : MonoBehaviour
{
    public string spotName;
    public int spiritPoint = 1;
    public string[] resource = new string[0];
    public int[] weight = new int[0];
    public int totalWeight;
    public int gatherTime;

    public Color32 unlockedColor;
    public Color32 lockedColor;

    public int unlockSpiritPoint = 0;
    public string[] unlockResource = new string[0];
    public string[] unlockResrouceAmount = new string[0];

    public bool unlocked = false;

    private void Start()
    {
        if (unlocked)
        {
            GetComponent<SpriteRenderer>().color = unlockedColor;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = lockedColor;
        }
        totalWeight = 0;
        foreach(int i in weight)
        {
            totalWeight += i;
        }
    }

    public void PlaceCharacter(Sprite sp)
    {
        transform.Find("Character Sprite").GetComponent<SpriteRenderer>().sprite = sp;
    }

    public void EndGathering()
    {
        transform.Find("Character Sprite").GetComponent<SpriteRenderer>().sprite = null;
    }

    public string GetDisplayInfo()
    {
        string text = spotName + "<br> <br>";
        if (unlocked)
        {
            text += "Gather reward: " + spiritPoint + " Spirit Points<br>";
            if (resource.Length == 0) return text;
            text += "Possible resources: <br>";
            foreach (string s in resource)
            {
                text += "    " +  s + "<br>";
            }
        }
        else
        {
            text += "Unlock Cost: " + unlockSpiritPoint + " Spirit Points<br>";
            if (unlockResource.Length == 0) return text;
            text += "And: <br>";
            for(int i =0; i<unlockResource.Length; i++)
            {
                text += "    " + unlockResrouceAmount[i] + "x" + unlockResource[i] + "<br>";
            }
        }
        return text;
    }
}
