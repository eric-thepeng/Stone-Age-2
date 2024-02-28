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

            Dictionary<SO_SerialEffectIdentifier, GameObject> allSpotsSEIDictionary = new Dictionary<SO_SerialEffectIdentifier, GameObject>();

            foreach (Transform siblingTransform in myScript.gameObject.transform.parent.transform)
            {
                GameObject holder = siblingTransform.gameObject;
                ISerialEffect tryToGetISE = siblingTransform.gameObject.GetComponent<ISerialEffect>();
                if ( tryToGetISE != null)
                {
                    if (tryToGetISE.mySEI == null)
                    {
                        Debug.LogWarning("Set Up SEI is null at Game Object: "+siblingTransform.gameObject.name);
                    }
                    else
                    {
                        allSpotsSEIDictionary.Add(tryToGetISE.mySEI, siblingTransform.gameObject);
                    }
                }
            }

            myScript.lineTemplate.SetActive(true);
            myScript.allConnectionLines.Clear();

            foreach (var relationship in myScript.TargetSEIGroup.allRelationships)
            {
                // Create Line Game Object
                if (allSpotsSEIDictionary.ContainsKey(relationship.FirstUnit) &&
                    allSpotsSEIDictionary.ContainsKey(relationship.SecondUnit))
                {

                    GameObject newLineGameObject = Instantiate(myScript.lineTemplate, myScript.transform);
                    Undo.RegisterCreatedObjectUndo(newLineGameObject, "Create Line"); // Register Undo for each new line

                    UI_ExploreSpotsConnection.ConnectionLine line = new UI_ExploreSpotsConnection.ConnectionLine();
                    
                    line.lineGameObject = newLineGameObject;
                    line.serialEffectIdentifier_1 = relationship.FirstUnit;
                    line.serialEffectIdentifier_2 = relationship.SecondUnit;

                    newLineGameObject.name = line.serialEffectIdentifier_1.name + " to " + line.serialEffectIdentifier_2.name;

                    Vector3 pos1 = allSpotsSEIDictionary[line.serialEffectIdentifier_1].gameObject.transform.localPosition;
                    Vector3 pos2 = allSpotsSEIDictionary[line.serialEffectIdentifier_2].gameObject.transform.localPosition;

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
            }
            
            myScript.lineTemplate.SetActive(false);
            
            Undo.CollapseUndoOperations(group); // End of Undo group
        }

        // Draw rest of inspector
        DrawDefaultInspector();
    }
}
