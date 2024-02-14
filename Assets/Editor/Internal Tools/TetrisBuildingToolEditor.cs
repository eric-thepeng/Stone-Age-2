using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using TMPro;

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
        
        //build a folder
        string newFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        AssetDatabase.CreateFolder(builder.folderPath, newFolderName);
        string newFolderPath = builder.folderPath + "/" + newFolderName;
        
        //building for each iso
        foreach (ItemScriptableObject iso in builder.allISOList.list)
        {
            // make new game object
            GameObject newGameObject = Instantiate(builder.tetrisBaseGameObject, new Vector3(0,0,0),quaternion.identity); //new GameObject("Tetris_"+iso.name);
            newGameObject.name = "New UniTetris " + iso.tetrisHoverName;
            newGameObject.GetComponent<Tetris>().itemSO = iso;
            GameObject unitsContainer = new GameObject("Units Container");
            unitsContainer.transform.parent = newGameObject.transform;
            GameObject edgesContainer = new GameObject("Edges Container");
            edgesContainer.transform.parent = newGameObject.transform;

            // add the unit tiles and edges
            foreach (Vector2Int coord in iso.HomogeneousCoord)
            {
                //add unit tiles
                Vector3 unitPosition = new Vector3(coord.x * builder.unitLength, -coord.y * builder.unitLength, 0);
                GameObject newUnit = Instantiate(builder.unitGameObject, unitPosition, Quaternion.identity);
                newUnit.transform.parent = unitsContainer.transform;
                newUnit.GetComponent<SpriteRenderer>().color = builder.tetrisColor;
                
                //add image
                
                //add edges
                foreach (Vector2Int dir in directions)
                {
                    if(iso.HomogeneousCoord.Contains(coord + dir)) continue;
                    Vector3 edgeDeltaPosition = 0.5f * new Vector3(dir.x * builder.unitLength, -dir.y * builder.unitLength, 0);
                    GameObject newEdge = Instantiate(builder.edgeGameObject,unitPosition + edgeDeltaPosition, quaternion.identity);
                    newEdge.transform.parent = edgesContainer.transform;

                    Edge edgeComponent = newEdge.GetComponent<Edge>();
                    
                    //assign Edge.MyFacing
                    if (dir == Vector2Int.left) edgeComponent.myFacing = Edge.facing.Left;
                    else if (dir == Vector2Int.right) edgeComponent.myFacing = Edge.facing.Right;
                    else if (dir == Vector2Int.up) edgeComponent.myFacing = Edge.facing.Up;
                    else if (dir == Vector2Int.down) edgeComponent.myFacing = Edge.facing.Down;

                    //assign Edge.AttachedCoordination
                    edgeComponent.attachedCoordination = coord;

                    //set facing of the edge. since y coord is flipped when spawning tetris units, we flip it back when setting the facing
                    Vector2Int dirToSetEdge = dir * new Vector2Int(1, -1);
                    newEdge.GetComponent<Edge>().SetFacingAccordingToDirection(dirToSetEdge);
                }
            }
            
            //create base image
            //GameObject newImage = new GameObject("ASSIGN IMAGE HERE", typeof(SpriteRenderer));
            GameObject newImage = Instantiate(builder.iconSpriteGameObject, newGameObject.transform);
            newImage.GetComponent<SpriteRenderer>().sprite = iso.iconSprite;
            newImage.gameObject.name = "Icon Sprite";

            // add text
            /*
            GameObject newLabel = Instantiate(builder.labelGameObject, newGameObject.transform);
            newLabel.GetComponent<TextMeshPro>().text = iso.tetrisHoverName;
            newLabel.gameObject.name = "Temporary Label";*/
            
            // create outline
            if (builder.outlineWidth != 0)
            {
                GameObject outlineContainer = new GameObject();
                outlineContainer.transform.SetParent(newGameObject.transform);
                outlineContainer.gameObject.name = "Outline container";
                GameObject outlineLeft = Instantiate(unitsContainer.gameObject, outlineContainer.transform);
                GameObject outlineRight = Instantiate(unitsContainer.gameObject, outlineContainer.transform);
                GameObject outlineUp = Instantiate(unitsContainer.gameObject, outlineContainer.transform);
                GameObject outlineDown = Instantiate(unitsContainer.gameObject, outlineContainer.transform);
                foreach (SpriteRenderer sr in outlineContainer.GetComponentsInChildren<SpriteRenderer>())
                {
                    sr.color = builder.outlineColor;
                    sr.sortingOrder -= 2;
                }
                outlineLeft.transform.localPosition = new Vector3(builder.outlineWidth,0,0);
                outlineRight.transform.localPosition = new Vector3(-builder.outlineWidth,0,0);
                outlineUp.transform.localPosition = new Vector3(0,builder.outlineWidth,0);
                outlineDown.transform.localPosition = new Vector3(0,-builder.outlineWidth,0);
            }

            // create shadow
            GameObject shadow = Instantiate(unitsContainer.gameObject, newGameObject.transform);
            shadow.gameObject.name = "Shadow container";
            shadow.transform.localPosition = new Vector3(0,0,0) + builder.shadowOffsetStandard;
            shadow.transform.localScale = new Vector3(1, 1, 1);
            foreach (SpriteRenderer sr in shadow.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = builder.shadowColor;
                sr.sortingOrder -= 3;
            }

            // adjust size
            newGameObject.transform.localScale *= builder.targetScale;
            
            // save as prefab
            string localPath = newFolderPath + "/" + newGameObject.name + ".prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            
            // dependency binding
            iso.myPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(newGameObject, localPath, InteractionMode.UserAction);
            EditorUtility.SetDirty(iso);
            AssetDatabase.SaveAssets();
        }
    }
}
