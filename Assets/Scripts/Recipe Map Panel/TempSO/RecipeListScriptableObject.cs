using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeListScriptableObject", menuName = "ScriptableObjects/RecipeListScriptableObject")]
public class RecipeListScriptableObject : SerializedScriptableObject
{
    public List<ItemCraftScriptableObject> list = new List<ItemCraftScriptableObject>();
}
