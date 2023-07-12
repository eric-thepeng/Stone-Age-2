using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Basic Stats", menuName = "ScriptableObjects/CharacterSystem/CharacterBasicStats")]
public class CharacterBasicStats : ScriptableObject
{
    string name;
    float energy;
    float gatherSpeed;
    float exploreSpeed;

    public override string ToString()
    {
        return "Basic Stats of " + name + " is [energy " + energy + " ] [gather speed " + gatherSpeed + " ] [explore speed " + exploreSpeed + "]";
    }
}
