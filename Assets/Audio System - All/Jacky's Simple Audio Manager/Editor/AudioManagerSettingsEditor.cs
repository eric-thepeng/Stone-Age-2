using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace JSAM
{
    public class AudioManagerSettingsEditor : EditorWindow
    {
        SerializedObject serializedObject;
        SerializedProperty packageLocation;
        SerializedProperty presetsLocation;
        SerializedProperty prefabsLocation;

        static AudioManagerSettingsEditor window;
        public static AudioManagerSettingsEditor Window
        {
            get
            {
                if (window == null) window = GetWindow<AudioManagerSettingsEditor>();
                return window;
            }
        }

        public static bool WindowOpen
        {
            get
            {
#if UNITY_2019_4_OR_NEWER
                return HasOpenInstances<AudioManagerSettingsEditor>();
#else
                return window != null;
#endif
            }
        }

        private void OnEnable()
        {
            OnSelectionChange();
            DesignateSerializedProperties();
        }

        private void OnSelectionChange()
        {
            if (Selection.activeObject.GetType() == typeof(AudioManagerSettings))
            {
                serializedObject = new SerializedObject(Selection.activeObject);

                DesignateSerializedProperties();
            }
            else
            {
                serializedObject = null;
            }
        }

        void DesignateSerializedProperties()
        {
            packageLocation = serializedObject.FindProperty("packageLocation");
            prefabsLocation = serializedObject.FindProperty("prefabsLocation");
            presetsLocation = serializedObject.FindProperty("presetsLocation");
        }

        [OnOpenAsset]
        public static bool Test(int instanceID, int line)
        {
            string assetPath = AssetDatabase.GetAssetPath(instanceID);
            AudioManagerSettings scriptableObject = AssetDatabase.LoadAssetAtPath<AudioManagerSettings>(assetPath);
            //if ((Selection.activeObject.GetType()).Equals(typeof(AudioManagerSettings)))
            if (scriptableObject)
            {
                Init();
            }
            //if ((AudioManagerSettings)Selection.activeObject)
            //{
            //    return true;
            //}
            return false;
        }

        // Add menu named "My Window" to the Window menu
        //[MenuItem("Window/AudioManager Settings")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            AudioManagerSettingsEditor window = (AudioManagerSettingsEditor)GetWindow(typeof(AudioManagerSettingsEditor));
            window.Show();
        }

        private void OnGUI()
        {
            if (serializedObject == null)
            {

            }
            else
            {
                GUIContent blontent = new GUIContent("JSAM Install Folder", "JSAM is stored here");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.TextField(blontent, packageLocation.stringValue);
                if (GUILayout.Button("Refresh"))
                {
                    FindAllFilePaths();
                    packageLocation.stringValue = JSAMEditorHelper.GetMonoScriptPathFor(typeof(AudioManager));
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Label("Benis", EditorStyles.boldLabel);
                //
                //if ()
            }
        }

        void FindAllFilePaths()
        {
            if (serializedObject != null)
            {
                string scriptLocation = JSAMEditorHelper.GetMonoScriptPathFor(typeof(AudioManager));
                packageLocation.stringValue = scriptLocation.Substring(0, scriptLocation.IndexOf("Scripts/AudioManager.cs"));
                prefabsLocation.stringValue = packageLocation.stringValue + "Prefabs/";
                presetsLocation.stringValue = packageLocation.stringValue + "Presets/";

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}