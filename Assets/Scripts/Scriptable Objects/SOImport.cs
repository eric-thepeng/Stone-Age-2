using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SOImport : MonoBehaviour
{
    [MenuItem("Assets/Update ScriptableObjects")]
    static void UpdateScriptableObjects()
    {
        Dictionary<string, string> data = SOReadCSV.ReadCSV("C:/Unity/ExportedData.txt");
        foreach (KeyValuePair<string, string> pair in data)
        {
            string path = "Assets/ScriptableObjects/CraftingSystem/ItemScriptableObject" + pair.Key + ".asset";
            ItemScriptableObject obj = AssetDatabase.LoadAssetAtPath<ItemScriptableObject>(path);

            if (obj != null)
            {
                obj.tetrisDescription = pair.Value;
                EditorUtility.SetDirty(obj);
            }
        }

        AssetDatabase.SaveAssets();
    }
}
