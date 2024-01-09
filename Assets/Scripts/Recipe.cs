using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe
{
    //The content of the Recipe
    //[TableMatrix(HorizontalTitle = "Recipe Matrix", SquareCells = false)
    public ItemScriptableObject[,] recipe = new ItemScriptableObject[8, 6];

    /// <summary>
    /// Get coordination representation of the recipe, the only accesible form.
    /// </summary>
    /// <returns></returns>
    public List<KeyValuePair<Vector2, ItemScriptableObject>> getCoordForm()
    {
        List<KeyValuePair<Vector2, ItemScriptableObject>> export = new List<KeyValuePair<Vector2, ItemScriptableObject>>();
        for (int x = 0; x < recipe.GetLength(0); x++)
        {
            for (int y = 0; y < recipe.GetLength(1); y++)
            {
                if (recipe[x, y] == null) continue;
                export.Add(new KeyValuePair<Vector2, ItemScriptableObject>(new Vector2(x, y), recipe[x, y]));
            }
        }
        return export;
    }

    /// <summary>
    /// Get what makes up this recipe, aka a list of how many what ISO.
    /// </summary>
    /// <returns></returns>
    public ResourceSet GetRecipeResourceSet()
    {
        // Make into a Dictionary
        Dictionary<ItemScriptableObject,int> holder = new Dictionary<ItemScriptableObject,int>();
        for (int x = 0; x < recipe.GetLength(0); x++)
        {
            for (int y = 0; y < recipe.GetLength(1); y++)
            {
                if (recipe[x, y] == null) continue;
                if (holder.ContainsKey(recipe[x, y]))
                {
                    holder[recipe[x, y]] += 1;
                }
                else
                {
                    holder.Add(recipe[x,y],1);
                }
            }
        }

        Dictionary<ItemScriptableObject,int> dic = new Dictionary<ItemScriptableObject,int>();

        foreach (var VARIABLE in holder.Keys)
        {
            dic.Add(VARIABLE, holder[VARIABLE] / VARIABLE.TetrisUnitAmount);
        }
        
        //Make into Resource Set
        List<ResourceSet.ResourceAmount> reourceAmountsList = new List<ResourceSet.ResourceAmount>();
        foreach (var VARIABLE in dic)
        {
            reourceAmountsList.Add(new ResourceSet.ResourceAmount( VARIABLE.Key,VARIABLE.Value));
        }
        ResourceSet export2 = new ResourceSet(0, reourceAmountsList);
        return export2;
    }

    public List<Vector2> getCoord(bool relativeToCenter = false)
    {
        List<Vector2> export = new List<Vector2>();
        Vector2 bottomRightBoundary = new Vector2(0,0);
        for (int x = 0; x < recipe.GetLength(0); x++)
        {
            for (int y = 0; y < recipe.GetLength(1); y++)
            {
                if (recipe[x, y] == null) continue;
                if (x > bottomRightBoundary.x) bottomRightBoundary.x = x;
                if (y > bottomRightBoundary.y) bottomRightBoundary.y = y;
                export.Add(new Vector2(x, y));
            }
        }

        if (!relativeToCenter) return export;

        bottomRightBoundary /= 2;
        
        for (int i = 0; i < export.Count; i++)
        {
            export[i] -= bottomRightBoundary;
        }
        return export;
    }

    //Compare CoordForm and self-CoordForm
    public bool CheckMatch(List<KeyValuePair<Vector2, ItemScriptableObject>> toCheck)
    {
        List<KeyValuePair<Vector2, ItemScriptableObject>> toMatch = getCoordForm();
        if (toCheck.Count != toMatch.Count) return false;
        foreach (KeyValuePair<Vector2, ItemScriptableObject> kvp in toCheck)
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

