using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Task Info", menuName = "ScriptableObjects/TaskInfo")]
public class TaskInfo : SerializedScriptableObject
{
    public enum Category {Jessie, Adam, Julie, Smith, Charles }

    public Category category;
    public string taskName;
    public string descriptionText;
    public Dictionary<ItemScriptableObject, int> request;
    /// <summary>
    /// 0: not assigned 1: assigned 2: completed 3: discarded
    /// </summary>
    public int completion;
    public TaskInfo[] unlocks;
}
