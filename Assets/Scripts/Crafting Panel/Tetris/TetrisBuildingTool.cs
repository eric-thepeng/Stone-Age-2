using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBuildingTool : MonoBehaviour
{
    [SerializeField] public List<ItemScriptableObject> isoListToProcess;
    [SerializeField] public float unitLength = 1;
    [SerializeField] public GameObject unitGameObject;
    [SerializeField] public GameObject edgeGameObject;
    [SerializeField] public GameObject tetrisBaseGameObject;
    [SerializeField] public string folderPath;
}
