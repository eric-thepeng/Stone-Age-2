using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * ItemScriptableObject represents the grid formulation of a single Tetris Game Obejct. It is a template for
*/

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "ScriptableObjects/CraftingSystem/BuildingISO")]
public class BuildingISO : ItemScriptableObject
{
    public GameObject buildingPrefab;
    [SerializeField] int width = 1;
    [SerializeField] int height = 1;
    public List<Vector2Int> getCoordinates()
    {
        List<Vector2Int> returnList = new List<Vector2Int>();
        for(int x = 0; x<width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                returnList.Add(new Vector2Int(x, y));
            }
        }
        return returnList;
    }

    public GameObject GetBuildingPrefab()
    {
        return buildingPrefab;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }
}
