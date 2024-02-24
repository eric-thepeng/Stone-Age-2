using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UniAction))]
public class UniActionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // UniActionType is the first field in UniAction
        var uniActionTypeProp = property.FindPropertyRelative("uniActionType");

        // First row - UniActionType dropdown
        var rowHeight = base.GetPropertyHeight(uniActionTypeProp, label);
        var rowRect = new Rect(position.x, position.y, position.width, rowHeight);
        EditorGUI.PropertyField(rowRect, uniActionTypeProp);

        // Depending on UniActionType, draw other fields
        switch ((UniAction.UniActionType)uniActionTypeProp.enumValueIndex)
        {
            case UniAction.UniActionType.NoAction:
                break;
            case UniAction.UniActionType.GameObjectAction:
                DrawProperty("gameObjectAction", property, ref rowRect);
                break;
            case UniAction.UniActionType.NarrativeSequenceAction:
                DrawProperty("narrativeSequenceAction", property, ref rowRect);
                break;
            case UniAction.UniActionType.UniQuestAction:
                DrawProperty("uniQuestAction", property, ref rowRect);
                break;
            case UniAction.UniActionType.PlayerStatAction:
                DrawProperty("playerStatAction", property, ref rowRect);
                break;
            case UniAction.UniActionType.ButtonAction:
                DrawProperty("buttonAction", property, ref rowRect);
                break;
            case UniAction.UniActionType.LevelUpAction:
                DrawProperty("levelUpAction", property, ref rowRect);
                break;
            case UniAction.UniActionType.GamePanelAction:
                DrawProperty("gamePanelAction", property, ref rowRect);
                break;
            case UniAction.UniActionType.CameraAction:
                DrawProperty("cameraAction", property, ref rowRect);
                break;
            case UniAction.UniActionType.IUniActionInteractionAction:
                DrawProperty("iuaiAction", property, ref rowRect);
                break;
            case UniAction.UniActionType.CharacterGatherAction:
                DrawProperty("characterGatherAction", property, ref rowRect);
                break;
            case UniAction.UniActionType.BlueprintAction:
                DrawProperty("blueprintAction", property, ref rowRect);
                break;
            case UniAction.UniActionType.TimelineAnimationAction:
                DrawProperty("timelineAnimationAction", property, ref rowRect);
                break;
            case UniAction.UniActionType.CharacterInteractionAction:
                DrawProperty("characterInteractionAction", property, ref rowRect);
                break;
            case UniAction.UniActionType.ScreenNotificationAction:
                DrawProperty("screenNotificationAction", property, ref rowRect);
                break;
            case UniAction.UniActionType.CharacterStatAction:
                DrawProperty("characterStatAction", property, ref rowRect);
                break;
        }

        EditorGUI.EndProperty();
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = base.GetPropertyHeight(property, label);

        var uniActionTypeProp = property.FindPropertyRelative("uniActionType");
        switch ((UniAction.UniActionType)uniActionTypeProp.enumValueIndex)
        {
            case UniAction.UniActionType.NoAction:
                break;
            case UniAction.UniActionType.GameObjectAction:
                height += GetHeightForProperty(property, "gameObjectAction");
                break;
            case UniAction.UniActionType.NarrativeSequenceAction:
                height += GetHeightForProperty(property, "narrativeSequenceAction");
                break;
            case UniAction.UniActionType.UniQuestAction:
                height += GetHeightForProperty(property, "uniQuestAction");
                break;
            case UniAction.UniActionType.PlayerStatAction:
                height += GetHeightForProperty(property, "playerStatAction");
                break;
            case UniAction.UniActionType.ButtonAction:
                height += GetHeightForProperty(property, "buttonAction");
                break;
            case UniAction.UniActionType.LevelUpAction:
                height += GetHeightForProperty(property, "levelUpAction");
                break;
            case UniAction.UniActionType.GamePanelAction:
                height += GetHeightForProperty(property, "gamePanelAction");
                break;
            case UniAction.UniActionType.CameraAction:
                height += GetHeightForProperty(property, "cameraAction");
                break;
            case UniAction.UniActionType.IUniActionInteractionAction:
                height += GetHeightForProperty(property, "iuaiAction");
                break;
            case UniAction.UniActionType.CharacterGatherAction:
                height += GetHeightForProperty(property, "characterGatherAction");
                break;
            case UniAction.UniActionType.BlueprintAction:
                height += GetHeightForProperty(property, "blueprintAction");
                break;
            case UniAction.UniActionType.TimelineAnimationAction:
                height += GetHeightForProperty(property, "timelineAnimationAction");
                break;
            case UniAction.UniActionType.CharacterInteractionAction:
                height += GetHeightForProperty(property, "characterInteractionAction");
                break;
            case UniAction.UniActionType.ScreenNotificationAction:
                height += GetHeightForProperty(property, "screenNotificationAction");
                break;
            case UniAction.UniActionType.CharacterStatAction:
                height += GetHeightForProperty(property, "characterStatAction");
                break;
        }

        return height;
    }

    private float GetHeightForProperty(SerializedProperty parentProperty, string propertyName)
    {
        var property = parentProperty.FindPropertyRelative(propertyName);
        return EditorGUI.GetPropertyHeight(property, includeChildren: true) + EditorGUIUtility.standardVerticalSpacing;
    }

    private void DrawProperty(string propertyName, SerializedProperty parentProperty, ref Rect rect)
    {
        var property = parentProperty.FindPropertyRelative(propertyName);
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(rect, property, includeChildren: true);
    }
}