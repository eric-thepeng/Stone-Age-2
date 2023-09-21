using System.Collections.Generic;
using Hypertonic.GridPlacement;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    //[SerializeField]
    private BoxCollider[] boxColliders; // 拖拽你的Box Collider到这里
    [SerializeField]
    private Sprite spriteToRender; // 拖拽你想渲染的Sprite到这里
    private List<GameObject> spriteObjs = new List<GameObject>();

    private GridOperationManager gridOperationManager;


    void Start()
    {
        gridOperationManager = FindObjectOfType<GridOperationManager>();

        spriteToRender = gridOperationManager.ObstacleSprite;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        if (GetComponent<BoxCollider>() == null)
            gameObject.AddComponent<BoxCollider>();

        boxColliders = gameObject.GetComponents<BoxCollider>();

        // 获取Box Collider的尺寸和位置

        foreach (BoxCollider boxCollider in boxColliders)
        {
            boxCollider.isTrigger = true;
            rb.useGravity = false;
            rb.isKinematic = true;

            Vector3 colliderSize = boxCollider.bounds.size;
            Vector2 colliderCenter = boxCollider.bounds.center;

            // 创建一个新的GameObject作为Sprite
            GameObject spriteObj = new GameObject("GridMask - "+transform.name);
            spriteObjs.Add(spriteObj);
            SpriteMask imageMask = spriteObj.AddComponent<SpriteMask>();
            imageMask.sprite = spriteToRender;
            //ObstacleMask obsMask = spriteObj.AddComponent<ObstacleMask>();

            float cellSize = GridUtilities.GetWorldSizeOfCell(gridOperationManager._gridSettings);
            float SpriteSizeXCeiled = (float)Mathf.Ceil(colliderSize.x / cellSize) * cellSize;
            float SpriteSizeZCeiled = (float)Mathf.Ceil(colliderSize.z / cellSize) * cellSize;

            // 设置Sprite的尺寸
            spriteObj.transform.localScale = new Vector3(SpriteSizeXCeiled , SpriteSizeZCeiled , 1);

            Camera mainCamera = Camera.main;
            //Vector3 mousePositionInWorld = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.nearClipPlane));
            Vector2Int cellIndex = gridOperationManager.GetCellIndexFromWorldPosition(gameObject.transform.position);

            //obsMask.cellX = cellIndex.x;
            //obsMask.cellY = cellIndex.y;

            Vector2 _newPosVector2 = gridOperationManager.GetWorldPositionFromCellIndex(cellIndex);
            Vector3 newPosition = new Vector3(_newPosVector2.x, gridOperationManager._gridSettings.GridPosition.y, _newPosVector2.y);

            if (SpriteSizeXCeiled/cellSize %2 == 0)
            {
                newPosition.x += cellSize/ 2;
            }
            if (SpriteSizeZCeiled / cellSize % 2 == 0)
            {
                newPosition.z += cellSize / 2;
            }

            //obsMask.cellSizeX = SpriteSizeXCeiled;
            //obsMask.cellSizeY = SpriteSizeZCeiled;

            // 定位Sprite到Box Collider的底部
            //spriteObj.transform.SetParent(this.transform);
            spriteObj.transform.rotation = Quaternion.Euler(90, 0, 0);
            spriteObj.transform.position = newPosition;
            
        }
    }

    private void OnDestroy()
    {
        foreach (GameObject spriteObj in spriteObjs)
        {
            Destroy(spriteObj);
        }
    }
}
