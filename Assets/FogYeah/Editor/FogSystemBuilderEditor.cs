using UnityEditor;
using UnityEngine;

namespace FogSystems
{
    /// <summary>
    /// Editor script for the FogSystem component.
    /// This will add some information and buttons for creating / removing Fog tiles from the scene. 
    /// </summary>
    [CustomEditor(typeof(FogSystem))]
    public class FogSystemBuilderEditor : Editor
    {
        readonly string currentVersion = "2.3.0";

        public Texture fogSystemLogo;

        public override void OnInspectorGUI()
        {
            // Ensure any modified properties remain.
            serializedObject.ApplyModifiedProperties();

            // Get the target script. 
            FogSystem fogScript = (FogSystem)target;

            // Draw Logo
            GUILayout.Box(fogSystemLogo);

            // General info
            EditorGUILayout.HelpBox("Customize these options and hit 'Generate Fog Tiles' at the bottom.", MessageType.Info);

            // Mode switcher
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("CurrentMode"));

            string[] options = new string[] { "2D Map Size", "2D Map Sprite", "3D Terrain Cover" };

            fogScript.CurrentMode = (FogSystemEnums.FogSystemMode)GUILayout.Toolbar(serializedObject.FindProperty("CurrentMode").intValue, options);

            // Display different editor fields based on what Mode is currently selected.
            switch(fogScript.CurrentMode)
            {
                case FogSystemEnums.FogSystemMode._2DMapSize:
                    // Settings
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_2DMapSizeSettings"));

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_CommonSettings"));

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_FogSystemAdvancedSettings"));
                    // Info
                    EditorGUILayout.HelpBox("Generating tiles could take some time if you have a large grid size number.", MessageType.Info);
                    break;
                case FogSystemEnums.FogSystemMode._2DMapSprite:
                    // Settings
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_2DMapSpriteSettings"));

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_CommonSettings"));

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_FogSystemAdvancedSettings"));
                    // Info
                    EditorGUILayout.HelpBox("Generating tiles could take some time if you have a large density / grid size number.", MessageType.Info);
                    break;
                case FogSystemEnums.FogSystemMode._3D:
                    // Settings
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_3DSettings"));

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_CommonSettings.StartMode"));

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_CommonSettings.RegrowthMode"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_CommonSettings.ShroudAmount"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_CommonSettings.FadeSpeed"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_CommonSettings.RefreshDelay"));

                    // Info
                    EditorGUILayout.HelpBox("Generating tiles could take some time if you have a large grid size number.", MessageType.Info);
                    break;
            }

            // Give the user a button for Generating the fog tiles 
            if (GUILayout.Button("Generate Fog Tiles"))
                fogScript.GenerateFog();

            // Give the user a button for Removing the fog tiles (Undo)
            if (GUILayout.Button("Remove Fog Tiles"))
                fogScript.RemoveFog();

            // Map sprite option cleanup is only available to 2d Map Sprite
            if (fogScript.CurrentMode == FogSystemEnums.FogSystemMode._2DMapSprite)
            {
                if (GUILayout.Button("Remove Map Sprite"))
                    fogScript.CleanupArtifacts();
            }

            // Effector settings are common to all. 
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EffectorOptions"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Effectors"));

            // Cache the background color.
            Color defaultGUIColor = GUI.backgroundColor;

            var vstyle = new GUIStyle(GUI.skin.button);
            vstyle.normal.textColor = Color.grey;
            vstyle.padding = new RectOffset(0, 0, 0, 0);
            vstyle.border = new RectOffset(0, 0, 0, 0);
            vstyle.fontSize = 8;
            GUI.Label(new Rect(22, 8, 70, 15), $"Fog Yeah v{currentVersion}", vstyle);

            // Revert the color to the cached version
            GUI.backgroundColor = defaultGUIColor;

            // Ensure any modified properties are applied.
            serializedObject.ApplyModifiedProperties();
        }
    }
}