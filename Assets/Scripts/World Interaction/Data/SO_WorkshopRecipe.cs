using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Workshop Recipe", menuName = "ScriptableObjects/WorldInteraction/Workshop Recipe")]
public class SO_WorkshopRecipe : ScriptableObject
{
    public List<ItemScriptableObject> materials;
    public ItemScriptableObject product;
    public float workTime = 5;
    public bool unlocked = false;
}
