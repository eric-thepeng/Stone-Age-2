using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


/*
 * ItemScriptableObject represents the grid formulation of a single Tetris Game Obejct.
*/

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "ScriptableObjects/CraftingSystem/ItemScriptableObject")]
public class ItemScriptableObject : SerializedScriptableObject
{
    public string tetrisHoverName = "not set";
    [SerializeField] private bool[,] recipeInBool = new bool[8,8];
    public Sprite tetrisSprite;
    public Sprite iconSprite;
    public string tetrisDescription = "not set";
    public string tetrisSideNote = "not set";
    [Header("--- DO NOT EDIT BELOW ---")]public GameObject myPrefab = null; 
    public bool isCraftingStation = false;
    public enum Category { Material, Building }
    public Category category = Category.Material;

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
    
    public int TetrisUnitAmount
    {
        get
        {
            int export = 0;

            for (int x = 0; x < recipeInBool.GetLength(0); x++)
            {
                for (int y = 0; y < recipeInBool.GetLength(1); y++)
                {
                    // Check if the value at the current coordinate is true
                    if (recipeInBool[x, y])
                    {
                        // Add the coordinate to the list
                        export++;
                    }
                }
            }
            
            return export;
        }
    }
    
    /// <summary>
    /// The max x and y dimension (bottom-right most unit) of the formation coord 
    /// </summary>
    public Vector2Int Dimension
    {
        get
        {
            Vector2Int botRight = new Vector2Int(0, 0);
            foreach (var VARIABLE in HomogeneousCoord)
            {
                if (VARIABLE.x > botRight.x) botRight.x = VARIABLE.x;
                if (VARIABLE.y > botRight.y) botRight.y = VARIABLE.y;
            }
            return botRight + new Vector2Int(1, 1); // plus 1,1 because coords start at 0,0
        }
    }

    
    /// <summary>
    /// </summary>
    /// <returns>The difference between the Tetris' bottom-right boundary and its center.</returns>
    public Vector2 GetBottomRightDelta()
    {
        /*
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
        return botRight/2;*/

        Vector2 delta = Dimension - new Vector2Int(1, 1);
        delta /= 2;
        return new Vector2(delta.x, -delta.y); // -y because coord is positive but delta is negative
    }

    public override string ToString()
    {
        return tetrisHoverName;
    }
}
