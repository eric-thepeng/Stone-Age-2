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

    public bool CheckMaterialMatch(ItemScriptableObject[] isoArray)
    {
        List<ItemScriptableObject> processedISOList = new List<ItemScriptableObject>();
        foreach (ItemScriptableObject i in isoArray)
        {
            if(i!=null) processedISOList.Add(i);
        }
        if (processedISOList.Count != materials.Count) return false;
        foreach (ItemScriptableObject i in processedISOList)
        {
            if (!materials.Contains(i)) return false;
        }
        return true;
    }
}
