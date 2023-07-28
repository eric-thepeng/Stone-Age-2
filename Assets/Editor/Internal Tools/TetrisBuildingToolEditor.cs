using System.Collections;
using System.Collections.Generic;
using PlasticGui;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TetrisBuildingTool))]
public class TetrisBuildingToolEditor : Editor
{
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

            // add the unit tiles first
            foreach (Vector2Int coord in iso.HomogeneousCoord)
            {
                GameObject newTetris = Instantiate(builder.unitGameObject, new Vector3(coord.x * builder.unitLength, -coord.y * builder.unitLength, 0), Quaternion.identity);
                newTetris.transform.parent = newGameObject.transform;
            }

            // then add the edges
        
            // save as prefab
            string localPath = builder.folderPath + "/" + newGameObject.name + ".prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            PrefabUtility.SaveAsPrefabAssetAndConnect(newGameObject, localPath, InteractionMode.UserAction);
            
        }
    }
}
