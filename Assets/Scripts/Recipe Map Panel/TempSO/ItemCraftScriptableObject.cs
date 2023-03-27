using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*
 * ItemScriptableObject represents the grid formulation of a single Tetris Game Obejct. It is a template for
*/

[CreateAssetMenu(fileName = "ItemCraftRecipeScriptableObject", menuName = "ScriptableObjects/CraftingSystem/ItemCraftRecipeScriptableObject")]
public class ItemCraftScriptableObject : SerializedScriptableObject
{
    public ItemScriptableObject ItemCrafted;
    //public ItemScriptableObject CraftingStationRequired; //TODO: delete this
    public List<Recipe> allRecipes = new List<Recipe>();
    public Dictionary<CraftingStationScriptableObject, int> craftingStationRequired = null; // new Dictionary<CraftingStationScriptableObject, int> ();

    //Iterate all recipes, return true if one of them matches.
    public bool CheckMatch(List<KeyValuePair<Vector2, ItemScriptableObject>> toCheck)
    {
        foreach (Recipe r in allRecipes)
        {
            if (r.CheckMatch(toCheck)) return true;
        }
        return false;
    }
    
}
