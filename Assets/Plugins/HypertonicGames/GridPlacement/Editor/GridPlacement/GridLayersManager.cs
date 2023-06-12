using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Hypertonic.GridPlacement
{
    [InitializeOnLoad]
    public class GridLayersManager
    {

        private static readonly int _unityLayerCount = 32;

        static GridLayersManager()
        {
            _ = Init();
        }

        private static async Task Init()
        {
            await Task.Delay(1);
            CreateLayersIfNotPresent(new string[] { "Grid", "PlacementGrid" });
        }

        /// <summary>
        /// Creates the layers in Unity if they don't already exist
        /// </summary>
        /// <param name="layerNames">The names of the layers to create </param>
        public static void CreateLayersIfNotPresent(string[] layerNames)
        {
            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            // Layers Property
            SerializedProperty layersProp = tagManager.FindProperty("layers");

            for (int i = 0; i < layerNames.Length; i++)
            {
                CreateLayer(layersProp, layerNames[i]);
            }

            tagManager.ApplyModifiedProperties();
        }
        
        private static bool CreateLayer(SerializedProperty layersProp, string layerName)
        {
            if (!PropertyExists(layersProp, 0, _unityLayerCount, layerName))
            {
                SerializedProperty sp;
                for (int i = 5, j = _unityLayerCount; i < j; i++)
                {
                    sp = layersProp.GetArrayElementAtIndex(i);
                    if (sp.stringValue == "")
                    {
                        // Assign string value to layer
                        sp.stringValue = layerName;
                        Debug.Log("Layer: " + layerName + " has been added");
                       
                        return true;
                    }
                    if (i == j)
                        Debug.LogError("All allowed layers have been filled. Could not create layers for the Grid Placement System.");
                }
            }
           
            return false;
        }

        /// <summary>
        /// Checks if the value exists in the property.
        /// </summary>
        /// <returns><c>true</c>, if exists was propertyed, <c>false</c> otherwise.</returns>
        /// <param name="property">Property.</param>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        /// <param name="value">Value.</param>
        private static bool PropertyExists(SerializedProperty property, int start, int end, string value)
        {
            for (int i = start; i < end; i++)
            {
                SerializedProperty t = property.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

#endif