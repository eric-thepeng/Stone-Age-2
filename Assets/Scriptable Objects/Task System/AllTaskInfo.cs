using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "All Task Info", menuName = "ScriptableObjects/AllTaskInfo")]
public class AllTaskInfo : SerializedScriptableObject
{
    public List<TaskInfo> info;
}
