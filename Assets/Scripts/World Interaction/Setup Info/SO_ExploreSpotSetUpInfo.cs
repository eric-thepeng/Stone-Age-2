using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ExploreSpotSetUpInfo", menuName = "ScriptableObjects/WorldInteraction/Explore Spot Set Up Information")]
public class SO_ExploreSpotSetUpInfo : ScriptableObject
{
    public SO_SerialEffectIdentifier serialEffectIdentifier;
    public ResourceSet unlockResourceSet;
    public int gatherTime = 5;
    public ResourceSet gatherResource;
    public bool startInLockedState = false;
}
