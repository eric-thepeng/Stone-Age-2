using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


//[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    private void Start()
    {
        /*
        print("carryover sprites");
        foreach(Transform child in transform)
        {
            ItemScriptableObject iso = child.GetComponent<Tetris>().itemSO;
            iso.tetrisSprite = child.GetComponent<SpriteRenderer>().sprite;
            EditorUtility.SetDirty(iso);
        }
        AssetDatabase.SaveAssets();*/

       // Grid testGrid = new Grid(10, 10, 1f, transform.Find("GridTopLeftCorner").transform.position);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            GameObject go = WorldUtility.CreateWorldText(""+WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.HOME_GRID, true), null, WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.HOME_GRID, true),5,null,TextAnchor.UpperCenter,TMPro.TextAlignmentOptions.Center).gameObject;
            go.transform.position += new Vector3(0, 0.1f, 0);
            go.transform.rotation = Quaternion.Euler(45,0,0);
            print(WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.HOME_GRID, true));
        }
    }
}
