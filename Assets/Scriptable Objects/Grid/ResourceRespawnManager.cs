using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Hypertonic.GridPlacement;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class ResourceRespawnManager : MonoBehaviour
{
    public enum AvailableType
    {
        None = 0,
        DefaultTile = 1 << 0, // 1
        WaterTile = 1 << 1, // 2
        HillTile = 1 << 2 // 4
        //Option4 = 1 << 3  // 8
        // 可以继续添加更多选项
    }
    public static AvailableType GetAvailableTypeByName(string name)
    {
        if (name.Contains("DefaultTile"))
        {
            return AvailableType.DefaultTile;
        } else if (name.Contains("WaterTile"))
        {
            return AvailableType.WaterTile;
        } else if (name.Contains("HillTile"))
        {
            return AvailableType.HillTile;
        }
        else
        {
            return AvailableType.None;
        }
    }

    [Serializable]
    protected class RespawnRule
    {
        public List<GameObject> respawnPrefabList;
        public AvailableType availableType;
        
        [SerializeField]
        private int maximumAmount = 3;
        
        [SerializeField]
        private Vector2 interval = new Vector2(10f,10f);
    
        private List<GameObject> spawnedObjects = new List<GameObject>();
        private BoxCollider obstacleCollider;

        [SerializeField]
        private bool keepLooping;
        
        // 新增的倒计时协程
        // counting related
        public Action onCountdownComplete;
        public Action OnRespawnComplete;
        private Coroutine spawnCoroutine;
    
        public bool IsCountingDown { get; private set; } = false;
        private float countdownStartTime;
    
        public float ElapsedTime
        {
            get
            {
                if (!IsCountingDown) return 0;
                return (Time.time - countdownStartTime);
            }
        }
        
        public bool StartLoop(MonoBehaviour runner) {
            IsCountingDown = true;
            countdownStartTime = Time.time;
            // 开始倒计时
            if (spawnCoroutine != null)
            {
                // Debug.Log("Still in counting!");
                runner.StopCoroutine(spawnCoroutine);
                // return false;
            }
            
            // Debug.Log("spawn countdown started - " + availableType);
            spawnCoroutine = runner.StartCoroutine(SpawnCountdown(runner));
            return true;
        }
        
        private IEnumerator SpawnCountdown(MonoBehaviour runner)
        {
            yield return new WaitForSeconds(Random.Range(interval.x,interval.y)); // 等待设定的生长时间
    
            IsCountingDown = false;
            // AdjustObjToAction(wetObj, FinishAction); // 应用结束动作
    
            SpawnCountdownComplete(); 
        }

        private void SpawnCountdownComplete()
        {
            // Debug.Log("spawn countdown ended - " + availableType);
            spawnedObjects.RemoveAll(item => item == null);
            if (spawnedObjects.Count < maximumAmount)
            {
                onCountdownComplete?.Invoke();
            }
            
            if (keepLooping)
            {
                OnRespawnComplete?.Invoke();
            }
        }
        
        public void AddSpawnedObject(GameObject gameObject) {
            spawnedObjects.Add(gameObject);
        }
        //
        // private void AdjustObjToAction(GameObject go, DoAction da)
        // {
        //     if (go == null) return;
        //     if (da == DoAction.NONE)
        //     {
        //         return;
        //     }
        //     else if (da == DoAction.ENABLE)
        //     {
        //         go.SetActive(true);
        //     }
        //     else if (da == DoAction.DISABLE)
        //     {
        //         go.SetActive(false);
        //     }
        //     else if (da == DoAction.DESTROY)
        //     {
        //         Destroy(go);
        //     }
        // }
    
        // public void SetUpUnlockCost(ResourceSet resourceSet)
        // {
        //     waterCost = resourceSet;
        // }
    }

    // public class TileWithIndex
    // {
    //     private TileBase _tileBase;
    //     private Vector3Int _position;
    //
    //     public TileWithIndex(Vector3Int position, TileBase tileBase)
    //     {
    //         _tileBase = tileBase;
    //         _position = position;
    //     }
    //
    //     public TileBase TileBase
    //     {
    //         get => _tileBase;
    //         set => _tileBase = value;
    //     }
    //
    //     public Vector3Int Position
    //     {
    //         get => _position;
    //         set => _position = value;
    //     }
    // }


    [SerializeField] List<RespawnRule> respawnRules;
    private Dictionary<AvailableType, List<Vector3Int>> tileDictionary = new Dictionary<AvailableType, List<Vector3Int>>();

    [Header("Animation")] [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private float animationDuration;
    
    
    // 获取Tilemap组件的引用
    // public Tilemap respawnTileFab;
    
    private Tilemap tilemap;
    private GameObject obstacleContainer;
    
    protected virtual void Start()
    {
        obstacleContainer = new GameObject("Obstacles Container");
        obstacleContainer.transform.parent = transform;
    }

    public void Setup()
    {
        UpdateTileDictionary();
        
        for (int i = 0; i < respawnRules.Count; i++)
        {
            int stateIndex = i;
            
            respawnRules[i].onCountdownComplete += () =>
            {
                AttemptSpawnObject(stateIndex);
            };
            
            respawnRules[i].OnRespawnComplete += () =>
            {
                respawnRules[stateIndex].StartLoop(this);
            };
            
            respawnRules[i].StartLoop(this);
        }
    }
    
    public void UpdateTileDictionary()
    {
        if (tilemap == null)
            tilemap = FindObjectOfType<GridTilemapManager>().TypeTilemap;

        // 获取Tilemap的边界范围
        BoundsInt bounds = tilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                TileBase tileBase = tilemap.GetTile(pos);

                // 检查瓷砖类型
                if (tileBase != null)
                {
                    AvailableType tileType = GetAvailableTypeByName(tileBase.name);

                    // 如果字典中已经包含该类型，将瓷砖添加到列表中
                    if (tileDictionary.ContainsKey(tileType))
                    {
                        tileDictionary[tileType].Add(pos);
                    }
                    // 否则，创建新的列表并添加到字典中
                    else
                    {
                        List<Vector3Int> tileList = new List<Vector3Int>();
                        tileList.Add(pos);
                        tileDictionary.Add(tileType, tileList);
                    }
                }
            }
        }

    }

    public void AttemptSpawnObject(int index)
    {
        List<GameObject> respawnPrefabs = respawnRules[index].respawnPrefabList;
        AvailableType type = respawnRules[index].availableType;
        
        // Debug.Log("attempting spawn: " + index + " - " + type);
        
        bool spawned = false;
        int attemptedSpawnCount = 0;
        while (!spawned && attemptedSpawnCount < 5)
        {
            Vector3 spawnPosition = GetRandomPosition(type);
            GameObject spawnedObject = Instantiate(respawnPrefabs[Random.Range(0, respawnPrefabs.Count)], spawnPosition,
                Quaternion.identity);
            spawnedObject.transform.position = spawnPosition;
            spawnedObject.transform.parent = obstacleContainer.transform;
            
            BoxCollider obstacleCollider = spawnedObject.GetComponent<BoxCollider>();

            // 检查位置是否为空
            if (IsPositionEmpty(obstacleCollider))
            {
                respawnRules[index].AddSpawnedObject(spawnedObject);
                
                // Debug.Log("successfully spawn: " + spawnedObject);
                spawned = true;
            }
            else
            {
                // Debug.Log("failure when spawn: " + spawnedObject);
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

    
    public Vector3Int GetRandomTileByType(AvailableType availableType)
    {
        List<Vector3Int> availableTiles = new List<Vector3Int>();

        foreach (AvailableType type in Enum.GetValues(typeof(AvailableType)))
        {
            if (type != AvailableType.None && (availableType & type) == type)
            {
                if (tileDictionary.ContainsKey(type))
                {
                    availableTiles.AddRange(tileDictionary[type]);
                }
            }
        }

        if (availableTiles.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableTiles.Count);
            return availableTiles[randomIndex];
        }

        return new Vector3Int(-1, -1, -1); // No matching tiles found
    }
    
    // public Vector3Int GetRandomTileByType(AvailableType availableType)
    // {
    //     List<Vector3Int> availableTiles = new List<Vector3Int>();
    //
    //     if ((availableType & AvailableType.DefaultTile) != 0)
    //     {
    //         availableTiles.AddRange(tileDictionary[AvailableType.DefaultTile]);
    //     }
    //     if ((availableType & AvailableType.WaterTile) != 0)
    //     {
    //         availableTiles.AddRange(tileDictionary[AvailableType.WaterTile]);
    //     }
    //     if ((availableType & AvailableType.HillTile) != 0)
    //     {
    //         availableTiles.AddRange(tileDictionary[AvailableType.HillTile]);
    //     }
    //     
    //     if (availableTiles.Count > 0)
    //     {
    //         int randomIndex = Random.Range(0, availableTiles.Count);
    //         Debug.Log("count - " + randomIndex);
    //         return availableTiles[randomIndex];
    //     }
    //
    //     return new Vector3Int(-1,-1,-1); // No matching tiles found
    // }
    
    Vector3 GetRandomPosition(AvailableType type)
    {
        Vector3Int randomPosition = GetRandomTileByType(type);
        Vector3 tileWorldPosition = tilemap.GetCellCenterWorld(randomPosition);
        tileWorldPosition.y = 0;
        // Debug.Log("Random Position: " + randomPosition);
        return tileWorldPosition;
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
