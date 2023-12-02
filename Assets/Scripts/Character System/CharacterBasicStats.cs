using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Basic Stats", menuName = "ScriptableObjects/CharacterSystem/CharacterBasicStats")]
public class CharacterBasicStats : ScriptableObject
{
    public string name;

    public GameObject l2dGameObject;

    public int energy;

    public float restingEnergyPercentage;

    public float gatherSpeed;
    public float exploreSpeed;
    public float restingSpeed;
    public int herdSize;
    
    public override string ToString()
    {
        return "Basic Stats of " + name + " is [energy " + energy + " ] [gather speed " + gatherSpeed + " ] [explore speed " + exploreSpeed + "]";
    }
}
