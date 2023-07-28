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
    [SerializeField] private bool[,] recipeInBool = new bool[8,8];
    public Sprite tetrisSprite;
    public Sprite iconSprite;
    public bool isCraftingStation = false;
    public enum Category { Regular, Building }
    public Category category = Category.Regular;

    public List<KeyValuePair<Vector2, ItemScriptableObject>> FormationRecipeCoord
    {
        get
        {
            List<KeyValuePair<Vector2, ItemScriptableObject>> export =
                new List<KeyValuePair<Vector2, ItemScriptableObject>>();

            for (int x = 0; x < recipeInBool.GetLength(0); x++)
            {
                for (int y = 0; y < recipeInBool.GetLength(1); y++)
                {
                    // Check if the value at the current coordinate is true
                    if (recipeInBool[x, y])
                    {
                        // Add the coordinate to the list
                        export.Add(new KeyValuePair<Vector2, ItemScriptableObject>(new Vector2(x, y),this));
                    }
                }
            }
            
            return export;
        }
    }
    
    public List<Vector2Int> HomogeneousCoord
    {
        get
        {
            List<Vector2Int> export = new List<Vector2Int>();

            for (int x = 0; x < recipeInBool.GetLength(0); x++)
            {
                for (int y = 0; y < recipeInBool.GetLength(1); y++)
                {
                    // Check if the value at the current coordinate is true
                    if (recipeInBool[x, y])
                    {
                        // Add the coordinate to the list
                        export.Add(new Vector2Int(x, y));
                    }
                }
            }
            
            return export;
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
