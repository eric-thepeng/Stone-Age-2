//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JSAM
{
    public class JSAMEditorHelper
    {
        /// <summary>
        /// Returns file path for a script
        /// Code by Unity Forums user Tortuap
        /// https://forum.unity.com/threads/get-the-asset-path-of-yourself.523594/
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetMonoScriptPathFor(System.Type type)
        {
            var asset = "";
            var guids = AssetDatabase.FindAssets(string.Format("{0} t:script", type.Name));
            if (guids.Length > 1)
            {
                foreach (var guid in guids)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    var filename = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                    if (filename == type.Name)
                    {
                        asset = guid;
                        break;
                    }
                }
            }
            else if (guids.Length == 1)
            {
                asset = guids[0];
            }
            else
            {
                Debug.LogErrorFormat("Unable to locate {0}", type.Name);
                return null;
            }
            return AssetDatabase.GUIDToAssetPath(asset);
        }
    }
}