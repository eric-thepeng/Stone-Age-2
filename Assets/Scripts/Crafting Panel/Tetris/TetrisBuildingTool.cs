using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBuildingTool : MonoBehaviour
{
    [SerializeField, Header("------Baisc Info------")] public ItemSOListScriptableObject allISOList;
    [SerializeField] public float unitLength = 1;
    [SerializeField] public string folderPath;
    [SerializeField] public float targetScale = 0.15f;

    [SerializeField, Header("------Color Info------")] public Color tetrisColor;
    [SerializeField] public Color outlineColor;
    [SerializeField] public float outlineWidth = 0.03f;
    [SerializeField] public Vector3 shadowOffsetStandard = new Vector3(0.1f, -0.1f, 0f);
    
    [SerializeField, Header("------Config, DO NOT EDIT------")] public GameObject unitGameObject;
    [SerializeField] public GameObject edgeGameObject;
    [SerializeField] public GameObject labelGameObject;
    [SerializeField] public GameObject tetrisBaseGameObject;
}
