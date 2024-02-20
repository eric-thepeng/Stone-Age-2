using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UI_ExploreSpotsConnection))]
public class UI_ExploreSpotsConnectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        UI_ExploreSpotsConnection myScript = (UI_ExploreSpotsConnection)target;

        // Calculate Lines Button
        if (GUILayout.Button("Calculate Lines"))
        {
            Undo.SetCurrentGroupName("Create Lines"); // Start of Undo group
            int group = Undo.GetCurrentGroup();

            myScript.lineTemplate.SetActive(true);

            // Calculate connection lines.
            foreach (var line in myScript.allConnectionLines)
            {
                GameObject newLineGameObject = Instantiate(myScript.lineTemplate, myScript.transform);
                Undo.RegisterCreatedObjectUndo(newLineGameObject, "Create Line"); // Register Undo for each new line

                line.lineGameObject = newLineGameObject;
                newLineGameObject.name = line.iSerialEffect1.gameObject.name + " to " + line.iSerialEffect2.gameObject.name;

                Vector3 pos1 = line.iSerialEffect1.gameObject.transform.localPosition;
                Vector3 pos2 = line.iSerialEffect2.gameObject.transform.localPosition;

                Vector3 center = (pos1 - pos2) / 2 + pos2;
                float length = (pos1 - pos2).magnitude;

                Vector2 direction = pos2 - pos1;
                float angleRadians = Mathf.Atan2(direction.y, direction.x);
                float angleDegrees = angleRadians * Mathf.Rad2Deg;

                newLineGameObject.transform.eulerAngles = new Vector3(65, 0, angleDegrees);
                newLineGameObject.transform.localPosition = center;
                newLineGameObject.transform.localScale = new Vector3(length, 0.4f, 1f);

                newLineGameObject.GetComponent<SpriteRenderer>().color = myScript.hiddenColor;
            }

            myScript.lineTemplate.SetActive(false);

            Undo.CollapseUndoOperations(group); // End of Undo group
        }

        // Draw rest of inspector
        DrawDefaultInspector();
    }
}
