using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NarrativeSequencesList", menuName = "ScriptableObjects/Narrative/NarrativeSequencesList")]
public class NarrativeSequenceList : SerializedScriptableObject
{
    public List<NarrativeSequence> data = new List<NarrativeSequence>();
}
