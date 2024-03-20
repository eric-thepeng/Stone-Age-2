using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;


public static class SOExport
{
    //[MenuItem("Assets/Export Data")]
    static void ExportScriptableObjects()
    {
        /*
        string[] guids = AssetDatabase.FindAssets("t:ItemScriptableObject"); 
        List<string> lines = new List<string>();

        foreach (string guid in guids)
        {

            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemScriptableObject obj = AssetDatabase.LoadAssetAtPath<ItemScriptableObject>(path);

            if (obj != null)
            {
                lines.Add(obj.tetrisHoverName + "; NOT SET");
            }
        }

        string outputFile = "C:/Unity/ExportedData.txt"; 
        File.WriteAllLines(outputFile, lines.ToArray());*/
    }
}
