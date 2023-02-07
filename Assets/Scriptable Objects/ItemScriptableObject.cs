using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*
 * ItemScriptableObject represents the grid formulation of a single Tetris Game Obejct. It is a template for
*/

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "ScriptableObjects/ItemScriptableObject")]
public class ItemScriptableObject : SerializedScriptableObject
{
    public string tetrisHoverName = "not set";
    public bool isGround = false, isCook = false;
    public GameObject myPrefab = null; //For ITEM specifically (homogeneous)
    public List<Recipe> allRecipes = new List<Recipe>();
    public Sprite objectSprite;
    public Sprite tetrisSprite;
    public List<KeyValuePair<Vector2, ScriptableObject>> FormationRecipeCoord
    {
        get
        {
            if(allRecipes.Count > 0) return allRecipes[0].getCoordForm();
            return null;
        }
    }

    //Iterate all recipes, return true if one of them matches.
    public bool CheckMatch(List<KeyValuePair<Vector2, ScriptableObject>> toCheck)
    {
        foreach (Recipe r in allRecipes)
        {
            if (r.CheckMatch(toCheck)) return true;
        }
        return false;
    }

    //Each Recipe class contains one recipe, can be accessed by methods.
    public class Recipe
    {
        //The content of the Recipe
        //[TableMatrix(HorizontalTitle = "Recipe Matrix", SquareCells = false)
        public ScriptableObject[,] recipe = new ScriptableObject[8, 6];

        //Get CoordForm of Recipe, the only form accessible.
        public List<KeyValuePair<Vector2, ScriptableObject>> getCoordForm()
        {
            List<KeyValuePair<Vector2, ScriptableObject>> export = new List<KeyValuePair<Vector2, ScriptableObject>>();
            for (int x = 0; x < recipe.GetLength(0); x++)
            {
                for (int y = 0; y < recipe.GetLength(1); y++)
                {
                    if (recipe[x, y] == null) continue;
                    export.Add(new KeyValuePair<Vector2, ScriptableObject>(new Vector2(x, y), recipe[x,y]));
                }
            }
            return export;
        }

        //Compare CoordForm and self-CoordForm
        public bool CheckMatch(List<KeyValuePair<Vector2, ScriptableObject>> toCheck)
        {
            List<KeyValuePair<Vector2, ScriptableObject>> toMatch = getCoordForm();
            if (toCheck.Count != toMatch.Count) return false;
            foreach(KeyValuePair<Vector2, ScriptableObject> kvp in toCheck)
            {
                if (!toMatch.Contains(kvp)) return false;
            }
            return true;
        }

        //Compare two Recipe by CoordForm
        public bool Equals(Recipe toCheck)
        {
            if (CheckMatch(toCheck.getCoordForm())) return true;
            return false;
        }
    }
}
