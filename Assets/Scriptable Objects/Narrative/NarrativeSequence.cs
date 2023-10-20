using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "NarrativeSequence", menuName = "ScriptableObjects/Narrative/NarrativeSequence")]
[Serializable]public class NarrativeSequence// : SerializedScriptableObject
{
    public List<string> data = new List<string>();
    public string GetLine(int index) { return data[index]; }
    public int LineCount() { return data.Count; }
    public bool HasLine(int index) { return index < LineCount(); }
}
