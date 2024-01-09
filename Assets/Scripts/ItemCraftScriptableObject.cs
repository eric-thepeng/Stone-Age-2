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
    public List<Recipe> allRecipes = new List<Recipe>();
    public Dictionary<ItemScriptableObject, int> craftingStationRequired = null; // new Dictionary<CraftingStationScriptableObject, int> ();

    public enum BlueprintState
    {
        Not_Obtained,
        Obtained_Not_Researched,
        Obtained_Researched
    }

    public BlueprintState blueprintState = BlueprintState.Not_Obtained;

    public void ChangeBlueprintState(BlueprintState changeTo)
    {
        blueprintState = changeTo;
    }
    
    public bool IsObtained()
    {
        return blueprintState == BlueprintState.Obtained_Researched ||
               blueprintState == BlueprintState.Obtained_Not_Researched;
    }

    public bool IsResearched()
    {
        return blueprintState == BlueprintState.Obtained_Researched;
    }

    //Iterate all recipes, return true if one of them matches.
    public bool CheckMatch(List<KeyValuePair<Vector2, ItemScriptableObject>> currentRecipeCord, ObjectStackList<ItemScriptableObject> currentCraftingStations)
    {
        bool hasRecipe = false;

        foreach (Recipe r in allRecipes)
        {
            if (r.CheckMatch(currentRecipeCord)) hasRecipe = true;
        }

        if(!hasRecipe) return false;

        if (craftingStationRequired == null) return true;

        foreach (KeyValuePair<ItemScriptableObject, int> kvp in craftingStationRequired)
        {
            if(currentCraftingStations.GetAmount(kvp.Key) < kvp.Value)
            {
                return false;
            }
        }

        return true;
    }

    public List<Vector2> GetDefaultRecipeCoords()
    {
        Recipe defaultRecipe = allRecipes[0];
        return defaultRecipe.getCoord(true);
    }

    public ResourceSet GetResourceSet()
    {
        return allRecipes[0].GetRecipeResourceSet();
    }

    /*
    public Dictionary<ItemScriptableObject, int> GetRecipeComposition()
    {
        return allRecipes[0].GetRecipeComposition();
    }

    public ResourceSet GetRecipeResourceSet()
    {
        List<ResourceSet.ResourceAmount> reourceAmountsList = new List<ResourceSet.ResourceAmount>();
        foreach (var VARIABLE in GetRecipeComposition())
        {
            reourceAmountsList.Add(new ResourceSet.ResourceAmount( VARIABLE.Key,VARIABLE.Value));
        }
        ResourceSet export = new ResourceSet(0, reourceAmountsList);
        return export;
    }*/
    
}
