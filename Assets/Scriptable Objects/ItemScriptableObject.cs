using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * ItemScriptableObject represents the grid formulation of a single Tetris Game Obejct.
*/

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "ScriptableObjects/CraftingSystem/ItemScriptableObject")]
public class ItemScriptableObject : SerializedScriptableObject
{
    public string tetrisHoverName = "not set";
    public GameObject myPrefab = null; 
    public Recipe myRecipe = null;
    public Sprite tetrisSprite;
    public Sprite iconSprite;
    public bool isCraftingStation = false;
    public enum Category { Regular, Building }
    public Category category = Category.Regular;

    public List<KeyValuePair<Vector2, ItemScriptableObject>> FormationRecipeCoord
    {
        get
        {
            return myRecipe.getCoordForm();
        }
    }

    /// <summary>
    /// </summary>
    /// <returns>The difference between the Tetris' bottom-right boundary and its center.</returns>
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
