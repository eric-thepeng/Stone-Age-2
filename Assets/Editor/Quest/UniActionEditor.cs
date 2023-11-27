using UnityEngine;
using UnityEditor;


// We are not using this since UniAction is not MonoBehaviour


[CustomEditor(typeof(UniAction))]
public class UniActionEditor : Editor
{
    SerializedProperty uniActionTypeProp;
    
    SerializedProperty gameObjectActionProp;
    SerializedProperty narrativeSequenceActionProp;
    SerializedProperty uniQuestActionProp;
    SerializedProperty playerStatActionProp;
    SerializedProperty buttonActionProp;
    

    void OnEnable()
    {
        // Link the serialized properties
        uniActionTypeProp = serializedObject.FindProperty("uniActionType");
        
        gameObjectActionProp = serializedObject.FindProperty("gameObjectAction");
        narrativeSequenceActionProp  = serializedObject.FindProperty("narrativeSequenceAction");;
        uniQuestActionProp = serializedObject.FindProperty("uniQuestAction");;
        playerStatActionProp = serializedObject.FindProperty("playerStatAction");;
        buttonActionProp = serializedObject.FindProperty("buttonAction");;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(uniActionTypeProp);

        UniAction.UniActionType selectedType = (UniAction.UniActionType)uniActionTypeProp.enumValueIndex;

        switch (selectedType)
        {
            case UniAction.UniActionType.NoAction:
                break;
            case UniAction.UniActionType.GameObjectAction:
                EditorGUILayout.PropertyField(gameObjectActionProp);
                break;
            case UniAction.UniActionType.NarrativeSequenceAction:
                EditorGUILayout.PropertyField(narrativeSequenceActionProp);
                break;
            case UniAction.UniActionType.UniQuestAction:
                EditorGUILayout.PropertyField(uniQuestActionProp);
                break;
            case UniAction.UniActionType.PlayerStatAction:
                EditorGUILayout.PropertyField(playerStatActionProp);
                break;
            case UniAction.UniActionType.ButtonAction:
                EditorGUILayout.PropertyField(buttonActionProp);
                break;
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
