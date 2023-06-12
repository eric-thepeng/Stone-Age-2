using Hypertonic.GridPlacement.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement
{
    public class GridCellPlacementIndicator : MonoBehaviour
    {
        private Transform _gridTransform;
        private GridSettings _gridSettings;
        private Vector3 _gridObjectSize;
        private ObjectAlignment _gridObjectAlignment;
        private Vector3 _gridObjectRotation;

        private GameObject _highlightGridCell;
        private RectTransform _highlightGridCellTransform;
        private Image _highlightGridCellImage;

        private Vector3 _objectRotationOnSetup;

        private GridManager _gridManager;

        private float _initialGridRotation;

        public void Setup(GridManager gridManager, Transform gridTransform, GridSettings gridSettings, Vector3 objectSize, ObjectAlignment objectAlignment, Vector3 gridObjectRotation)
        {
            _gridManager = gridManager;
            _objectRotationOnSetup = gridObjectRotation;
            _gridTransform = gridTransform;
            _gridSettings = gridSettings;
            _gridObjectSize = objectSize;
            _gridObjectAlignment = objectAlignment;
            _gridObjectRotation = gridObjectRotation;

            _initialGridRotation = _gridManager.RuntimeGridRotation;


            CreateIndicator(gridTransform, gridSettings, objectSize, objectAlignment, gridObjectRotation);
            UpdateHighlightCellValidStatus(true);
        }

        public void HandleGridRotationUpdated(Vector3 gridObjectRotation)
        {
            _gridObjectRotation = gridObjectRotation;
        }

        private float GetGridDisplayImageSize()
        {
            return 10;
        }

        private void CreateIndicator(Transform gridTransform, GridSettings gridSettings, Vector3 objectSize, ObjectAlignment objectAlignment, Vector3 objectRotation)
        {
            float gridCellImageSize = GetGridDisplayImageSize();

            _highlightGridCell = new GameObject("Cell Indicator");
            _highlightGridCell.transform.SetParent(gridTransform, false);

            _highlightGridCellTransform = _highlightGridCell.AddComponent<RectTransform>();

            #region Set Size

            Vector3 gridObjectSizeRotated = CalculateRotatedSize(objectSize, objectRotation);

            Vector2Int cellSizeRequired = GridUtilities.CalculateGridCellAreaRequiredForObject(gridObjectSizeRotated, _gridSettings.CellSize);

            double height = (_gridSettings.Height / _gridSettings.AmountOfCellsY) * (cellSizeRequired.y * gridCellImageSize);
            double width = (_gridSettings.Width / _gridSettings.AmountOfCellsX) * (cellSizeRequired.x * gridCellImageSize);

          
            #endregion Set Size

            _highlightGridCellTransform.sizeDelta = new Vector2((float)width, (float)height);

            _highlightGridCellImage = _highlightGridCell.AddComponent<Image>();
            _highlightGridCellImage.sprite = gridSettings.OccupiedCellImage;
            _highlightGridCellImage.raycastTarget = false;
            _highlightGridCellImage.type = Image.Type.Tiled;

            _highlightGridCellImage.pixelsPerUnitMultiplier = (float)(_gridSettings.AmountOfCellsX / _gridSettings.Width) * 10;

            // Initialise the colour to have 0 alpha if Colour
            _highlightGridCellImage.color = new Color(0, 0, 0, 0);
            _highlightGridCellImage.sprite = gridSettings.OccupiedCellImage;
        }

        public void UpdateHighlightGridCell(Vector2Int gridCellOfObject, PlacementValidResponse placementValidResponse)
        {
            if (_gridSettings == null) return;

            if(_gridSettings.HidePlacementCellsIfOutsideOfGrid)
            {
                UpdateVisibility(placementValidResponse);
            }
          
            UpdateHighlightCellValidStatus(placementValidResponse.Valid);
            MoveCellToWorldPosition(gridCellOfObject, _gridObjectRotation);
        }

        public void UpdateHighlightCellValidStatus(bool isAvailable)
        {
            SetHighlightCellColour(isAvailable);
        }

        private void MoveCellToWorldPosition(Vector2Int gridCellOfObject, Vector3 gridObjectRotation)
        {
            float gridOffsetX = (float)_gridSettings.AmountOfCellsX / 2;
            float gridOffsetY = (float)_gridSettings.AmountOfCellsY / 2;

            float cellSize = GetGridDisplayImageSize() * _gridSettings.CellSize;

            float xPos = (gridCellOfObject.x - gridOffsetX) * cellSize;
            float yPos = (gridCellOfObject.y - gridOffsetY) * cellSize;

            Vector3 gridObjectSize = _gridObjectSize;

            Vector3 gridObjectSizeRotated = CalculateRotatedSize(gridObjectSize, gridObjectRotation);
         
            Vector2Int cellSizeRequired = GridUtilities.CalculateGridCellAreaRequiredForObject(gridObjectSizeRotated, _gridSettings.CellSize);

            if (cellSizeRequired.x % 2 != 0)
            {
                xPos += cellSize / 2;
            }

            if (cellSizeRequired.y % 2 != 0)
            {
                yPos += cellSize / 2;
            }

            _highlightGridCellTransform.localPosition = new Vector3(xPos, yPos, 0);
        }

        public void UpdateObjectAlignment(ObjectAlignment objectAlignment)
        {
            _gridObjectAlignment = objectAlignment;
            RecreateIndicator();
        }

        public void HandleObjectRotated(Vector3 rotation, Vector2Int gridCellOfObject, PlacementValidResponse placementValidResponse)
        {
            _gridObjectRotation = rotation;
            RecreateIndicator();
            UpdateHighlightCellValidStatus(placementValidResponse.Valid);
            MoveCellToWorldPosition(gridCellOfObject, rotation);

            if (_gridSettings.HidePlacementCellsIfOutsideOfGrid)
            {
                UpdateVisibility(placementValidResponse);
            }
        }

        private void RecreateIndicator()
        {
            Destroy(_highlightGridCell);
            CreateIndicator(_gridTransform, _gridSettings, _gridObjectSize, _gridObjectAlignment, _gridObjectRotation);
        }

        private void SetHighlightCellColour(bool isAvailable)
        {
            _highlightGridCellImage.color = isAvailable
               ? _gridSettings.CellColourAvailable
               : _gridSettings.CellColourNotAvailable;
        }

        private void UpdateVisibility(PlacementValidResponse placementValidResponse)
        {
            if(placementValidResponse.PlacementInvalidReason.HasValue && placementValidResponse.PlacementInvalidReason.Value.Equals(PlacementInvalidReason.OUTSIDE_GRID))
            {
                UpdateVisibility(false);
            }
            else
            {
                UpdateVisibility(true);
            }
        }

        private void UpdateVisibility(bool show)
        {
            if(show)
            {
                _highlightGridCellImage.gameObject.SetActive(true);
            }
            else
            {
                _highlightGridCellImage.gameObject.SetActive(false);
            }
        }

        public void Clear()
        {
            Destroy(_highlightGridCell);
            _gridSettings = null;
            _gridObjectSize = new Vector3();
            _highlightGridCell = null;
            _highlightGridCellImage = null;
            _highlightGridCellTransform = null;
        }

        public GameObject GetCloneOfHighlightCell(Transform parent)
        {
            GameObject highlightGridCell = Instantiate(_highlightGridCell, parent);
            return highlightGridCell;
        }

        public void MoveToWorldPosition(RectTransform rectTransform, Vector2Int gridCellOfObject, Vector3 gridObjectRotation)
        {
            float gridOffsetX = (float)_gridSettings.AmountOfCellsX / 2;
            float gridOffsetY = (float)_gridSettings.AmountOfCellsY / 2;

            float cellSize = GetGridDisplayImageSize() * _gridSettings.CellSize;

            float xPos = (gridCellOfObject.x - gridOffsetX) * cellSize;
            float yPos = (gridCellOfObject.y - gridOffsetY) * cellSize;

            Vector3 gridObjectSize = _gridObjectSize;
            Vector3 gridObjectSizeRotated = CalculateRotatedSize(gridObjectSize, gridObjectRotation);

            Vector2Int cellSizeRequired = GridUtilities.CalculateGridCellAreaRequiredForObject(gridObjectSizeRotated, _gridSettings.CellSize);

            if (cellSizeRequired.x % 2 != 0)
            {
                xPos += cellSize / 2;
            }

            if (cellSizeRequired.y % 2 != 0)
            {
                yPos += cellSize / 2;
            }

            rectTransform.localPosition = new Vector3(xPos, yPos, 0);
        }

        private Vector3 CalculateRotatedSize(Vector3 objectSize, Vector3 objectRotation)
        {
            int rotationAbs = GetRotationAbs(objectRotation, _objectRotationOnSetup, _initialGridRotation, _gridManager.RuntimeGridRotation);

            if (rotationAbs != 0 && rotationAbs % 180 != 0 && rotationAbs % 360 != 0)
            {
                float tmp = objectSize.x;
                objectSize.x = objectSize.z;
                objectSize.z = tmp;
            }

            return objectSize;
        }

        private int GetRotationAbs(Vector3 objectRotation, Vector3 objectsInitialRotation, float initialGridRotation, float gridRotation)
        {
            float rot = GetObjectRelativeRotation(objectRotation, objectsInitialRotation, initialGridRotation, gridRotation);
            int rotation = Mathf.RoundToInt(rot * 10000) / 10000;
            int rotationAbs = Mathf.Abs(rotation);
            return rotationAbs;
        }

        private float GetObjectRelativeRotation(Vector3 objectRotation, Vector3 objectsInitialRotation, float initialGridRotation, float gridRotation)
        {
            float objectRotationYModulus = objectRotation.y % 360;
            float objectRotY = Mathf.RoundToInt(objectRotationYModulus * 10000) / 10000;

            float objectInitRotYModulus = objectsInitialRotation.y % 360;
            float objectInitialRotationY = Mathf.RoundToInt(objectInitRotYModulus * 10000) / 10000;

            float initialGridRotModulus = initialGridRotation % 360;
            float initialGridRotationRounded = Mathf.RoundToInt(initialGridRotModulus * 10000) / 10000;

            float gridRotationModulus = gridRotation % 360;
            float gridRotationRounded = Mathf.RoundToInt(gridRotationModulus * 10000) / 10000;

            float relativeRotation = (objectRotY - objectInitialRotationY) - gridRotationRounded + initialGridRotationRounded;

            return relativeRotation;
        }
    }
}