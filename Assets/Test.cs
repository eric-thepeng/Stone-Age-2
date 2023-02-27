using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    private void Start()
    {
        print("carryover sprites");
        foreach(Transform child in transform)
        {
            ItemScriptableObject iso = child.GetComponent<Tetris>().itemSO;
            iso.tetrisSprite = child.GetComponent<SpriteRenderer>().sprite;
            EditorUtility.SetDirty(iso);
        }
        AssetDatabase.SaveAssets();
    }
}
