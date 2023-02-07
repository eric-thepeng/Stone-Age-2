using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GroundRecipeScriptableObject", menuName = "ScriptableObjects/Ground Recipe SO")]
public class GroundRecipeScriptableObject : SerializedScriptableObject
{

    public Dictionary<ItemScriptableObject, ItemScriptableObject> groundRecipe = new Dictionary<ItemScriptableObject, ItemScriptableObject>();

    public bool CanGround(ItemScriptableObject from)
    {
        return groundRecipe[from] != null;
    }

    public ItemScriptableObject Ground(ItemScriptableObject from)
    {
        if (groundRecipe.ContainsKey(from))
        {
            return groundRecipe[from];
        }
        return null;
    }
}
