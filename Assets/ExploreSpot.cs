using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreSpot : MonoBehaviour
{

    public int spiritPoint = 1;
    public string[] resource = new string[0];
    public int[] weight = new int[0];
    public int totalWeight;
    public int gatherTime;

    private void Start()
    {
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
}
