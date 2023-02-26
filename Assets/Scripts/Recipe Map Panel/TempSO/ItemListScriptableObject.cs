using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemListScriptableObject", menuName = "ScriptableObjects/ItemListScriptableObject")]
public class ItemListScriptableObject : SerializedScriptableObject
{
    public List<ItemCraftScriptableObject> list = new List<ItemCraftScriptableObject>();
}
