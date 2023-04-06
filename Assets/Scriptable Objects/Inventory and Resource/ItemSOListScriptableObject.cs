using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSOListScriptableObject", menuName = "ScriptableObjects/ItemSOListScriptableObject")]
public class ItemSOListScriptableObject : SerializedScriptableObject
{
    public List<ItemScriptableObject> list = new List<ItemScriptableObject>();
}
