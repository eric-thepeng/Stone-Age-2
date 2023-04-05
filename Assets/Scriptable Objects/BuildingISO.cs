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
}
