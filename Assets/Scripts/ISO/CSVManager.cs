using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public enum ISOFolder
{
    Food,
    Furniture,
    Materials,
    Parts,
    ResourceBuilding,
    Storage,
    Tool,
    Unimplemented,
    Workshop
}
public class csvManager : MonoBehaviour
{ 

    //private void Awake()
    //{
    //    LocalizationManager.Read();
    //    LocalizationManager.Language = "BASIC DESCRIPTION";
    //    Debug.Log("basic: " + LocalizationManager.Localize("Egg Fried Rice"));
    //}

    [MenuItem("Assets/Update ScriptableObjects")]
    static void UpdateScriptableObjects()
    {
        foreach (ISOFolder folderName in Enum.GetValues(typeof(ISOFolder)))
        {
            string fileName = folderName.ToString() + ".csv";
            string filePath = Path.Combine(Application.dataPath, "Scripts/ISO/data", fileName);

            Dictionary<string, string[]> data = ReadCSV(filePath);
            foreach (KeyValuePair<string, string[]> pair in data)
            {
                string path = "Assets/Scriptable Objects/ISO/" + folderName.ToString() + "/" + "ISO_" + pair.Key + ".asset";
                if (folderName == ISOFolder.Furniture || folderName == ISOFolder.ResourceBuilding || folderName == ISOFolder.Storage || folderName == ISOFolder.Workshop)
                {
                    path = "Assets/Scriptable Objects/ISO/" + folderName.ToString() + "/" + "BISO_" + pair.Key + ".asset";
                }

                ItemScriptableObject obj = AssetDatabase.LoadAssetAtPath<ItemScriptableObject>(path);

                if (obj != null)
                {
                    obj.tetrisDescription = pair.Value[0];
                    obj.tetrisSideNote = pair.Value[1];
                    EditorUtility.SetDirty(obj);
                    //print($"ItemScriptableObject found for key: {pair.Key}");
                }
                else
                {
                    print($"ItemScriptableObject not found for key: {path}");
                }
            }
        }
       

        AssetDatabase.SaveAssets();
    }

    public static Dictionary<string, string[]> ReadCSV(string file)
    {
        var result = new Dictionary<string, string[]>();
        string[] lines = File.ReadAllLines(file);
        for (int i = 0; i < lines.Length; i++)
        {
            string[] col = lines[i].Split(',');
            col[0] = col[0].Replace(' ', '_').Trim();
            for (int j = 1; j < col.Length; j++)
            {
                col[j] = col[j].Replace(';', ',').Trim();
            }
            result.Add(col[0], new string[] { col[1], col[2] });
        }
        return result;
    }
}