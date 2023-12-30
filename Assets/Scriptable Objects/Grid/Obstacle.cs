using System.Collections.Generic;
using Hypertonic.GridPlacement;
using UnityEngine;
using UnityEngine.Tilemaps;

    // [RequireComponent(typeof(BoxCollider))]
    // [RequireComponent(typeof(Rigidbody))]
    // [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(ObstacleIndicator))]
public class Obstacle : MonoBehaviour
{
    //[SerializeField]
    private BoxCollider[] boxColliders; // 拖拽你的Box Collider到这里
    //[SerializeField]
    private Sprite spriteToRender; // 拖拽你想渲染的Sprite到这里
    private List<GameObject> spriteObjs = new List<GameObject>();

    private GridOperationManager gridOperationManager;

    public Vector2Int topLeftCellIndex;
    public Vector2Int bottomRightCellIndex;

    void Start()
    {
        gridOperationManager = FindObjectOfType<GridOperationManager>();

        if (gridOperationManager.enableObstacleMasks)
        {
            spriteToRender = gridOperationManager.ObstacleSprite;

            Rigidbody rb = GetComponent<Rigidbody>();

            boxColliders = gameObject.GetComponents<BoxCollider>();

            // 获取Box Collider的尺寸和位置
            //
            // Tilemap _tilemap = GameObject.Find("Placement Grid Canvas " + gridOperationManager._gridSettings.name)
            //     .transform.GetChild(0).transform.GetChild(0).GetComponent<Tilemap>();
            //
            foreach (BoxCollider boxCollider in boxColliders)
            {
                boxCollider.isTrigger = true;
                rb.useGravity = false;
                rb.isKinematic = true;


                // 创建一个新的GameObject作为Sprite
                GameObject spriteObj = new GameObject("GridMask - "+transform.name);

                GameObject gridMasks = GameObject.Find("GridMasks");
                if (gridMasks == null)
                {
                    gridMasks = new GameObject("GridMasks");
                }
                spriteObj.transform.SetParent(gridMasks.transform);
                spriteObjs.Add(spriteObj);
                SpriteMask imageMask = spriteObj.AddComponent<SpriteMask>();
                imageMask.sprite = spriteToRender;
                ObstacleMask obsMask = spriteObj.AddComponent<ObstacleMask>();

                float cellSize = GridUtilities.GetWorldSizeOfCell(gridOperationManager._gridSettings);

                Bounds colliderBounds = boxCollider.bounds;
                Vector3 minCorner = colliderBounds.min; // 碰撞箱的左下角
                Vector3 maxCorner = colliderBounds.max; // 碰撞箱的右上角

                topLeftCellIndex = gridOperationManager.GetCellIndexFromWorldPosition(new Vector3(minCorner.x, 0, maxCorner.z));
                bottomRightCellIndex = gridOperationManager.GetCellIndexFromWorldPosition(new Vector3(maxCorner.x, 0, minCorner.z));

                // for (int x = topLeftCellIndex.x; x < bottomRightCellIndex.x; x++)
                // {
                //     for (int y = bottomRightCellIndex.y; y < topLeftCellIndex.y; y++)
                //     {
                //         Vector3Int position = new Vector3Int(x, y, 0);
                //         _tilemap.SetTile(position, null);
                //         // _tilemap.RefreshTile(position);
                //     }
                // }
                Vector2 topLeftWorldPosition = gridOperationManager.GetWorldPositionFromCellIndex(topLeftCellIndex);
                Vector2 bottomRightWorldPosition = gridOperationManager.GetWorldPositionFromCellIndex(bottomRightCellIndex);

                Vector3 spriteSize = new Vector3(Mathf.Abs(bottomRightWorldPosition.x - topLeftWorldPosition.x) + cellSize, Mathf.Abs(topLeftWorldPosition.y - bottomRightWorldPosition.y) + cellSize, 1);
                Vector3 spritePosition = new Vector3((topLeftWorldPosition.x + bottomRightWorldPosition.x) / 2, gridOperationManager._gridSettings.GridPosition.y, (topLeftWorldPosition.y + bottomRightWorldPosition.y) / 2);



                // 定位Sprite到Box Collider的底部
                //spriteObj.transform.SetParent(this.transform);
                spriteObj.transform.rotation = Quaternion.Euler(90, 0, 0);
                spriteObj.transform.position = spritePosition;
                spriteObj.transform.localScale = spriteSize;

                //boxCollider.enabled = false;

            }

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

