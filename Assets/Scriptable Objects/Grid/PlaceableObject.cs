using System;
using System.Collections;
using System.Collections.Generic;
using Hypertonic.GridPlacement;
using Hypertonic.GridPlacement.CustomSizing;
using Hypertonic.GridPlacement.Enums;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(NavMeshObstacle))]
[RequireComponent(typeof(GridHeightPositioner))]
[RequireComponent(typeof(GridValidator))]

public class PlaceableObject : MonoBehaviour
{
    [Flags] // 标记这个枚举可以作为位掩码使用
    public enum BiomeType
    {
        None = 0,
        DefaultTile = 1 << 0, // 1
        WaterTile = 1 << 1, // 2
        HillTile = 1 << 2, // 4
        LavaTile = 1 << 3 // 8
        //Option6 = 1 << 3  // 16
        // 可以继续添加更多选项
    }

    public static BiomeType GetBiomeTypeByName(string name)
    {
        if (name.Contains("DefaultTile"))
        {
            return BiomeType.DefaultTile;
        } else if (name.Contains("WaterTile"))
        {
            return BiomeType.WaterTile;
        } else if (name.Contains("HillTile"))
        {
            return BiomeType.HillTile;
        } else if (name.Contains("LavaTile"))
        {
            return BiomeType.LavaTile;
        }
        else
        {
            return BiomeType.None;
        }
    }
    
    public BiomeType biomeType;
    public string GridKey { get; private set; }

    [Header("Runtime Info")]
    [SerializeField]
    private Vector2 gridCellIndex;

    public Vector2 GridCellIndex
    {
        get { return gridCellIndex; }
        private set { gridCellIndex = value; }
    }

    public ObjectAlignment ObjectAlignment { get; private set; }

    public Vector3? ObjectBounds { get; set; }

    public Vector3? PositionOffset { get; set; }

    public bool HasBeenPlaced => !string.IsNullOrEmpty(GridKey);

    public void Setup(string gridKey, Vector2Int gridCellIndex, ObjectAlignment objectAlignment)
    {
        GridKey = gridKey;
        GridCellIndex = gridCellIndex;
        ObjectAlignment = objectAlignment;
    }

    [Header("BISO Settings")]
    [SerializeField]
    private BuildingISO BISO;

    private List<string> _gridObjectTags = new List<string>();


    [Header("Character Settings")] 
    [SerializeField] private bool isOccupiedByCharacter; 
    [SerializeField] private Character occupiedCharacter;
    // [SerializeField] private float remainOccupyTime;

    [Header("Character Tasks")] 
    [SerializeField] private float taskDuration;
    [SerializeField] private int charRewardPoint;
    [SerializeField] private Action finishedEvent;

    public bool IsOccupiedByCharacter
    {
        get => isOccupiedByCharacter;
        set => isOccupiedByCharacter = value;
    }

    public Character OccupiedCharacter
    {
        get => occupiedCharacter;
        set => occupiedCharacter = value;
    }

    public float TaskDuration
    {
        get => taskDuration;
        set => taskDuration = value;
    }

    // public void Occupy(Character character, float occupyTime)
    // {
    //     isOccupiedByCharacter = true;
    //     occupiedCharacter = character;
    //
    //     StartCoroutine(OccupationCountdown(occupyTime)); // 启动协程
    // }
    //
    // private IEnumerator OccupationCountdown(float duration)
    // {
    //     remainOccupyTime = duration;
    //     while (remainOccupyTime > 0)
    //     {
    //         yield return new WaitForSeconds(0.1f); // 等待1秒
    //         remainOccupyTime -= 0.1f; // 更新剩余时间
    //     }
    //
    //     // 倒计时结束
    //     isOccupiedByCharacter = false;
    //     occupiedCharacter = null; // 或者保留角色引用，取决于你的需求
    //     remainOccupyTime = 0;
    //
    //     finishedEvent?.Invoke(); // 调用完成事件
    //     SpiritPoint.i.Add(charRewardPoint);
    // }

    public bool containsTag(string ObjectTag)
    {
        if (_gridObjectTags.Contains(ObjectTag)) return true;
        else return false;
    }

    public BuildingISO GetBuildingISO()
    {
        return BISO;
    }


    private List<GameObject> objectsWithEffects = new List<GameObject>();

    private BuildingManager buildingManager;

    private BoxCollider boxCollider;
    // private Sprite spriteToRender; // 拖拽你想渲染的Sprite到这里

    private GridOperationManager gridOperationManager;

    private GridHeightPositioner gridHeightPositioner;

    public enum Direction
    {
        Forward,
        Backward,
        Left,
        Right
    }

    public Direction direction; // 通过Unity编辑器选择方向
    public float offset = 0.5f; // 边框外偏移量
    public Vector3 characterInteractionPoint;
    // public GameObject characterInteractionObject;
    
    private void Start()
    {
        gridHeightPositioner = GetComponent<GridHeightPositioner>();
        gridOperationManager = FindObjectOfType<GridOperationManager>();

        buildingManager = FindObjectOfType<BuildingManager>();
        CheckEffects(transform);
        DisableEffects();
        GetComponent<NavMeshObstacle>().enabled = false;

        boxCollider = GetComponent<BoxCollider>();

        if (BISO != null)
        {
            _gridObjectTags = BISO.gridObjectTags;
        }
        else
        {
            _gridObjectTags.Add("EmptyObject");
        }
        
    }

    public Vector3 GetInteractionPoint()
    {
        
        Vector3 newPosition = transform.position + boxCollider.center;

        // 根据碰撞体的大小和选定的方向计算偏移
        switch (direction)
        {
            case Direction.Forward:
                newPosition += transform.forward * (boxCollider.size.z / 2 + offset);
                break;
            case Direction.Backward:
                newPosition -= transform.forward * (boxCollider.size.z / 2 + offset);
                break;
            case Direction.Left:
                newPosition -= transform.right * (boxCollider.size.x / 2 + offset);
                break;
            case Direction.Right:
                newPosition += transform.right * (boxCollider.size.x / 2 + offset);
                break;
        }

        // newPosition.y = 0;
        
        // 更新GameObject的位置
        // characterInteractionObject.transform.position = newPosition;
        return newPosition;
    }
    

    void CheckEffects(Transform parent)
    {
        foreach (Transform child in parent)
        {
            VisualEffect visualEffect = child.GetComponent<VisualEffect>();
            ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
            if ((child.GetComponent<VisualEffect>() != null && visualEffect.enabled == true)
                || particleSystem != null && particleSystem.gameObject.activeSelf)
            {
                objectsWithEffects.Add(child.gameObject);
            }


            if (child.childCount > 0)
            {
                CheckEffects(child);
            }
        }
    }
    public void DisableEffects()
    {
        GetComponent<NavMeshObstacle>().enabled = false;
        if (objectsWithEffects != null)
        {
            foreach (GameObject obj in objectsWithEffects)
            {
                VisualEffect visualEffect = obj.GetComponent<VisualEffect>();
                if (visualEffect != null)
                {
                    visualEffect.enabled = false;
                }

                ParticleSystem particleSystem = obj.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    obj.SetActive(false);
                    //particleSystem.Play();
                }
            }

            //objectsWithEffects.Clear();
        }
    }

    public void EnableEffects()
    {
        GetComponent<NavMeshObstacle>().enabled = true;
        if (objectsWithEffects != null)
        {
            foreach (GameObject obj in objectsWithEffects)
            {
                VisualEffect visualEffect = obj.GetComponent<VisualEffect>();
                if (visualEffect != null)
                {
                    visualEffect.enabled = true;
                }

                ParticleSystem particleSystem = obj.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    obj.SetActive(true);
                    //particleSystem.Play();
                }
            }

        }
    }

    public void InvokeFinishedWorkEvent()
    {
        finishedEvent?.Invoke();
        SpiritPoint.i.Add(charRewardPoint);
    }





}