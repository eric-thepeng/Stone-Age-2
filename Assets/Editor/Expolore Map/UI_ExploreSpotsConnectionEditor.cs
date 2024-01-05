using Unity.Mathematics;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UI_ExploreSpotsConnection))]
public class UI_ExploreSpotsConnectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector.
        DrawDefaultInspector();
        
        // Reference to script.
        UI_ExploreSpotsConnection myScript = (UI_ExploreSpotsConnection)target;
        
        // Add a button to the inspector.
        if(GUILayout.Button("Calculate Lines"))
        {
            // Calculate connection lines.
            foreach (var line in myScript.allConnectionLines)
            {
                Vector3 pos1 = line.exploreSpot1.gameObject.transform.localPosition;
                Vector3 pos2 = line.exploreSpot2.gameObject.transform.localPosition;
                
                Vector3 center = (pos1 - pos2)/2 + pos2;
                float length = (pos1 - pos2).magnitude;

                Vector2 direction = pos2 - pos1;
                float angleRadians = Mathf.Atan2(direction.y, direction.x);
                float angleDegrees = angleRadians * Mathf.Rad2Deg;

                line.lineGameObject.transform.eulerAngles = new Vector3(65, 0, angleDegrees);
                line.lineGameObject.transform.localPosition = center;
                line.lineGameObject.transform.localScale = new Vector3(length, 0.4f, 1f);

                line.lineGameObject.GetComponent<SpriteRenderer>().color = myScript.hiddenColor;
            }
        }
    }
}
