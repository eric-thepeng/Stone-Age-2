using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Serial Effect Identifier Group", menuName = "ScriptableObjects/WorldInteraction/SerialEffectIdentifierGroup")]
public class SO_SerialEffectIdentifierGroup : ScriptableObject
{
    [Serializable]
    public class Relationship
    {
        public enum Direction {OneWay, BothWay}
        public SO_SerialEffectIdentifier FirstUnit;
        public SO_SerialEffectIdentifier SecondUnit;
        public Direction direction;
    }
    public List<Relationship> allRelationships;
}
