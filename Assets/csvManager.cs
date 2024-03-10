using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using UnityEngine;
using UnityEditor;
using System.IO;

public class csvManager : MonoBehaviour
{
    private void Awake()
    {
        LocalizationManager.Read();
        LocalizationManager.Language = "BASIC DESCRIPTION";
        Debug.Log("basic: " + LocalizationManager.Localize("Egg Fried Rice"));
    }

    [MenuItem("Assets/Update ScriptableObjects")]
    static void UpdateScriptableObjects()
    {
        //string relativePath = Resources.Load<Item>("Items/MyItem");
        Dictionary<string, string> data = ReadCSV("./ISO_description.txt");
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

    public static Dictionary<string, string> ReadCSV(string file)
    {
        var result = new Dictionary<string, string>();
        string[] lines = File.ReadAllLines(file);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] col = lines[i].Split(';');
            result.Add(col[0], col[1]);
        }

        return result;
    }

}
