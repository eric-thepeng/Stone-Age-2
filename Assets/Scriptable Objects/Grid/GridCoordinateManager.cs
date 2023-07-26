using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hypertonic.GridPlacement;

public class GridCoordinateManager : MonoBehaviour
{
    [SerializeField]
    private GridManager _gridManager;

    // [SerializeField]
    // private Text _gridCoordinates;

    public int coordinateX, coordinateY;

    private void Awake()
    {
        // _gridCoordinates.text = string.Empty;
    }

    private void OnEnable()
    {
        _gridManager.OnObjectPositionUpdated += HandleGridObjectPositionUpdated;
        _gridManager.OnGridObjectPlaced += HandleGridObjectPlaced;
    }

    private void OnDisable()
    {
        _gridManager.OnObjectPositionUpdated -= HandleGridObjectPositionUpdated;
        _gridManager.OnGridObjectPlaced -= HandleGridObjectPlaced;
    }

    private void HandleGridObjectPositionUpdated(Vector2Int coordinates)
    {
        // _gridCoordinates.text = string.Format("X: {0}  |  Y: {1}", coordinates.x, coordinates.y);
        coordinateX = coordinates.x;
        coordinateY = coordinates.y;
    }

    private void HandleGridObjectPlaced(GameObject gameObject)
    {
        // _gridCoordinates.text = string.Empty;
        coordinateX = -1;
        coordinateY = -1;

    }
}
