using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBuildingTool : MonoBehaviour
{
    [SerializeField] public List<ItemScriptableObject> isoListToProcess;
    [SerializeField] public float unitLength = 1;
    [SerializeField] public GameObject unitGameObject;
    [SerializeField] public GameObject edgeGameObject;
    [SerializeField] public GameObject labelGameObject;
    [SerializeField] public GameObject tetrisBaseGameObject;
    [SerializeField] public string folderPath;
    [SerializeField] public float targetScale = 0.15f;
    [SerializeField] public Vector3 shadowOffsetStandard = new Vector3(0.1f, -0.1f, 0f);
}
