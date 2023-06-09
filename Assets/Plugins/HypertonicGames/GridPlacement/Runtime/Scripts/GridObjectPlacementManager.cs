using Hypertonic.GridPlacement.CustomSizing;
using Hypertonic.GridPlacement.Enums;
using Hypertonic.GridPlacement.GridObjectComponents;
using Hypertonic.GridPlacement.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hypertonic.GridPlacement
{
    public class GridObjectPlacementManager : MonoBehaviour
    {
        public Action<Vector2Int> OnGridPositionChanged;

        public GameObject ObjectToPlace { get; private set; }

        public bool IsPlacingObject => ObjectToPlace != null;

        public Vector3 ObjectBounds
        {
            get
            {
                Vector3 objectBounds = new Vector3(_objectBounds.x, _objectBounds.y, _objectBounds.z);

                int rotation = _gridObjectRotationRelative;

                if (rotation != 0 && Mathf.Abs(rotation) % 180 != 0)
                {
                    var tmp = objectBounds.x;
                    objectBounds.x = objectBounds.z;
                    objectBounds.z = tmp;
                }

                return objectBounds;
            }
            private set
            {
                _objectBounds = value;
            }
        }

        private Vector3 _objectBounds;

        public Vector3 PositionOffset
        {
            get
            {
                int rotation = _gridObjectRotationRelative;

                if (rotation == 0 || rotation == 360)
                {
                    var offset = new Vector3(_positionOffset.x, _positionOffset.y, _positionOffset.z);
                    return offset;
                }

                if (rotation == 270 || rotation == -90)
                {
                    var offset = new Vector3(-_positionOffset.z, _positionOffset.y, _positionOffset.x);
                    return offset;

                }

                if (rotation == 180 || rotation == -180)
                {
                    var offset = new Vector3(-_positionOffset.x, _positionOffset.y, -_positionOffset.z);
                    return offset;
                }

                if (rotation == 90 || rotation == -270)
                {

                    var offset = new Vector3(_positionOffset.z, _positionOffset.y, -_positionOffset.x);
                    return offset;
                }

                return Vector3.zero;

            }
            private set
            {
                _positionOffset = value;
            }
        }

        private Vector3 _positionOffset = new Vector3();

        /// <summary>
        /// Used to override the snapped position of the object.
        /// </summary>
        public Vector3 CustomPositionOffset { get; set; } = Vector3.zero;

        public Vector3 SnapAlignmentOffset { get; private set; } = new Vector3();

        public ObjectAlignment ObjectAlignment { get; private set; } = ObjectAlignment.CENTER;

        [HideInInspector]
        public Vector3 SelectedWorldPosition;

        public Vector3 WorldPosOfAssignedGridCell { get; private set; }
        public Vector3 CenterOfGridArea { get; private set; }

        public Vector2Int GridCellOfSelectedPosition {
            get
            {
                return _gridCellOfSelectedPosition;
            }
            private set
            {
                _gridCellOfSelectedPosition = value;
                OnGridPositionChanged?.Invoke(_gridCellOfSelectedPosition);
            }
        }

        private Vector2Int _gridCellOfSelectedPosition = Vector2Int.zero;

        private GridSettings _gridSettings;

        private LayerMask _originalObjectLayer;

        private int _gridObjectRotation => Mathf.RoundToInt(ObjectToPlace.transform.localRotation.eulerAngles.y * 10000) / 10000;

        private int _gridObjectRotationRelative { 
            get 
            {
                int gridObjectRotation = _gridObjectRotation % 360;
                int gridObjectsInitRotation = _gridObjectsInitRotation % 360;

                int res = (gridObjectRotation - gridObjectsInitRotation) % 360;
                return res;
            } 
        }

        private int _gridObjectsInitRotation = 0;


        public GameObject ObjectPlacementContainer
        {
            get
            {                
                if (_objectPlacementContainer == null)
                {
                    _objectPlacementContainer = new GameObject("Object Placement Container");

                    if(_gridManager != null)
                    {
                        _objectPlacementContainer.transform.position = _gridManager.GridTransformGO.transform.position;
                    }
                }

                return _objectPlacementContainer;
            }
            private set
            {
                _objectPlacementContainer = value;
            }
        }
        private GameObject _objectPlacementContainer;


        private GridManager _gridManager;

        public void Setup(GridManager gridManager, GridSettings gridSettings, GameObject objectToPlace)
        {
            _gridManager = gridManager;
            _gridSettings = gridSettings;
            ObjectToPlace = objectToPlace;

            WorldPosOfAssignedGridCell = new Vector3();

            Quaternion objectToPlaceCurrentRotation = objectToPlace.transform.localRotation;
            Vector3 objectToPlaceCurrentPosition = objectToPlace.transform.position;

            _gridObjectsInitRotation = 0;

         

            ObjectPlacementContainer.transform.rotation = Quaternion.identity;


            SetCustomHeightOverride(objectToPlace);


            ObjectToPlace.transform.rotation = Quaternion.Euler(Vector3.zero);
            
            SetObjectProperties();

            ObjectToPlace.transform.position = Vector3.zero;
            ObjectToPlace.transform.SetParent(ObjectPlacementContainer.transform, false);
            ObjectPlacementContainer.transform.rotation = Quaternion.Euler(new Vector3(0, _gridManager.RuntimeGridRotation, 0));

            UpdateSnapAlignment(_gridSettings.DefaultAlignment);
            _originalObjectLayer = objectToPlace.layer;

            ObjectPlacementContainer.transform.position = objectToPlaceCurrentPosition;
            ObjectToPlace.transform.localPosition = Vector3.zero;
            ObjectToPlace.transform.localRotation = objectToPlaceCurrentRotation;
        }

        private void SetCustomHeightOverride(GameObject objectToPlace)
        {
            CustomPositionOffset customPositionOffset = objectToPlace.GetComponent<CustomPositionOffset>();

            if(customPositionOffset == null)
            {
                CustomPositionOffset = Vector3.zero;
                return;
            }

            CustomPositionOffset = customPositionOffset.PositionOverride;
            customPositionOffset.OnPositionChanged += HandleCustomPositionChanged;
        }

        /// <summary>
        /// A handler for the event being raised in the CustomPositionOverride component.
        /// </summary>
        /// <param name="position">the vector 3 to add to the snapped position of the gameobject</param>
        private void HandleCustomPositionChanged(Vector3 position)
        {
            ObjectPlacementContainer.transform.position -= CustomPositionOffset;
            CustomPositionOffset = position;
            ObjectPlacementContainer.transform.position += CustomPositionOffset;
        }


        private void SetObjectProperties()
        {
            // Check to see if the object has already has it's properties set.
            GridObjectInfo gridObjectInfo = ObjectToPlace.GetComponent<GridObjectInfo>();

            if(gridObjectInfo != null && gridObjectInfo.ObjectBounds.HasValue && gridObjectInfo.PositionOffset.HasValue)
            {
                ObjectBounds = gridObjectInfo.ObjectBounds.Value;
                PositionOffset = gridObjectInfo.PositionOffset.Value;

                TryApplyGridHeightPosition();

                return;
            }

            CustomBoundsController customBoundsController = ObjectToPlace.GetComponent<CustomBoundsController>();

            if (customBoundsController != null)
            {
                SetObjectCustomBoundsProperties(customBoundsController);
            }
            else
            {
                CalculateObjectProperties();
            }

            TryApplyGridHeightPosition();

            if (gridObjectInfo == null)
            {
                gridObjectInfo = ObjectToPlace.AddComponent<GridObjectInfo>();
            }
          
            gridObjectInfo.ObjectBounds = _objectBounds;
            gridObjectInfo.PositionOffset = _positionOffset;
        }

        private bool TryApplyGridHeightPosition()
        {
            GridHeightPositioner gridHeightPositioner = ObjectToPlace.GetComponent<GridHeightPositioner>();

            if (gridHeightPositioner != null)
            {
                ApplyGridHeightPosition(gridHeightPositioner);
                return true;
            }

            return false;
        }

        private void CalculateObjectProperties()
        {
             GridUtilities.CalculateObjectSize(_gridSettings, ObjectToPlace, out Vector3 objectBounds, out Vector3 positionOffset);

            ObjectBounds = objectBounds;
            PositionOffset = positionOffset;
        }

        public void HandleGridPositionUpdated(Vector3 positionDifference)
        {
            ObjectPlacementContainer.transform.position += positionDifference;
        }

        public void HandleGridRotationUpdated(float rotationDifference)
        {
            float currentRot = Mathf.RoundToInt(ObjectPlacementContainer.transform.rotation.eulerAngles.y * 10000) / 10000;
            float newRot = Mathf.RoundToInt((currentRot + rotationDifference) * 10000) / 10000;
            float diff = newRot - currentRot;

            ObjectPlacementContainer.transform.RotateAround(_gridManager.GridTransformGO.transform.position, Vector3.up, diff);
        }

        private void SetObjectCustomBoundsProperties(CustomBoundsController customBoundsController)
        {
            Vector3 size = customBoundsController.Bounds.size;
            Vector3 center = customBoundsController.Bounds.center;

            center = new Vector3(-center.x, 0, -center.z);

            ObjectBounds = size;
            PositionOffset = center;
        }

        private void ApplyGridHeightPosition(GridHeightPositioner gridHeightPositioner)
        {
            float gridHeightPosition = gridHeightPositioner.GridHeight;

            PositionOffset = new Vector3(_positionOffset.x, -gridHeightPosition, _positionOffset.z);
        }

        public void UpdateSnapAlignment(ObjectAlignment updatedAlignment)
        {
            ObjectPlacementContainer.transform.position -= Quaternion.Euler(0, _gridManager.RuntimeGridRotation, 0) * SnapAlignmentOffset;

            SnapAlignmentOffset = GetSnapAlignmentOffset(updatedAlignment, ObjectBounds, _gridSettings);
            ObjectAlignment = updatedAlignment;

            ObjectPlacementContainer.transform.position += Quaternion.Euler(0, _gridManager.RuntimeGridRotation, 0) * SnapAlignmentOffset;
        }

        public Vector3 GetSnapAlignmentOffset(ObjectAlignment objectAlignment, Vector3 objectBounds, GridSettings gridSettings)
        {
            Vector2 cellAreaRequiredForObject = GridUtilities.CalculateGridCellAreaRequiredForObject(objectBounds, gridSettings.CellSize);
            Vector2 cellSpanOfObject = GridUtilities.CalculateCellSpanOfGridObject(objectBounds, gridSettings.CellSize);

            float paddingX = cellAreaRequiredForObject.x - cellSpanOfObject.x;
            float paddingZ = cellAreaRequiredForObject.y - cellSpanOfObject.y;

            float offsetX = (paddingX * gridSettings.CellSize) / 2;
            float offsetZ = (paddingZ * gridSettings.CellSize) / 2;

            Vector3 snapAlignmentOffset = new Vector3();

            switch (objectAlignment)
            {
                case ObjectAlignment.CENTER:
                    snapAlignmentOffset = Vector3.zero;
                    break;
                case ObjectAlignment.MIDDLE_LEFT:
                    snapAlignmentOffset = new Vector3(-offsetX, 0, 0);
                    break;
                case ObjectAlignment.UPPER_LEFT:
                    snapAlignmentOffset = new Vector3(-offsetX, 0, offsetZ);
                    break;
                case ObjectAlignment.UPPER_MIDDLE:
                    snapAlignmentOffset = new Vector3(0, 0, offsetZ);
                    break;
                case ObjectAlignment.UPPER_RIGHT:
                    snapAlignmentOffset = new Vector3(offsetX, 0, offsetZ);
                    break;
                case ObjectAlignment.MIDDLE_RIGHT:
                    snapAlignmentOffset = new Vector3(offsetX, 0, 0);
                    break;
                case ObjectAlignment.BOTTOM_RIGHT:
                    snapAlignmentOffset = new Vector3(offsetX, 0, -offsetZ);
                    break;
                case ObjectAlignment.BOTTOM_MIDDLE:
                    snapAlignmentOffset = new Vector3(0, 0, -offsetZ);
                    break;
                case ObjectAlignment.BOTTOM_LEFT:
                    snapAlignmentOffset = new Vector3(-offsetX, 0, -offsetZ);
                    break;
            }

            return snapAlignmentOffset;
        }

        public void UpdateObjectPosition(Vector3 worldPos)
        {
            if (ObjectToPlace == null)
            {
                Debug.LogWarning("Warning: Cannot update object position as _objectToPlace is null");
                return;
            }

            GridCellOfSelectedPosition = PlacementGrid.GetCellIndexFromWorldPosition(worldPos, _gridSettings, _gridManager.RuntimeGridRotation, _gridManager.RuntimeGridPosition);
            WorldPosOfAssignedGridCell = PlacementGrid.GetCellPositionFromGridCellIndex(GridCellOfSelectedPosition, _gridSettings, _gridManager.RuntimeGridPosition, _gridManager.RuntimeGridRotation);
          
            CenterOfGridArea = GridUtilities.GetCenterOfGridArea(WorldPosOfAssignedGridCell, ObjectBounds, _gridSettings);

            ObjectPlacementContainer.transform.position = Quaternion.Euler(0, _gridManager.RuntimeGridRotation, 0) * (CenterOfGridArea + PositionOffset + SnapAlignmentOffset + CustomPositionOffset);
        }

        /// <summary>
        /// Returns the size of the object.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetObjectRelativeSize()
        {
            return ObjectBounds;
        }

        public void HandleGridObjectRotated()
        {
            WorldPosOfAssignedGridCell = PlacementGrid.GetCellPositionFromGridCellIndex(GridCellOfSelectedPosition, _gridSettings, _gridManager.RuntimeGridPosition, _gridManager.RuntimeGridRotation);
            CenterOfGridArea = GridUtilities.GetCenterOfGridArea(WorldPosOfAssignedGridCell, ObjectBounds, _gridSettings);
            
            ObjectPlacementContainer.transform.position = Quaternion.Euler(0, _gridManager.RuntimeGridRotation, 0) * (CenterOfGridArea + PositionOffset + SnapAlignmentOffset + CustomPositionOffset);

            UpdateSnapAlignment(ObjectAlignment);
        }

        public Vector3 GetCurrentPositionOfObject() => ObjectPlacementContainer.transform.position - PositionOffset;

        public float GetObjectRotation() => ObjectToPlace.transform.eulerAngles.y;


        /// <summary>
        /// Places the object by setting up the grid object info component
        /// </summary>
        /// <param name="gridCellIndex">The should be the raw grid cell index of the position without the offset of the alignment applied. </param>
        public void PlaceObject(Vector2Int gridCellIndex)
        {
            if (!ObjectToPlace.TryGetComponent(out GridObjectInfo gridObjectInfo))
                gridObjectInfo = ObjectToPlace.AddComponent<GridObjectInfo>();

            gridObjectInfo.Setup(_gridSettings.Key, gridCellIndex, ObjectAlignment);

            ObjectToPlace.layer = _originalObjectLayer;

            UnsubscribeFromCustomHeightOverrideEvent();

            ObjectToPlace = null;
            
            ClearPlacingObject();
        }

        public void ResetObjectPlacementContainer()
        {
            ObjectPlacementContainer.transform.position = _gridManager.GridTransformGO.transform.position;
        }

        public void ClearPlacingObject()
        {
            if (ObjectToPlace != null)
            {
                UnsubscribeFromCustomHeightOverrideEvent();
                Destroy(ObjectToPlace);
            }

            ObjectToPlace = null;
            ObjectBounds = Vector3.zero;
            PositionOffset = Vector3.zero;
            SnapAlignmentOffset = Vector3.zero;
            PositionOffset = Vector3.zero;
            WorldPosOfAssignedGridCell = Vector3.zero;
        }

        private void UnsubscribeFromCustomHeightOverrideEvent()
        {
            CustomPositionOffset customHeightOverride = ObjectToPlace.GetComponent<CustomPositionOffset>();

            if (customHeightOverride != null)
            {
                customHeightOverride.OnPositionChanged -= HandleCustomPositionChanged;
            }
        }
    }
}