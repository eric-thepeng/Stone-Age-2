using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * ItemScriptableObject represents the grid formulation of a single Tetris Game Obejct. It is a template for
*/

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "ScriptableObjects/CraftingSystem/ItemScriptableObject")]
public class ItemScriptableObject : SerializedScriptableObject
{
    public string tetrisHoverName = "not set";
    public GameObject myPrefab = null; 
    //public List<Recipe> allRecipes = new List<Recipe>();
    public Recipe myRecipe = null;
    public Sprite tetrisSprite;
    public Sprite iconSprite;
    public enum Category { Regular, Building }
    public Category category = Category.Regular;

    public List<KeyValuePair<Vector2, ItemScriptableObject>> FormationRecipeCoord
    {
        get
        {
            return myRecipe.getCoordForm();
        }
    }

    //Each Recipe class contains one recipe, can be accessed by methods.
   
    public Dictionary<ItemScriptableObject, int> GetIngredients()
    {
        Dictionary<ItemScriptableObject, int> ingredients = new Dictionary<ItemScriptableObject, int>();
        return ingredients;
    }

    public Vector2 GetBottomRightDelta()
    {
        Vector2 botRight = new Vector2(0, 0);
        bool first = true;
        foreach( KeyValuePair < Vector2, ItemScriptableObject> kvp in FormationRecipeCoord)
        {
            if (first)
            {
                botRight = kvp.Key;
                first = false;
            }
            if(kvp.Key.x > botRight.x) botRight.x = kvp.Key.x;
            if (kvp.Key.y > botRight.y) botRight.y = kvp.Key.y;
        }
        botRight = new Vector2(botRight.x, -botRight.y);
        return botRight/2;
    }

    public override string ToString()
    {
        return tetrisHoverName;
    }
}
