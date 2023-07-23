using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResourceSetDisplayer))]
public class ResourceSetDisplayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ResourceSetDisplayer resourceSetDisplayer = (ResourceSetDisplayer)target;

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // Add some flexible space to center the button
        if (GUILayout.Button("Generate"))
        {
            resourceSetDisplayer.Generate();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        DrawDefaultInspector();
    }
}