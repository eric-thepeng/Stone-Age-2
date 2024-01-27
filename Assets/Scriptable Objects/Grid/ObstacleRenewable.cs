using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Hypertonic.GridPlacement;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ObstacleRenewable : MonoBehaviour
{
    [SerializeField]
    private Vector2 interval = new Vector2(10f,10f); // 初始倒计时时间
    
    [SerializeField]
    private int maxObjects = 3; // 最大对象数量

    [SerializeField]
    private int generationRange = 10;
    
    
    private Tilemap tilemap; // 用于检查位置是否为空的Tilemap
    
    public List<GameObject> respawnPrefabList; // 存储Prefab的列表
    
    
    [Header("Animation")] [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private float animationDuration;

    private List<GameObject> spawnedObjects = new List<GameObject>(); // 存储已生成的对象列表

    private float countdownTimer;

    private BoxCollider obstacleCollider;

    void Start()
    {
        // need to change to find tilemap
        countdownTimer = Random.Range(interval.x,interval.y);
        
    }

    // Update is called once per frame
    void Update()
    {
        countdownTimer -= Time.deltaTime;

        if (countdownTimer <= 0f)
        {
            spawnedObjects.RemoveAll(item => item == null);
            if (spawnedObjects.Count < maxObjects)
            {
                bool spawned = false;
                int attemptedSpawnCount = 0;
                while (!spawned && attemptedSpawnCount < 5)
                {
                    Vector3 spawnPosition = transform.position + GetRandomPosition();
                    GameObject spawnedObject = Instantiate(respawnPrefabList[Random.Range(0, respawnPrefabList.Count)], spawnPosition,
                        Quaternion.identity);
                    spawnedObject.transform.position = spawnPosition;
                    spawnedObject.transform.parent = GameObject.Find("Obstacles Container").transform;
                    obstacleCollider = spawnedObject.GetComponent<BoxCollider>();

                    // 检查位置是否为空
                    if (IsPositionEmpty(obstacleCollider))
                    {
                        spawnedObjects.Add(spawnedObject);
                        
                        spawned = true;
                    }
                    else
                    {
                        Destroy(spawnedObject);
                        attemptedSpawnCount++;
                    }
                    
                    if (spawned)
                    {
                        Vector3 originalScale = spawnedObject.transform.localScale;
                        DOVirtual.Float(0f, 1f, animationDuration, (float value) =>
                        {
                            float scaleValue = animationCurve.Evaluate(value);
                            Vector3 newScale = spawnedObject.transform.localScale;
                            newScale.x = originalScale.x * scaleValue;
                            newScale.y = originalScale.y * scaleValue;
                            newScale.z = originalScale.z * scaleValue;
                            spawnedObject.transform.localScale = newScale;
                        }).OnComplete(() =>
                        {
                            // 动画完成后恢复原始尺寸
                            spawnedObject.transform.localScale = originalScale;
                        });

                    }
                }
                
            }
            
            countdownTimer = Random.Range(interval.x,interval.y);
            
        }
    }
    
    
    Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = new Vector3(Random.Range(-generationRange, generationRange), 0f, Random.Range(-generationRange, generationRange));
        // Debug.Log("Random Position: " + randomPosition);
        return randomPosition;
    }

    bool IsPositionEmpty(BoxCollider boxCollider)
    {
        if (tilemap == null)
            tilemap = FindObjectOfType<GridTilemapManager>().TypeTilemap;
        Vector2Int startCell = GetCellIndexFromWorldPosition(GridManagerAccessor.GridManager.GridSettings,boxCollider.bounds.min);
        Vector2Int endCell = GetCellIndexFromWorldPosition(GridManagerAccessor.GridManager.GridSettings,boxCollider.bounds.max);
        
        // Debug.Log(startCell + " , " + endCell);
        
        for (int x = startCell.x; x < endCell.x; x++)
        {
            for (int y = startCell.y; y < endCell.y; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                // _tile = tilemap.GetTile(position);

                if (tilemap.GetTile(pos) == null)
                {
                    // Debug.Log("tilemap: " + pos + " is null");
                    return false;
                }
            }
        }

        return true;

        // }
    }
    
    
    public Vector2Int GetCellIndexFromWorldPosition(GridSettings gridSettings, Vector3 WorldPosition)
    {
        Vector3 _gridCenterPosition = gridSettings.GridPosition;

        int cellIndexX = Mathf.FloorToInt((WorldPosition.x - (_gridCenterPosition.x - gridSettings.AmountOfCellsX * gridSettings.CellSize / 2)) / gridSettings.CellSize);
        int cellIndexZ = Mathf.FloorToInt((WorldPosition.z - (_gridCenterPosition.z - gridSettings.AmountOfCellsY * gridSettings.CellSize / 2)) / gridSettings.CellSize);

        return new Vector2Int(cellIndexX, cellIndexZ);
    }
}
