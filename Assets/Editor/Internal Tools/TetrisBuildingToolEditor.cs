using System.Collections;
using System.Collections.Generic;
using PlasticGui;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TetrisBuildingTool))]
public class TetrisBuildingToolEditor : Editor
{
    List<Vector2Int> directions = new List<Vector2Int> { Vector2Int.down, Vector2Int.left, Vector2Int.up, Vector2Int.right};
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Build All"))
        {
            Build();
        }
        base.OnInspectorGUI();
    }

    void Build()
    {
        //binding target
        TetrisBuildingTool builder = (TetrisBuildingTool)target;
        
        //building for each iso
        foreach (ItemScriptableObject iso in builder.isoListToProcess)
        {
            // make new game object
            GameObject newGameObject = new GameObject("Tetris_"+iso.name);

            // add the unit tiles and edges
            foreach (Vector2Int coord in iso.HomogeneousCoord)
            {
                //add unit tiles
                Vector3 unitPosition = new Vector3(coord.x * builder.unitLength, -coord.y * builder.unitLength, 0);
                GameObject newUnit = Instantiate(builder.unitGameObject, unitPosition, Quaternion.identity);
                newUnit.transform.parent = newGameObject.transform;
                
                //add edges
                foreach (Vector2Int dir in directions)
                {
                    if(iso.HomogeneousCoord.Contains(coord + dir)) continue;
                    Vector3 edgeDeltaPosition = 0.5f * new Vector3(dir.x * builder.unitLength, -dir.y * builder.unitLength, 0);
                    GameObject newEdge = Instantiate(builder.edgeGameObject,unitPosition + edgeDeltaPosition, quaternion.identity);
                    newEdge.transform.parent = newGameObject.transform;
                }
            }

            // save as prefab
            string localPath = builder.folderPath + "/" + newGameObject.name + ".prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            PrefabUtility.SaveAsPrefabAssetAndConnect(newGameObject, localPath, InteractionMode.UserAction);
            
        }
    }
}
