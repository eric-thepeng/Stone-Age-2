using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest Info", menuName = "ScriptableObjects/Narrative/QuestInfo")]
public class QuestInfo : ScriptableObject
{
    [SerializeField] string questName;
    [SerializeField] string questDescription;
    [SerializeField] string questType;
}
