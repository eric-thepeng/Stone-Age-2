using System.Collections;
using System.Collections.Generic;
using Hypertonic.GridPlacement;
using UnityEditor;
using UnityEngine;

public class GridNearbyObjectDetect : MonoBehaviour
{
    [SerializeField]
    private GameObject OperationManager;

    [SerializeField]
    private List<GameObject> recipe;

    [SerializeField]
    private GameObject result;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if (OperationManager.GetComponent<GridOperationManager>().operateStatus)
        // {
        //     GameObject itemPlaced = OperationManager.GetComponent<GridOperationManager>().itemPlaced;
        //     List<GameObject> nearbyGameObjects = GridManagerAccessor.GridManager.GetSurroundingObjects(itemPlaced, 1);
        //     List<GameObject> containGameObjects = new List<GameObject>();
        //     Debug.Log(nearbyGameObjects.Count);
        //     //foreach(GameObject surroundObject in nearbyGameObjects)
        //     //{
        //         
        //     //}
        //     bool containsAll = true;
        //     foreach (GameObject recipeObject in recipe)
        //     {
        //         bool foundMatch = false;
        //
        //         // 遍历场景中的inventory游戏对象
        //         foreach (GameObject inventoryObject in nearbyGameObjects)
        //         {
        //             // 检查inventoryObject是否与recipePrefab相匹配
        //             if (PrefabUtility.GetPrefabInstanceHandle(inventoryObject) == PrefabUtility.GetPrefabInstanceHandle(recipeObject))
        //             {
        //                 foundMatch = true;
        //                 Debug.Log("Found match true!");
        //                 nearbyGameObjects.Remove(inventoryObject);
        //                 containGameObjects.Add(inventoryObject);
        //                 break;
        //             }
        //         }
        //         // 如果没有找到匹配的Prefab对象，则返回false
        //         if (!foundMatch)
        //         {
        //             containsAll = false;
        //             Debug.Log(containsAll);
        //             break;
        //         }
        //
        //         //if (!nearbyGameObjects.Contains(recipeObject))
        //         //{
        //         //    containsAll = false;
        //         //    Debug.Log(containsAll);
        //         //    break;
        //         //}
        //         //nearbyGameObjects.Remove(recipeObject);
        //     }
        //
        //     if (containsAll)
        //     {
        //         foreach (GameObject cont in containGameObjects)
        //         {
        //             GridManagerAccessor.GridManager.DeleteObject(cont);
        //         }
        //         Instantiate(result, itemPlaced.transform);
        //     }
        //
        //
        // }
    }

}
