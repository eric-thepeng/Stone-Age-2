using Hypertonic.GridPlacement.GridInput;
using Hypertonic.GridPlacement.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Hypertonic.GridPlacement
{
    /// <summary>
    /// This class provides useful functions related to the Grid.
    /// </summary>
    public class GridUtilities
    {
        /// <summary>
        /// Function to determine the center of an area from the worldspace position. Get Cell position does not work if the area is even and greate than 1 in either dimension
        /// </summary>
        /// <param name="worldSpaceOfSelectedGridCell">The position in world space </param>
        /// <param name="objectSize"> The Size of the object</param>
        /// <param name="objectAlignment"> The alignment of the object </param>
        /// <param name="gridSettings">The grid settings</param>
        /// <returns> The center of the grid area</returns>
        public static Vector3 GetCenterOfGridArea(Vector3 worldSpaceOfSelectedGridCell, Vector3 objectSize, GridSettings gridSettings)
        {
            Vector2Int areaRequired = CalculateGridCellAreaRequiredForObject(objectSize, gridSettings.CellSize);

            if (areaRequired.x % 2 == 0)
            {
                worldSpaceOfSelectedGridCell.x -= gridSettings.CellSize / 2;
            }

            if (areaRequired.y % 2 == 0)
            {
                worldSpaceOfSelectedGridCell.z -= gridSettings.CellSize / 2;
            }

            return worldSpaceOfSelectedGridCell;
        }

        /// <summary>
        /// Returns the position in world space from a grid cell index
        /// </summary>
        /// <param name="cellIndex">The index of the grid cell</param>
        /// <param name="gridSettings">The grid settings </param>
        /// <returns>The world space position of the grid cell </returns>
        public static Vector3 GetWorldPositionFromCellIndex(GridManager gridManager, Vector2Int cellIndex, GridSettings gridSettings, Vector3 gridPosition)
        {
            float worldSizeOfCell = GetWorldSizeOfCell(gridSettings);

            double worldX = (cellIndex.x * worldSizeOfCell) - (gridSettings.Width / 2) + (worldSizeOfCell / 2);
            double worldZ = (cellIndex.y * worldSizeOfCell) - (gridSettings.Height / 2) + (worldSizeOfCell / 2);

            Vector3 worldPos = new Vector3((float)worldX, 0, (float)worldZ);

            worldPos = Quaternion.Euler(0, gridManager.RuntimeGridRotation, 0) * (worldPos);

            // Apply the grids position after the world position of the grid cell has been calculated.
            worldPos += gridPosition;

            return worldPos;
        }

        /// <summary>
        /// Gets the size of the grid cell relative to Unity's world space
        /// </summary>
        /// <param name="gridSettings">The grid settings </param>
        /// <returns>The size of a cell (x & y) in Unity's worldspace scale </returns>
        public static float GetWorldSizeOfCell(GridSettings gridSettings) => gridSettings.CellSize;

        /// <summary>
        /// Return the exact amount of Grid cells that the grid object takes up.
        /// </summary>
        /// <param name="objectSize">The size of the object</param>
        /// <param name="cellSize">The size of the cell </param>
        /// <returns></returns>
        public static Vector2 CalculateCellSpanOfGridObject(Vector3 objectSize, float cellSize)
        {
            // Prevents Unity's rounding error. Else objectSize.X could be 1.000001 and could as 2 cells.
            float objectSizeX = Mathf.Round(objectSize.x * 10000) / 10000;
            float objectSizeZ = Mathf.Round(objectSize.z * 10000) / 10000;

            return new Vector2(objectSizeX / cellSize, objectSizeZ / cellSize);
        }

        /// <summary>
        /// This function is responsible for calculating the actual area required based on the cellspan of the object and it's alignment
        /// If an object is centered on an axis it'll need an extra cell for padding
        /// </summary>
        /// <param name="objectBounds">The size of the object</param>
        /// <param name="cellSize">The size of the cell</param>
        /// <returns> The area required to fit a grid object in. </returns>
        public static Vector2Int CalculateGridCellAreaRequiredForObject(Vector3 objectBounds, float cellSize)
        {
            Vector2 cellSpanOfObject = CalculateCellSpanOfGridObject(objectBounds, cellSize);

            float xSizeRequired = cellSpanOfObject.x;
            float zSizeRequired = cellSpanOfObject.y;

            // Workout if the object fits perfectly or needs rounding up
            float diff = cellSize - (xSizeRequired % cellSize);
            var diffRounded = Mathf.Round(diff * 10000) / 10000;
            var similar = Mathf.Approximately(diffRounded, 0f);

            bool perfectFitX = (xSizeRequired % cellSize == 0) || similar;

            if (!perfectFitX)
            {
                xSizeRequired = Mathf.CeilToInt(xSizeRequired);
            }

            float diffz = cellSize - (zSizeRequired % cellSize);
            var diffRoundedz = Mathf.Round(diffz * 10000) / 10000;
            var similarz = Mathf.Approximately(diffRoundedz, 0f);


            bool perfectFitZ = (zSizeRequired % cellSize == 0) || similarz;

            if (!perfectFitZ)
            {
                zSizeRequired = Mathf.CeilToInt(zSizeRequired);
            }

            return new Vector2Int((int)xSizeRequired, (int)zSizeRequired);
        }

        public static Vector3MinMax GetVector3MinMax(List<Vector3> vectorList)
        {
            if(vectorList == null || vectorList.Count == 0)
            {
                return new Vector3MinMax(Vector3.zero, Vector3.zero);
            }

            float minX = vectorList[0].x, maxX = vectorList[0].x, minY = vectorList[0].y, maxY = vectorList[0].y, minZ = vectorList[0].z, maxZ = vectorList[0].z;

            for (int i = 1; i < vectorList.Count; i++)
            {
                var data = vectorList[i];

                if (data.x < minX)
                {
                    minX = data.x;
                }

                if (data.x > maxX)
                {
                    maxX = data.x;
                }

                if (data.y < minY)
                {
                    minY = data.y;
                }

                if (data.y > maxY)
                {
                    maxY = data.y;
                }

                if (data.z < minZ)
                {
                    minZ = data.z;
                }

                if (data.z > maxZ)
                {
                    maxZ = data.z;
                }
            }

            return new Vector3MinMax(
                new Vector3(minX, minY, minZ),
                new Vector3(maxX, maxY, maxZ));
        }

        public static Vector2IntMinMax GetVector2IntMinMax(List<Vector2Int> vectorList)
        {
            if(vectorList == null || vectorList.Count == 0)
            {
                return new Vector2IntMinMax(Vector2Int.zero, Vector2Int.zero);
            }

            int minX = vectorList[0].x, maxX = vectorList[0].x, minY = vectorList[0].y, maxY = vectorList[0].y;

            for (int i = 1; i < vectorList.Count; i++)
            {
                var data = vectorList[i];

                if (data.x < minX)
                {
                    minX = data.x;
                }

                if (data.x > maxX)
                {
                    maxX = data.x;
                }

                if (data.y < minY)
                {
                    minY = data.y;
                }

                if (data.y > maxY)
                {
                    maxY = data.y;
                }
            }

            return new Vector2IntMinMax(
                new Vector2Int(minX, minY),
                new Vector2Int(maxX, maxY));

        }

        /// <summary>
        /// Calculates the total bounds from a list of bounds vectors
        /// </summary>
        /// <param name="bounds"> The list of bounds </param>
        /// <returns>The total bounds </returns>
        public static Vector3 GetTotalBounds(List<Vector3> bounds)
        {
            if (bounds.Count == 0)
            {
                return Vector3.zero;
            }

            float minX = bounds[0].x, maxX = bounds[0].x, minY = bounds[0].y, maxY = bounds[0].y, minZ = bounds[0].z, maxZ = bounds[0].z;

            for (int i = 1; i < bounds.Count; i++)
            {
                var data = bounds[i];

                if (data.x < minX)
                {
                    minX = data.x;
                }

                if (data.x > maxX)
                {
                    maxX = data.x;
                }

                if (data.y < minY)
                {
                    minY = data.y;
                }

                if (data.y > maxY)
                {
                    maxY = data.y;
                }

                if (data.z < minZ)
                {
                    minZ = data.z;
                }

                if (data.z > maxZ)
                {
                    maxZ = data.z;
                }
            }

            float boundsX = maxX - minX;
            float boundsY = maxY - minY;
            float boundsZ = maxZ - minZ;

            return new Vector3(boundsX, boundsY, boundsZ);
        }

        public static Vector3 GetBoundsOfTransform(Transform transform)
        {
            List<Vector3> bounds = new List<Vector3>();

            Collider[] colliders = transform.GetComponents<Collider>();

            for (int i = 0; i < colliders.Length; i++)
            {
                bounds.Add(colliders[i].bounds.min);
                bounds.Add(colliders[i].bounds.max);
            }

            Vector3 totalBounds = GetTotalBounds(bounds);

            return totalBounds;
        }

        /// <summary>
        /// Determines if the Grid Settings are valid. This function calculates the number of vertical cells required for desired amount of cells
        /// horizontally with a certain width.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        public static bool GridSettingsValid(GridSettings gridSettings)
        {
            double cellCountYFloat = gridSettings.Height / (gridSettings.Width / gridSettings.AmountOfCellsX);
            double remainder = cellCountYFloat % 1;

            if (!Mathf.Approximately((float)remainder, 0f) && remainder > 0.00001f && remainder + 0.00001f < 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Finds the camera component that is assigned to the Canvas component of the placement grid UI
        /// </summary>
        /// <param name="gridSettings"> The grid settings of the grid </param>
        /// <returns> The Unity Camera component assigned to the Canvas component of the placement grid UI</returns>
        public static Camera GetCameraForGrid(GridSettings gridSettings)
        {
            GameObject cameraGO = GameObject.Find(gridSettings.GridCanvasEventCameraName);

            if (cameraGO != null)
            {
                if (cameraGO.TryGetComponent(out Camera camera))
                {
                    return camera;
                }
            }

            Camera mainCamera = Camera.main;

            if(mainCamera == null)
            {
                Debug.LogErrorFormat("The grid system cannot find a camera. The grid canvas event camera is required. Please either enter the name of the GameObject" +
                    " that contains the camera in the Grid Settings, or ensure there is a camera tagged as MainCamera");
            }

            return mainCamera;
        }

        /// <summary>
        /// Returns the GridInputDefinition for the provided grid settings.
        /// </summary>
        /// <param name="gridSettings">The grid settings to search </param>
        /// <returns>The Grid Input Definition for the current runtime platform</returns>
        public static GridInputDefinition GetInputDefinitionForRuntime(GridSettings gridSettings)
        {
            foreach(var mapping in gridSettings.PlatformGridInputsDefinitionMappings)
            {
                if (mapping == null)
                    continue;

                if(mapping.RuntimePlatform.Equals(Application.platform))
                {
                    return mapping.GridInputDefinition;
                }
            }

            Debug.LogErrorFormat("The grid settings: {0} don't have a definition setup for the current platform: {1}", gridSettings.Key, Application.platform);
            return null;
        }

        /// <summary>
        /// Determines that there are not multiple platforms in the input definition mappings list
        /// </summary>
        /// <param name="gridSettings">The grid settings that contain the input definition mappings</param>
        /// <returns>If the settings are valid</returns>
        public static bool ValidateGridInputDefinitions(GridSettings gridSettings)
        {
            List<RuntimePlatform> platforms = new List<RuntimePlatform>();

            foreach (var mapping in gridSettings.PlatformGridInputsDefinitionMappings)
            {
                if (mapping == null)
                    continue;


                if(platforms.Contains(mapping.RuntimePlatform))
                {
                    Debug.LogErrorFormat("The grid settings: {0} has duplicate input definition mappings for the runtime platform: {1}. Please ensure the runtime platform is only declared once per grid settings.", gridSettings.Key, mapping.RuntimePlatform);
                    return false;
                }

                platforms.Add(mapping.RuntimePlatform);
            }

            return true;
        }

        /// <summary>
        /// Determines the initial placement position based on the placement settings
        /// </summary>
        /// <param name="gridManager">The Grid Manager</param>
        /// <param name="gridSettings">The grid settings to use</param>
        /// <param name="placementSettings">The placement settings to use</param>
        /// <returns> The world position to place the grid object.</returns>
        public static Vector3 GetInitialPlacementPosition(GridManager gridManager, GridSettings gridSettings, PlacementSettings placementSettings)
        {
            Vector3 initialPlacementPosition = GetWorldPositionFromCellIndex(gridManager, gridSettings.InitialPlacementCellIndex, gridSettings, gridManager.RuntimeGridPosition);

            if (placementSettings != null && placementSettings.WorldPosition.HasValue)
            {
                initialPlacementPosition = placementSettings.WorldPosition.Value;
            }

            if (placementSettings != null && placementSettings.GridCellIndex.HasValue)
            {
                initialPlacementPosition = GetWorldPositionFromCellIndex(gridManager, placementSettings.GridCellIndex.Value, gridSettings, gridManager.RuntimeGridPosition);
            }

            return initialPlacementPosition;
        }

        /// <summary>
        /// Calculates which grid cells an object occupies
        /// </summary>
        /// <param name="gridCellOfObject">The grid cell the object is assigned to</param>
        /// <param name="objectSize">The size of the object</param>
        /// <param name="cellSize">The size of the cells</param>
        /// <returns>A list of grid cell indexes</returns>
        public static List<Vector2Int> GetCellIndexesRequiredForObject(Vector2Int gridCellOfObject, Vector3 objectSize, float cellSize)
        {
            List<Vector2Int> indexes = new List<Vector2Int>();

            Vector2 cellAreaRequired = CalculateGridCellAreaRequiredForObject(objectSize, cellSize);

            int cellSpanX = (int)cellAreaRequired.x;
            int radiusX = Mathf.FloorToInt(cellSpanX / 2);

            int cellSpanY = (int)cellAreaRequired.y;
            int radiusY = Mathf.FloorToInt(cellSpanY / 2);

            int minX = -radiusX, maxX = radiusX;

            if (radiusX != 0 && cellSpanX % 2 == 0)
            {
                maxX -= 1;
            }

            int minY = -radiusY, maxY = radiusY;

            if (radiusY != 0 && cellSpanY % 2 == 0)
            {
                maxY -= 1;
            }

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Vector2Int idx = gridCellOfObject + new Vector2Int(x, y);
                    indexes.Add(idx);
                }
            }

            return indexes;
        }

        /// <summary>
        /// Finds the grid objects within a given list of grid cells
        /// </summary>
        /// <param name="placementGrid">The placement grid</param>
        /// <param name="searchArea">The list of grid cells to search</param>
        /// <returns>A list of grid objects that are within the search area</returns>
        public static List<GameObject> GetGridObjectsWithinArea(PlacementGrid placementGrid, List<Vector2Int> searchArea)
        {
            List<GameObject> gridObjects = new List<GameObject>();

            for (int i = 0; i < searchArea.Count; i++)
            {
                GridCell gridCell = placementGrid.GetGridCellFromIndex(searchArea[i]);
                GameObject gridObject = gridCell.GetGridObject();

                if(gridObject == null)
                {
                    continue;
                }

                if (!gridObjects.Contains(gridObject))
                {
                    gridObjects.Add(gridObject);
                }
            }

            return gridObjects;
        }     

        /// <summary>
        /// Calculates the grid cells surrounding a block of grid cells.
        /// </summary>
        /// <param name="gridSettings">The grid settings</param>
        /// <param name="indexes">The cells to search around</param>
        /// <param name="searchRadius">The distance from the search area to include in the results</param>
        /// <returns>A list of grid cell indexes that surround the search area. Inludes the grid cell indexes of the search area</returns>
        public static List<Vector2Int> GetSurroundingIndexesInclusive(GridSettings gridSettings, List<Vector2Int> indexes, int searchRadius = 1)
        {
            List<Vector2Int> surroundingIndexes = new List<Vector2Int>();

            Vector2IntMinMax indexMinMax = GetVector2IntMinMax(indexes);

            for (int x = indexMinMax.Min.x - searchRadius; x <= indexMinMax.Max.x + searchRadius; x++)
            {
                for (int y = indexMinMax.Min.y - searchRadius; y <= indexMinMax.Max.y + searchRadius; y++)
                {
                    Vector2Int index = new Vector2Int(x, y);
                    surroundingIndexes.Add(index);
                }
            }

            return FilterValidGridIndexes(gridSettings, surroundingIndexes);
        }

        /// <summary>
        /// Loops over the given indexes and disregards any indexes that are out of the grid.
        /// </summary>
        /// <param name="gridSettings">The grid settings </param>
        /// <param name="gridIndexes">The indexes to filter</param>
        /// <returns>A list of grid cells that exist on the grid </returns>
        public static List<Vector2Int> FilterValidGridIndexes(GridSettings gridSettings, List<Vector2Int> gridIndexes)
        {
            // Filter those indexes to only include the indexes that are actually on the grid
            for (int i = gridIndexes.Count - 1; i >= 0; i--)
            {
                Vector2Int surroundingIndex = gridIndexes[i];

                if (surroundingIndex.x < 0 ||
                   surroundingIndex.x >= gridSettings.AmountOfCellsX ||
                   surroundingIndex.y < 0 ||
                   surroundingIndex.y >= gridSettings.AmountOfCellsY)
                {
                    gridIndexes.Remove(surroundingIndex);
                }
            }

            return gridIndexes;
        }


        /// <summary>
        /// Calculates the area an object takes up on the grid and where the center of the object is based on it's bounds.
        /// </summary>
        /// <param name="gridSettings">The grid settings </param>
        /// <param name="gridObject">The object to calculate the size for</param>
        /// <param name="objectBounds">The size of the object</param>
        /// <param name="positionOffset">How much the object needs to be offset to be centered.</param>
        public static void CalculateObjectSize(GridSettings gridSettings, GameObject gridObject, out Vector3 objectBounds, out Vector3 positionOffset)
        {
            List<Vector3> tranformBoundsDataAbsolute = new List<Vector3>();
            List<Vector3> tranformBoundsDataRelative = new List<Vector3>();

            List<Transform> childObjects = new List<Transform>();
            List<Transform> processedTransforms = new List<Transform>();

            childObjects = GetAllChildTransforms(ref childObjects, gridObject.transform);

            // Get and pass in root objects colldier
            Collider[] rootColliders = gridObject.GetComponents<Collider>();

            for (int i = 0; i < rootColliders.Length; i++)
            {
                tranformBoundsDataAbsolute.Add(rootColliders[i].bounds.min);
                tranformBoundsDataAbsolute.Add(rootColliders[i].bounds.max);
            }

            if (gridSettings.CalculateChildColliders)
            {
                while (childObjects.Count > 0)
                {
                    Transform endTransform = FindUnprocessedEndTransform(childObjects, processedTransforms);

                    if (endTransform == null)
                    {
                        break;
                    }

                    if (endTransform && endTransform.TryGetComponent<Collider>(out var collider))
                    {
                        Collider[] colliders = endTransform.GetComponents<Collider>();

                        for (int i = 0; i < colliders.Length; i++)
                        {
                            tranformBoundsDataAbsolute.Add(colliders[i].bounds.min);
                            tranformBoundsDataAbsolute.Add(colliders[i].bounds.max);

                            Vector3 childBoundsRelativeMin = CalculateBoundsForChildTransformRelativeMin(colliders[i], gridObject.transform.position);
                            tranformBoundsDataRelative.Add(childBoundsRelativeMin);

                            Vector3 childBoundsRelativeMax = CalculateBoundsForChildTransformRelativeMax(colliders[i], gridObject.transform.position);
                            tranformBoundsDataRelative.Add(childBoundsRelativeMax);
                        }
                    }

                    childObjects.Remove(endTransform);
                    processedTransforms.Add(endTransform);
                }
            }

            objectBounds = GetObjectBounds(gridSettings, tranformBoundsDataAbsolute);
            positionOffset = GetObjectPositionOffset(tranformBoundsDataRelative, gridObject);
        }

        private static List<Transform> GetAllChildTransforms(ref List<Transform> childTransforms, Transform transform)
        {
            foreach (Transform t in transform)
            {
                childTransforms.Add(t);

                if (t.childCount > 0)
                {
                    GetAllChildTransforms(ref childTransforms, t);
                }
            }

            return childTransforms;
        }

        /// <summary>
        /// This function calculates the position of the object relative to the root object.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="rootObjectPosition"></param>
        /// <returns></returns>
        private static Vector3 CalculateBoundsForChildTransformRelativeMin(Collider collider, Vector3 gridObjectPosition)
        {
            var res = collider.bounds.center - collider.bounds.extents;
            res -= gridObjectPosition;
            return res;
        }

        private static Vector3 CalculateBoundsForChildTransformRelativeMax(Collider collider, Vector3 gridObjectPosition)
        {
            var res = collider.bounds.center + collider.bounds.extents;
            res -= gridObjectPosition;

            return res;
        }

        private static Transform FindUnprocessedEndTransform(List<Transform> childObjects, List<Transform> processedTransforms)
        {
            foreach (var child in childObjects)
            {
                if (AllChildTransformsProcessed(child, processedTransforms))
                {
                    return child;
                }
            }

            return null;
        }

        private static bool AllChildTransformsProcessed(Transform transform, List<Transform> processedTransforms)
        {
            bool allProcessed = true;

            foreach (Transform t in transform)
            {
                if (!processedTransforms.Contains(t))
                    allProcessed = false;
            }

            return allProcessed;
        }

        private static Vector3 GetObjectBounds(GridSettings gridSettings, List<Vector3> boundsData)
        {
            if (boundsData.Count == 0)
            {
                // If no colliders are on the object the bounds will be defaulted to a 1x1 grid cell.
                Debug.Log("The grid object doesn't have any colliders. A default size of 1x1 will be applied");
                return new Vector3(gridSettings.CellSize, gridSettings.CellSize, gridSettings.CellSize);
            }

            Vector3 totalBounds = GetTotalBounds(boundsData);

            return totalBounds;
        }

        private static Vector3 GetObjectPositionOffset(List<Vector3> boundsDataRelative, GameObject gridObject)
        {
            // Calculate the root objects collider Size and offset.
            Collider[] rootObjectColliders = gridObject.transform.GetComponents<Collider>();

            // These are the collider centers
            List<Vector3> rootColliderOffsets = new List<Vector3>();

            // Get the root colliders bounds offset
            for (int i = 0; i < rootObjectColliders.Length; i++)
            {
                rootColliderOffsets.Add(rootObjectColliders[i].bounds.center + rootObjectColliders[i].bounds.extents - gridObject.transform.position);
                rootColliderOffsets.Add(rootObjectColliders[i].bounds.center - rootObjectColliders[i].bounds.extents - gridObject.transform.position);
            }

            // Add root colliders to all object bounds
            for (int i = 0; i < rootColliderOffsets.Count; i++)
            {
                boundsDataRelative.Add(rootColliderOffsets[i]);
            }

            Vector3MinMax offsetsMinMax = GetVector3MinMax(boundsDataRelative);

            float childMidPointX = (offsetsMinMax.Min.x + offsetsMinMax.Max.x) / 2f;
            float childMidPointZ = (offsetsMinMax.Min.z + offsetsMinMax.Max.z) / 2f;


            float midPointOffsetX = childMidPointX;
            float midPointOffsetZ = childMidPointZ;

            Vector3 offset = new Vector3(-midPointOffsetX, 0, -midPointOffsetZ);
            return offset;
        }

        /// <summary>
        /// Checks if a given cell index is locatied within the grid.
        /// </summary>
        /// <param name="gridSettings">The grid settings </param>
        /// <param name="gridIndex">The index to validate </param>
        /// <returns>If the cell index is valid </returns>
        public static bool IsIndexWithinGrid(GridSettings gridSettings, Vector2Int gridIndex)
        {
            if (gridIndex.x < 0)
                return false;

            if (gridIndex.y < 0)
                return false;

            if (gridIndex.x >= gridSettings.AmountOfCellsX)
                return false;

            if (gridIndex.y >= gridSettings.AmountOfCellsY)
                return false;

            return true;
        }
    }
}