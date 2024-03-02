using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Basic Stats", menuName = "ScriptableObjects/CharacterSystem/CharacterBasicStats")]
public class CharacterBasicStats : ScriptableObject
{
    public string name;


    public int energy;
    public float restingEnergyPercentage;
    
    public int saturation;
    public float restingSaturationPercentage;

    public float gatherSpeed;
    public float exploreSpeed;
    public float restingSpeed;
    public int herdSize;

    [Header("Icon")]
    public Sprite iconSprite;

    [Header("Home Related")]
    public GameObject l2dGameObject;
    public int moveSpeed;
    public float hangOutWaitTime;

    [Header("Interaction (Rua) Related")]
    public int maxClicks = 5;
    public int pointsToAdd = 1;
    public float clickInterval = 0.1f;
    public float countdownTime = 10;
    
    public List<BuildingISO> favoriteBuildings;

    public override string ToString()
    {
        return "Basic Stats of " + name + " is [energy " + energy + " ] [gather speed " + gatherSpeed + " ] [explore speed " + exploreSpeed + "]";
    }

    public bool containsFavoriteBuilding(BuildingISO building)
    {
        Debug.Log(building);
        return favoriteBuildings.Contains(building);
    }
}
