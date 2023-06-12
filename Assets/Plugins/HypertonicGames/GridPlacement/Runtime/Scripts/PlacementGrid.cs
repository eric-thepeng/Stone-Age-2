using Hypertonic.GridPlacement.Enums;
using Hypertonic.GridPlacement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Hypertonic.GridPlacement
{
    public class PlacementGrid
    {
        public List<GameObject> GridObjects { get => _gridObjectsAssignedToGrid.Values.ToList(); }

        public GridCell[,] CellObjects { get; private set; }


        private GridSettings _gridSettings;

        private Func<GameObject, bool> _deleteGameObjectFn;

        private Dictionary<Vector2Int, GameObject> _gridObjectsAssignedToGrid = new Dictionary<Vector2Int, GameObject>();

        private bool _populatedGridCells = false;

        // A callback to tell the object that created this class when the grid has finished populating
        private Action _setupCallback;

        public PlacementGrid(GridSettings gridSettings, Func<GameObject, bool> deleteGameObjectFunc, Action setupCallback)
        {
            _gridSettings = gridSettings;
            _deleteGameObjectFn = deleteGameObjectFunc;
            _setupCallback = setupCallback;

            CellObjects = new GridCell[_gridSettings.AmountOfCellsX, _gridSettings.AmountOfCellsY];

            if(!Application.platform.Equals(RuntimePlatform.WebGLPlayer))
            {
                PopulateCellObjectsThreaded();
            }
            else
            {
                _ = PopulateCellObjects();
            }
        }

        private void PopulateCellObjectsThreaded()
        {
            Thread receiveThread = new Thread(() => _ = PopulateCellObjects());
            receiveThread.Start();
        }

        private async Task PopulateCellObjects()
        {
            for (int x = 0; x < _gridSettings.AmountOfCellsX; x++)
            {
                for (int y = 0; y < _gridSettings.AmountOfCellsY; y++)
                {
                    var gridCell = (GridCell)Activator.CreateInstance(typeof(GridCell), new object[] { });

                    gridCell.Initialise(x, y);

                    CellObjects[x, y] = gridCell;
                }
            }

            await Task.Delay(0);
            _setupCallback?.Invoke();
            _populatedGridCells = true;
        }

        /// <summary>
        /// Returns the world space position of the nearest cell to the provided world space position
        /// </summary>
        /// <param name="gridCellIndex">The grid cell index</param>
        /// <param name="gridSettings">The grid settings</param>
        /// <returns></returns>
        public static Vector3 GetCellPositionFromGridCellIndex(Vector2 gridCellIndex, GridSettings gridSettings, Vector3 gridPosition, float gridRotation)
        {
            float gridOffsetX = (float)gridSettings.AmountOfCellsX / 2;
            float gridOffsetY = (float)gridSettings.AmountOfCellsY / 2;

            float cellSize = gridSettings.CellSize;

            float xPos = (gridCellIndex.x - gridOffsetX) * cellSize + cellSize / 2;
            float yPos = (gridCellIndex.y - gridOffsetY) * cellSize + cellSize / 2;

            Vector3 gridPositionWithRotation = Quaternion.Euler(0, -gridRotation, 0) * (gridPosition);

            return (new Vector3(xPos, 0, yPos) + gridPositionWithRotation);
        }

        public static Vector2Int GetCellIndexFromWorldPosition(Vector3 worldPosition, GridSettings gridSettings, float gridRotation, Vector3 gridPosition)
        {
            worldPosition.x -= gridPosition.x;
            worldPosition.z -= gridPosition.z;

            worldPosition = (Quaternion.Euler(0, -gridRotation, 0) * (worldPosition));


            double gridOffsetX = (double)gridSettings.AmountOfCellsX / 2;
            double gridOffsetY = (double)gridSettings.AmountOfCellsY / 2;

            float worldSizeOfCell = GridUtilities.GetWorldSizeOfCell(gridSettings);
            
            double worldPosX = worldPosition.x / worldSizeOfCell;
            double worldPosXWithOffset = worldPosX + gridOffsetX;
            int x = Mathf.FloorToInt((float)worldPosXWithOffset);

            double worldPosY = worldPosition.z / worldSizeOfCell;
            double worldPosYWithOffset = worldPosY + gridOffsetY;
            int y = Mathf.FloorToInt((float)worldPosYWithOffset);

            return new Vector2Int(x, y);
        }

        /// <summary>
        /// Determine if the cells that the object is placed on are available
        /// </summary>
        /// <param name="gridCellOfObject"> This represents the XY of the grid cell that the 
        /// object is snapped to. Not including the offset applied. </param>
        /// <param name="objectSize"></param>
        /// <returns></returns>
        public PlacementValidResponse CanPlaceObject(Vector2Int gridCellOfObject, Vector3 objectSize)
        {
            float cellSize = _gridSettings.CellSize;

            List<Vector2Int> cellIndexsRequired = GridUtilities.GetCellIndexesRequiredForObject(gridCellOfObject, objectSize, cellSize);

            for(int i = 0; i < cellIndexsRequired.Count; i++)
            {
                Vector2Int gridCellIndex = cellIndexsRequired[i];

                if (gridCellIndex.x >= CellObjects.GetLength(0)
                    || gridCellIndex.x < 0
                    || gridCellIndex.y >= CellObjects.GetLength(1)
                    || gridCellIndex.y < 0)
                {
                    return new PlacementValidResponse(false, PlacementInvalidReason.OUTSIDE_GRID);
                }

                if (!GetGridCellFromIndex(gridCellIndex).IsAvailable())
                {
                    return new PlacementValidResponse(false, PlacementInvalidReason.OVERLAPPING_CELL);
                }
            }

            return new PlacementValidResponse(true, null);
        }

        /// <summary>
        /// Calculated how many grid cells the object will require to be placed alonng the X and Y
        /// </summary>
        /// <param name="objectSize"> The size of the object that is being placed</param>
        /// <param name="cellSize">How big each cell is in the grid in Unity Worldspace </param>
        public static Vector2 CalculateGridCellSpanForObject(Vector3 objectSize, float cellSize)
        {
            int objectCellSpanX;
            int objectCellSpanY;

            // Prevents Unity's rounding error. Else objectSize.X could be 1.000001 and could as 2 cells.
            float objectSizeX = Mathf.Round(objectSize.x * 10000) / 10000;
            float objectSizeZ = Mathf.Round(objectSize.z * 10000) / 10000;

            if (objectSizeX > cellSize)
            {
                float cellSpanX = objectSizeX / cellSize;

                objectCellSpanX = Mathf.CeilToInt(cellSpanX);
            }
            else
            {
                objectCellSpanX = 1;
            }

            if (objectSizeZ > cellSize)
            {
                float cellSpanY = objectSizeZ / cellSize;
                objectCellSpanY = Mathf.CeilToInt(cellSpanY);
            }
            else
            {
                objectCellSpanY = 1;
            }

            return new Vector2(objectCellSpanX, objectCellSpanY);
        }

        public bool AddObjectToGrid(GameObject gridObject, Vector2Int gridCellOfObject, Vector3 objectSize)
        {
            // Store ref to the object in the main cell
            GridCell gridCell = GetGridCellFromIndex(gridCellOfObject);
            gridCell.SetObject(gridObject);

            if(_gridObjectsAssignedToGrid.ContainsKey(gridCellOfObject))
            {
                Debug.LogErrorFormat("Cannot add gameobject {0} to the grid as it's already assigned to the grid. Please ensure grid objects are removed correctly before adding them again.", gridObject.name);
                return false;
            }

            _gridObjectsAssignedToGrid.Add(gridCellOfObject, gridObject);

            List<Vector2Int> indexesOfRequiredCells = GridUtilities.GetCellIndexesRequiredForObject(gridCellOfObject, objectSize, _gridSettings.CellSize);

            for(int i = 0; i < indexesOfRequiredCells.Count; i++)
            {
                GridCell neibourCell = GetGridCellFromIndex(indexesOfRequiredCells[i]);
                neibourCell.SetObject(gridObject);
                gridCell.AddNeighbourIndex(neibourCell);
            }

            return true;
        }

        public bool DeleteGridObject(Vector2Int gridCellIndex)
        {
            GridCell gridCell = GetGridCellFromIndex(gridCellIndex);

            GameObject gameObject = gridCell.GetGridObject();
            bool deleteSuccess = _deleteGameObjectFn(gameObject);

            if(!deleteSuccess)
            {
                Debug.LogError("Error: Failed to delete gameobject: " + gameObject.name);
                return false;
            }

            _gridObjectsAssignedToGrid.Remove(gridCellIndex);
            gridCell.Clear();


            return true;
        }

        public void RemoveGridObject(Vector2Int gridCellIndex)
        {
            GridCell gridCell = GetGridCellFromIndex(gridCellIndex);
            _gridObjectsAssignedToGrid.Remove(gridCellIndex);
            gridCell.Clear();
        }

        public void SetGridObjectCellsAvailablity(Vector2Int cellIndex, bool available)
        {
            var cell = GetGridCellFromIndex(cellIndex);
            cell.SetAvailabilty(available);

            List<GridCell> cellNeighbours = cell.Neighbours;

            foreach(var c in cellNeighbours)
            {
                c.SetAvailabilty(available);
            }
        }

        public GridCell GetGridCellFromIndex(Vector2Int gridCell)
        {
            if(!_populatedGridCells)
            {
                Debug.LogErrorFormat("The grid has not yet finished setting up");
                return null;
            }

            return CellObjects[gridCell.x, gridCell.y];
        }
    }
}