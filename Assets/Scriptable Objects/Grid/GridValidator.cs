using System;
using Hypertonic.GridPlacement;
using Hypertonic.GridPlacement.GridObjectComponents;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

/// <summary>
    /// This is an example of a validator you may add to the gameobject you're placing.
    /// This example component is being used to detect if the object is colliding with the wall gameobjects 
    /// in the scene. If it is, it will mark the component as having an invalid placement.
    /// </summary>
    [RequireComponent(typeof(CustomValidator))]
public class GridValidator : MonoBehaviour
{
    private CustomValidator _customValidator;
    //private bool _collisionStay = false;

    private BoxCollider _boxCollider;
    Bounds _bounds;
    private Tilemap tilemap;

    private GridSettings _gridSettings;
    private PlaceableObject _placeableObject;
    
    private void Awake()
    {
        _customValidator = GetComponent<CustomValidator>();
        _boxCollider = GetComponent<BoxCollider>();
        _gridSettings = GridManagerAccessor.GridManager.GridSettings;
        _placeableObject = GetComponent<PlaceableObject>();
    }

    private void Start()
    {
        
        FindTilemap();
        
    }

    /// <summary>
    /// We will check what object we hit. If it a wall object we'll set the 
    /// custom validation to be invalid.
    /// </summary>
    /// <param name="other"></param>



    private Vector2Int _startCell;
    private Vector2Int _endCell;
    private Vector3Int _position;
    private TileBase _tile;

    private bool _validation = false;
    private bool _lastFrameValidation = false;

    public void FindTilemap()
    {
        tilemap = GameObject.Find("Tilemap - " + _gridSettings.name).GetComponent<Tilemap>();
    }
    
    private void Update()
    {
        if (_validation != _lastFrameValidation)
        {
            _customValidator.SetValidation(_validation);
            _lastFrameValidation = _validation;
        }

        // _bounds = _boxCollider.bounds;
        // if (GridManagerAccessor.GridManager.IsPlacingGridObject)
        // {
        _startCell = GetCellIndexFromWorldPosition(_gridSettings,_boxCollider.bounds.min);
        _endCell = GetCellIndexFromWorldPosition(_gridSettings,_boxCollider.bounds.max);
        
        // Debug.Log(startCell + " , " + endCell);
        
        for (int x = _startCell.x; x < _endCell.x; x++)
        {
            for (int y = _startCell.y; y < _endCell.y; y++)
            {
                _position = new Vector3Int(x, y, 0);
                _tile = tilemap.GetTile(_position);

                if (_tile == null || (_placeableObject.biomeType & PlaceableObject.GetBiomeTypeByName(_tile.name)) == 0)
                {
                    _validation = false;
                    return;
                }
                //
                // if (_tile == null)
                // {
                //     _validation = false;
                //     return;
                // }
            }
        }
        _validation = true;

        // }
    
    }

    public Vector2Int GetCellIndexFromWorldPosition(GridSettings gridSettings, Vector3 WorldPosition)
    {
        Vector3 _gridCenterPosition = gridSettings.GridPosition;

        int cellIndexX = Mathf.FloorToInt((WorldPosition.x - (_gridCenterPosition.x - gridSettings.AmountOfCellsX * gridSettings.CellSize / 2)) / gridSettings.CellSize);
        int cellIndexZ = Mathf.FloorToInt((WorldPosition.z - (_gridCenterPosition.z - gridSettings.AmountOfCellsY * gridSettings.CellSize / 2)) / gridSettings.CellSize);

        return new Vector2Int(cellIndexX, cellIndexZ);
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.GetComponent<Obstacle>() != null || other is TerrainCollider)
    //     {
    //         collisionCount++;
    //         HandleEnteredWallArea();
    //     }
    // }
    //
    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.gameObject.GetComponent<Obstacle>() != null || other is TerrainCollider)
    //     {
    //         collisionCount--;
    //         if (collisionCount == 0)
    //         {
    //             HandleExitedWallArea();
    //         }
    //     }
    // }


    public void HandleEnteredWallArea()
    {
        _customValidator.SetValidation(false);
    }

    public void HandleExitedWallArea()
    {
        _customValidator.SetValidation(true);
    }

    // private void CheckTerrainCollision()
    // {
    //     RaycastHit hit;
    //     if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
    //     {
    //         if (hit.collider is TerrainCollider && !wasOnTerrainLastFrame)
    //         {
    //             HandleExitedWallArea();
    //             wasOnTerrainLastFrame = true;
    //             //Debug.Log("Exited the terrin collider");
    //         }
    //     }
    //     else if (wasOnTerrainLastFrame)
    //     {
    //         HandleEnteredWallArea();
    //         wasOnTerrainLastFrame = false;
    //         //Debug.Log("Entered the terrin collider");
    //     }
    // }

}
