using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Serial Effect Identifier", menuName = "ScriptableObjects/WorldInteraction/SerialEffectIdentifier")]
public class SO_SerialEffectIdentifier : ScriptableObject
{
    public void SendSerialEffect()
    {
        SO_SerialEffectIdentifierGroup[] allSEIGroup = Resources.LoadAll<SO_SerialEffectIdentifierGroup>("folder path");

        foreach (SO_SerialEffectIdentifierGroup seiGroup in allSEIGroup)
        {
            foreach (SO_SerialEffectIdentifierGroup.Relationship relationship in seiGroup.allRelationships)
            {
                if (relationship.direction == SO_SerialEffectIdentifierGroup.Relationship.Direction.OneWay)
                {
                    if (relationship.FirstUnit == this) { relationship.SecondUnit.SendSerialEffect(); return; }
                }
                else
                {
                    if (relationship.FirstUnit == this) { relationship.SecondUnit.SendSerialEffect(); return; }
                    if (relationship.SecondUnit == this) { relationship.FirstUnit.SendSerialEffect(); return; }
                }
            }
        }
    }   
}
