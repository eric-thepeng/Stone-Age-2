using System.Collections;
using UnityEngine;
using Hypertonic.GridPlacement;

public class GridOperationManager : MonoBehaviour
{
    //[SerializeField]

    [Header("Grid Manager Related")]
    public GridSettings _gridSettings;

    [SerializeField]
    private GameObject _gridEmptyObjectPrefab;

    public GridManager _gridManager;

    [Header("Nearby Objects Related")]
    public bool operateStatus;
    public GameObject itemPlaced;

    [Header("Obstacle Masks")]
    public bool enableObstacleMasks;
    public Sprite ObstacleSprite;

    private void Start()
    {
        if(_gridSettings == null)
        {
            Debug.LogError("The GameManager needs to have the grid settings assigned.");
            return;
        }

        if(_gridEmptyObjectPrefab == null)
        {
            Debug.LogError("The GameManager needs to have the grid object prefab assigned.");
            return;
        }

        _gridManager = new GameObject("Grid Manager").AddComponent<GridManager>();
        _gridManager.Setup(_gridSettings);

        //StartCoroutine(CheckForInput());
        AddGridCoordinateManager();
        AddMaskGenerateManager();

    }


    public void SwitchPaintMode()
    {
        if (GridManagerAccessor.GridManager.IsPlacingGridObject)
        {
            EndPaintMode();
        } else
        {
            StartPaintMode();
        }
    }

    public void StartPaintMode()
    {
        Debug.Log("Paint mode entered");
        _gridManager.StartPaintMode(_gridEmptyObjectPrefab);
    }

    public void EndPaintMode()
    {
        _gridManager.EndPaintMode();
    }

    private void AddGridCoordinateManager()
    {
        if (GetComponent<GridCoordinateManager>() == null)
        {
            gameObject.AddComponent<GridCoordinateManager>();

        }
    }

    private void AddMaskGenerateManager()
    {
        if (GetComponent<MaskGenerateManager>() == null)
        {
            gameObject.AddComponent<MaskGenerateManager>().defaultSizePrefab = _gridEmptyObjectPrefab;


        }
    }

    public Vector2Int GetCellIndexFromWorldPosition(Vector3 WorldPosition)
    {
        Vector3 _gridCenterPosition = _gridSettings.GridPosition;

        int cellIndexX = Mathf.FloorToInt((WorldPosition.x - (_gridCenterPosition.x - _gridSettings.AmountOfCellsX * _gridSettings.CellSize / 2)) / _gridSettings.CellSize);
        int cellIndexZ = Mathf.FloorToInt((WorldPosition.z - (_gridCenterPosition.z - _gridSettings.AmountOfCellsY * _gridSettings.CellSize / 2)) / _gridSettings.CellSize);

        return new Vector2Int(cellIndexX, cellIndexZ);
    }

    public Vector2 GetWorldPositionFromCellIndex(Vector2 CellIndex)
    {
        double posX = (_gridSettings.GridPosition.x - _gridSettings.AmountOfCellsX * _gridSettings.CellSize / 2) + (CellIndex.x + 0.5) * _gridSettings.CellSize;
        double posY = (_gridSettings.GridPosition.z - _gridSettings.AmountOfCellsY * _gridSettings.CellSize / 2) + (CellIndex.y + 0.5) * _gridSettings.CellSize;

        return new Vector2((float)posX, (float)posY);
    }

//    private IEnumerator CheckForInput()
//    {
//        while (true)
//        {

//            yield return null;

//            // Don't check for input if the grid manager isn't placing an object
//            if (!_gridManager.IsPlacingGridObject)
//                continue;

//            // Check input from new Input System
//#if ENABLE_INPUT_SYSTEM
//            if (UnityEngine.InputSystem.Mouse.current.leftButton.isPressed)
//            {
//                _gridManager.ConfirmPlacement();
//            }
//#endif


//            // Check input from old Input System
//#if ENABLE_LEGACY_INPUT_MANAGER
//        if (Input.GetMouseButton(0) && !GridManagerAccessor.GridManager.ObjectToPlace.GetComponent<GridObjectTags>().containsTag("EmptyObject"))
//        {
//                //Debug.Log(_gridManager.ConfirmPlacement());
//            //    itemPlaced = _gridManager.ObjectToPlace;
//            //operateStatus = _gridManager.ConfirmPlacement();
//        }
//#endif
//        }
//    }
}
