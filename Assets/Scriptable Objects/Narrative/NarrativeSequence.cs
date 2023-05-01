using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NarrativeSequence", menuName = "ScriptableObjects/Narrative/NarrativeSequence")]
public class NarrativeSequence : SerializedScriptableObject
{
    public string narrativeSequenceID = "";
    public enum Character {Bird}
    public Character character = Character.Bird;
    public List<string> data = new List<string>();
    public string questIDToQue = "No Quest";
    public string GetLine(int index) { return data[index]; }
    public int LineCount() { return data.Count; }
    public bool HasLine(int index) { return index < LineCount(); }
    public bool HasQuest() { return !questIDToQue.Equals("No Quest"); }
}
