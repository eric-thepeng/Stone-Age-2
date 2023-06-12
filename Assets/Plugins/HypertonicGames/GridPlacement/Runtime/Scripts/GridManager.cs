using Hypertonic.GridPlacement.Enums;
using Hypertonic.GridPlacement.GridInput;
using Hypertonic.GridPlacement.GridObjectComponents;
using Hypertonic.GridPlacement.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Hypertonic.GridPlacement
{
    [RequireComponent(typeof(GridDisplayManager))]
    public class GridManager : MonoBehaviour
    {
        #region Events
        public delegate void GridEvent();
        public delegate void GridObjectEvent(GameObject gridObject);
        public delegate void GridObjectValidPlacementEvent(bool isValid);
        public delegate void GridObjectPlacementEvent(Vector2Int gridPosition);
        public delegate void GridManagerEvent(GridManager gridManager);

        public static event GridManagerEvent OnGridManagerSetup;
        public static event GridManagerEvent OnGridManagerDestroyed;
        public event GridEvent OnPlacementModeStarted;
        public event GridEvent OnGridShown;
        public event GridEvent OnGridHidden;
        public event GridEvent OnGridObjectDeleted;
        public event GridObjectEvent OnGridObjectPlaced;
        public event GridObjectValidPlacementEvent OnPlacementValidated;
        public event GridEvent OnPlacementGridPopulated;

        /// <summary>
        /// Fires an event containing the grid cell position of the grid object currently being placed.
        /// </summary>
        public event GridObjectPlacementEvent OnObjectPositionUpdated;

        /// <summary>
        /// An event that fires when an already placed grid object enters placement mode to modify its placement.
        /// </summary>
        public event GridObjectEvent OnStartModifyObjectPlacement;

        #endregion Event

        public GridData GridData
        {
            get
            {
                List<GameObject> gridObjects = _grid.GridObjects;
                List<GridObjectPositionData> gridObjectPositionDatas = new List<GridObjectPositionData>();

                for (int i = 0; i < gridObjects.Count; i++)
                {
                    GameObject gridObject = gridObjects[i];
                    GridObjectInfo gridObjectInfo = gridObject.GetComponent<GridObjectInfo>();

                    gridObjectPositionDatas.Add(new GridObjectPositionData(gridObject, gridObjectInfo.GridCellIndex, gridObjectInfo.ObjectAlignment));
                }

                return new GridData(_gridSettings.Key, gridObjectPositionDatas);
            }
        }

        /// <summary>
        /// Returns if the grid is in placement mode
        /// </summary>
        public bool IsPlacingGridObject { get => _gridObjectPlacementManager.IsPlacingObject; }

        /// <summary>
        /// Returns the object currently in placement mode.
        /// </summary>
        public GameObject ObjectToPlace { get => _gridObjectPlacementManager.ObjectToPlace; }

        public GridSettings GridSettings => _gridSettings;
        [SerializeField] private GridSettings _gridSettings;

        public float RuntimeGridRotation { get; private set; } = 0f;
        public Vector3 RuntimeGridPosition { get; private set; } = Vector3.zero;
        public GameObject GridTransformGO { get; private set; }

        public GridInputDefinition GridInputDefinition
        {
            get
            {
                if (!GridUtilities.ValidateGridInputDefinitions(GridSettings))
                {
                    return null;
                }

                return GridUtilities.GetInputDefinitionForRuntime(_gridSettings);
            }
        }

        public bool IsSetup { get; private set; } = false;


        private GridDisplayManager _gridDisplayManager;

        private PlacementGrid _grid;

        private GridObjectPlacementManager _gridObjectPlacementManager;
        private GridObjectVisualManager _gridObjectVisualManager;
        private GridCellPlacementIndicator _gridCellPlacementIndicator;
        private UnavailableGridCellDisplayManager _unavailableGridCellDisplayManager;

        private GameObject _gridInputDetectorGO;

        private bool _paintMode = false;
        private GridInputDetector _gridInputDetector;
        private GridInputDefinition _gridInputDefinition
        {
            get => _gridInputDetector.GridInputDefinition;
        }

        /// <summary>
        /// This is used to store a reference to the prefab that should be placed continuously.
        /// </summary>
        private GameObject _paintObjectPrefab;

        #region Unity Lifecycle events

        private void Awake()
        {
            if (!TryGetComponent(out _gridDisplayManager))
                _gridDisplayManager = gameObject.AddComponent<GridDisplayManager>();

            if (!TryGetComponent(out _gridObjectPlacementManager))
                _gridObjectPlacementManager = gameObject.AddComponent<GridObjectPlacementManager>();

            if (!TryGetComponent(out _gridObjectVisualManager))
                _gridObjectVisualManager = gameObject.AddComponent<GridObjectVisualManager>();

            if (!TryGetComponent(out _gridCellPlacementIndicator))
                _gridCellPlacementIndicator = gameObject.AddComponent<GridCellPlacementIndicator>();

            if (!TryGetComponent(out _unavailableGridCellDisplayManager))
                _unavailableGridCellDisplayManager = gameObject.AddComponent<UnavailableGridCellDisplayManager>();

            // Determine if this Grid Manager was made programatically or in the Editor
            if (_gridSettings != null)
            {
                Setup(_gridSettings);
            }
        }

        private void OnDestroy()
        {
            GridManagerAccessor.UnregisterGridManager(_gridSettings.Key, this);
            OnGridManagerDestroyed?.Invoke(this);
        }

        #endregion Unity Lifecycle events

        /// <summary>
        /// Setup method required to initialise the grid system. This overload allows the system to
        /// use a different GridSettings object.
        /// </summary>
        /// <param name="gridSettings"> Used to configure the grid system </param>
        public void Setup(GridSettings gridSettings)
        {
            _gridSettings = gridSettings;
            Setup();
        }

        /// <summary>
        /// Setup method required to initialise the grid system
        /// </summary>
        public void Setup()
        {
            if (_gridSettings == null)
            {
                Debug.LogError("Unable to setup the grid Manager. You must either set the grid settings in the inspector or pass them in to the Setup function");
                return;
            }

            if (!GridUtilities.GridSettingsValid(_gridSettings))
            {
                Debug.LogErrorFormat("The grid settings are invalid for grid settings {0}. Ensure the width to height ratio doesn't cause the vertical cell count to not be a whole number.", _gridSettings.Key);
                return;
            }

            RuntimeGridRotation = _gridSettings.GridRotation;
            RuntimeGridPosition = _gridSettings.GridPosition;

            _grid = new PlacementGrid(_gridSettings, DeleteObjectFn, PlacementGridSetupCallback);
            _gridDisplayManager.Setup(this, _gridSettings);

            GridTransformGO = new GameObject("Grid Object Container");
            GridTransformGO.transform.position = RuntimeGridPosition;
            GridTransformGO.transform.rotation = Quaternion.Euler(new Vector3(0, RuntimeGridRotation, 0));

            if (_gridDisplayManager.IsDisplaying)
                _gridDisplayManager.Hide();

            GridManagerAccessor.RegisterGridManager(_gridSettings.Key, this);

            _unavailableGridCellDisplayManager.Setup(_gridSettings);

            CreateGridInputDetector();

            _gridObjectPlacementManager.OnGridPositionChanged += HandleGridObjectPositionUpdated;

            IsSetup = true;
            OnGridManagerSetup?.Invoke(this);
        }

        private void PlacementGridSetupCallback()
        {
            OnPlacementGridPopulated?.Invoke();
        }

        private void HandleGridObjectPositionUpdated(Vector2Int gridCellPosition)
        {
            OnObjectPositionUpdated?.Invoke(gridCellPosition);
        }

        private bool DeleteObjectFn(GameObject gameObject)
        {
            Destroy(gameObject);
            return true;
        }

        private void CreateGridInputDetector()
        {
            _gridInputDetectorGO = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _gridInputDetectorGO.name = "Grid Input Detector";
            _gridInputDetectorGO.transform.localScale = new Vector3((float)_gridSettings.Width / 10, 0, (float)_gridSettings.Height / 10);
            _gridInputDetectorGO.transform.position = RuntimeGridPosition;
            _gridInputDetectorGO.transform.rotation = Quaternion.Euler(new Vector3(0, RuntimeGridRotation, 0));

            _gridInputDetectorGO.layer = LayerMask.NameToLayer("Grid");

            if (_gridInputDetectorGO.layer == 0)
            {
                Debug.LogError("There was an error while setting up the Grid manager. Please ensure the 'Grid' Layer exists in the scene.");
                return;
            }

            _gridInputDetectorGO.AddComponent<BoxCollider>();

            if (_gridInputDetectorGO.TryGetComponent(out MeshRenderer meshRenderer))
            {
                meshRenderer.enabled = false;
            }

            _gridInputDetectorGO.transform.parent = transform;

            if (!GridUtilities.ValidateGridInputDefinitions(GridSettings))
            {
                return;
            }

            GridInputDefinition gridInputDefinition = GridUtilities.GetInputDefinitionForRuntime(_gridSettings);

            if (gridInputDefinition == null)
            {
                Debug.LogErrorFormat("No input definitions setup for Platform: {0} ", Application.platform);
                return;
            }

            if (_gridInputDetectorGO.TryGetComponent(out GridInputDetector gridInputDetector))
            {
                _gridInputDetector = gridInputDetector;
            }
            else
            {
                _gridInputDetector = _gridInputDetectorGO.AddComponent<GridInputDetector>();
            }

            _gridInputDetector.Setup(HandlePointerHitGrid, _gridSettings, this);

            _gridInputDetectorGO.SetActive(false);
        }

        private void HandlePointerHitGrid(Vector3 worldPos)
        {
            if (_gridObjectPlacementManager.ObjectToPlace != null)
            {
                UpdateGridObjectPlacement(worldPos);
            }
        }

        private void UpdateGridObjectPlacement(Vector3 worldPos)
        {
            _gridObjectPlacementManager.UpdateObjectPosition(worldPos);

            PlacementValidResponse placementValidResponse = IsObjectPlacementValid();
            bool placementValid = placementValidResponse.Valid;

            _gridObjectVisualManager.SetIsValid(placementValid);
            UpdatePlacementIndicator(placementValidResponse);
        }

        /// <summary>
        /// Displays the grid.
        /// </summary>
        public void DisplayGrid()
        {
            if (_gridInputDetectorGO != null)
            {
                _gridInputDetectorGO.SetActive(true);
            }

            _gridDisplayManager.Display();
            OnGridShown?.Invoke();

            UpdateOccupiedGridCellImages();
        }



        /// <summary>
        /// Hides the grid.
        /// </summary>
        public void HideGrid()
        {
            if (_unavailableGridCellDisplayManager != null)
            {
                _unavailableGridCellDisplayManager.HideGridCellImages(gameObject.transform);
            }

            if (_gridInputDetectorGO != null)
            {
                _gridInputDetectorGO.SetActive(false);
            }

            if (_gridCellPlacementIndicator != null)
            {
                _gridCellPlacementIndicator.Clear();
            }

            OnGridHidden?.Invoke();
            _gridDisplayManager.Hide();
        }

        /// <summary>
        /// Modify the placement of an already placed grid object.
        /// </summary>
        /// <param name="gridObject">The grid object that is to be modified. </param>
        public void ModifyPlacementOfGridObject(GameObject gridObject)
        {
            if (_gridObjectPlacementManager.IsPlacingObject)
            {
                Debug.LogWarning("Cannot modify placement of object: " + gridObject.name + " as there is already an object being placed.");
                return;
            }

            if (!gridObject.TryGetComponent(out GridObjectInfo gridObjectInfo))
            {
                Debug.LogError("Error. No grid object info found on the game object you are trying to modify the place of.");
                return;
            }

            OnStartModifyObjectPlacement?.Invoke(gridObject);

            EnterPlacementMode(gridObject);
        }

        /// <summary>
        /// Passes the grid object to the grid system and enters placement mode. 
        /// </summary>
        /// <param name="gridObjectToPlace"> The object that is being added to the grid </param>
        /// <param name="placementSettings"> Optional settings that control where the object is first positioned </param>
        public void EnterPlacementMode(GameObject gridObjectToPlace, PlacementSettings placementSettings = null)
        {
            if (_gridObjectPlacementManager.IsPlacingObject)
            {
                Debug.LogWarning("Cannot enter placement mode as there is already an object being placed.");
                return;
            }

            _gridObjectVisualManager.Setup(_gridSettings, gridObjectToPlace);
            _gridObjectPlacementManager.Setup(this, _gridSettings, gridObjectToPlace);

            if (!_gridDisplayManager.IsDisplaying)
            {
                DisplayGrid();
            }

            gridObjectToPlace.TryGetComponent(out GridObjectInfo gridObjectInfo);
            bool isModifyingExisitingGridObject = !string.IsNullOrEmpty(gridObjectInfo.GridKey);

            if (isModifyingExisitingGridObject)
            {
                Vector2Int rawGridCellIndex = gridObjectInfo.GridCellIndex;
                Vector3 worldPositionOfPlacedObject = GridUtilities.GetWorldPositionFromCellIndex(this, rawGridCellIndex, _gridSettings, RuntimeGridPosition);

                _gridObjectPlacementManager.UpdateSnapAlignment(gridObjectInfo.ObjectAlignment);
                _gridObjectPlacementManager.UpdateObjectPosition(worldPositionOfPlacedObject);
                _gridObjectPlacementManager.HandleGridObjectRotated();
                _unavailableGridCellDisplayManager.RemoveGridCellImage(rawGridCellIndex);

                _grid.SetGridObjectCellsAvailablity(rawGridCellIndex, true);
                _grid.RemoveGridObject(rawGridCellIndex);

                UpdateGridObjectPlacement(worldPositionOfPlacedObject);
            }
            else // If New to Grid
            {
                Vector3 initialPlacementPosition = GridUtilities.GetInitialPlacementPosition(this, _gridSettings, placementSettings);
                UpdateGridObjectPlacement(initialPlacementPosition);
            }

            PlacementValidResponse placementValidResponse = IsObjectPlacementValid();
            bool placementValid = placementValidResponse.Valid;

            _gridObjectVisualManager.SetIsValid(placementValid);

            _gridCellPlacementIndicator.Setup(this, _gridDisplayManager.GridTransform, _gridSettings, _gridObjectPlacementManager.ObjectBounds, _gridObjectPlacementManager.ObjectAlignment, gridObjectToPlace.transform.rotation.eulerAngles);
            UpdatePlacementIndicator(placementValidResponse);

            SubscribeToCustomValidation();

            OnPlacementModeStarted?.Invoke();
        }

        /// <summary>
        /// Change the snap position of the current object being placed.
        /// </summary>
        /// <param name="objectAlignment">The new alignment for the grid object</param>
        public void ChangeAlignment(ObjectAlignment objectAlignment)
        {
            if (_gridObjectPlacementManager.ObjectToPlace == null)
            {
                Debug.LogWarning("Cannot change the alignment as there is not an object being placed.");
                return;
            }

            _gridObjectPlacementManager.UpdateSnapAlignment(objectAlignment);

            PlacementValidResponse placementValidResponse = IsObjectPlacementValid();
            bool placementValid = placementValidResponse.Valid;

            _gridObjectVisualManager.SetIsValid(placementValid);

            _gridCellPlacementIndicator.UpdateObjectAlignment(objectAlignment);

            UpdatePlacementIndicator(placementValidResponse);
        }


        /// <summary>
        /// Used to confirm the placement of the grid object in it's current position. You can only confirm the placement when the object position in the grid is valid. 
        /// E.G it's not partially outside of the grid or overlapping another grid object.
        /// </summary>
        /// <returns> Returns if the object was successfully placed </returns>
        public bool ConfirmPlacement()
        {
            if (!_gridObjectPlacementManager.IsPlacingObject)
            {
                Debug.LogWarning("Unable to confirm the placement as the grid manager is not currently placing an object");
                return false;
            }

            PlacementValidResponse placementValidResponse = IsObjectPlacementValid();
            bool placementValid = placementValidResponse.Valid;

            if (placementValid)
            {
                return PlaceObject();
            }

            return false;
        }

        private bool PlaceObject()
        {
            GameObject gridObject = _gridObjectPlacementManager.ObjectToPlace;
            Vector3 objectSize = _gridObjectPlacementManager.GetObjectRelativeSize();

            Vector3 worldPositionOfObjectWithAlignment = _gridObjectPlacementManager.GetCurrentPositionOfObject();

            Vector2Int gridCellIndexObjectIsAssignedTo = _gridObjectPlacementManager.GridCellOfSelectedPosition;

            float objectRotation = _gridObjectPlacementManager.GetObjectRotation();
            UnSubscribeToCustomValidation();

            _gridObjectPlacementManager.PlaceObject(gridCellIndexObjectIsAssignedTo);
            _gridObjectVisualManager.ResetObjectMaterial();

            GameObject gridCellIndicatorClone = _gridCellPlacementIndicator.GetCloneOfHighlightCell(gameObject.transform);

            // By the time the cloned object has been created the position may be outdated. So we use the below function to force it to the position it was when the placement was confirmed.
            _gridCellPlacementIndicator.MoveToWorldPosition(gridCellIndicatorClone.GetComponent<RectTransform>(), gridCellIndexObjectIsAssignedTo, new Vector3(0, objectRotation, 0));
            _unavailableGridCellDisplayManager.AddGridCellIamge(gridCellIndexObjectIsAssignedTo, gridCellIndicatorClone);
            _gridCellPlacementIndicator.Clear();

            bool success = _grid.AddObjectToGrid(gridObject, gridCellIndexObjectIsAssignedTo, objectSize);

            if (!success)
            {
                return false;
            }

            if (!_paintMode)
            {
                HideGrid();
            }

            if (_gridSettings.ParentToGrid)
            {
                gridObject.transform.SetParent(GridTransformGO.transform, true);
            }
            else
            {
                gridObject.transform.SetParent(null);
            }

            _gridObjectPlacementManager.ResetObjectPlacementContainer();

            OnGridObjectPlaced?.Invoke(gridObject);

            return true;
        }

        /// <summary>
        /// Determines if the current object being placed is allowed to be placed at it's current position
        /// </summary>
        /// <returns> 
        /// Returns a response which determines if the placement is valid or not.
        /// If the placement is invalid a reason is provided.
        /// </returns>
        public PlacementValidResponse IsObjectPlacementValid()
        {
            Vector3 objectSize = _gridObjectPlacementManager.GetObjectRelativeSize();
            ObjectAlignment objectAlignment = _gridObjectPlacementManager.ObjectAlignment;

            Vector2Int XYObjectSnappedTo = _gridObjectPlacementManager.GridCellOfSelectedPosition;

            PlacementValidResponse res = _grid.CanPlaceObject(XYObjectSnappedTo, objectSize);

            // If the placement is valid check the custom validation. Saves getting the component for all times it's already invalid.
            if (res.Valid)
            {
                GameObject objectToPlace = _gridObjectPlacementManager.ObjectToPlace;

                CustomValidator customValidation = objectToPlace.GetComponent<CustomValidator>();

                if (customValidation != null && !customValidation.IsValid)
                {
                    res = new PlacementValidResponse(false, PlacementInvalidReason.CUSTOM_VALIDATION_FALED);
                }
            }

            OnPlacementValidated?.Invoke(res.Valid);

            return res;
        }

        /// <summary>
        /// Returns the world space position of the center of the grid
        /// </summary>
        /// <returns></returns>
        public Vector3 GetGridPosition() => RuntimeGridPosition;

        private void UpdateOccupiedGridCellImages()
        {
            if (_gridSettings.ShowOccupiedCells)
            {
                if (_unavailableGridCellDisplayManager != null)
                {
                    _unavailableGridCellDisplayManager.ShowGridCellImages(_gridDisplayManager.GridTransform);
                }
            }
        }


        /// <summary>
        /// Used to get a grid object at a specific grid cell index.
        /// </summary>
        /// <param name="cellX"></param>
        /// <param name="cellY"></param>
        /// <returns>The grid object at the requested grid position, or null if no grid object is placed in that position </returns>
        public GameObject GetGridObjectAtCellIndex(int cellX, int cellY)
        {
            return _grid.GetGridCellFromIndex(new Vector2Int(cellX, cellY)).GetGridObject();
        }


        /// <summary>
        /// Deletes a grid object and removes it from the grid.
        /// </summary>
        /// <param name="gridObject"></param>
        /// <returns></returns>
        public bool DeleteObject(GameObject gridObject, bool hideGrid = true)
        {
            if (!gridObject.TryGetComponent(out GridObjectInfo gridObjectInfo))
            {
                Debug.LogError("Error in Grid Manager. You cannot delete " + gridObject.name + " from the grid as it doesn't have the GridObjectInfo component attached. Make sure this is not removed");
                return false;
            }

            UnSubscribeToCustomValidation();

            Vector2Int rawGridCellIndex = gridObjectInfo.GridCellIndex;
            Vector3 worldSpaceOfObject = GridUtilities.GetWorldPositionFromCellIndex(this, rawGridCellIndex, _gridSettings, RuntimeGridPosition);

            Vector2Int gridCellIndexWithOffset = rawGridCellIndex;

            _grid.DeleteGridObject(gridCellIndexWithOffset);

            _unavailableGridCellDisplayManager.RemoveGridCellImage(gridCellIndexWithOffset);

            OnGridObjectDeleted?.Invoke();

            if (hideGrid)
            {
                HideGrid();
            }

            if (_paintMode && _gridObjectPlacementManager.ObjectToPlace != gridObject)
            {
                // If in continuoius placement mode then the object passed in to delete will not be the 
                // object currently being placed so we don't want the normal clearing of the managers to happen. 
                // Just the delete.
                Destroy(gridObject);
            }
            else
            {
                _gridCellPlacementIndicator.Clear();
                _gridObjectVisualManager.Clear();
                _gridObjectPlacementManager.ClearPlacingObject();
            }

            return true;
        }


        /// <summary>
        /// Removes any referenecs to the object from the grid. However does not destory the Gameobject
        /// </summary>
        public bool RemoveObjectFromGrid(GameObject gridObject)
        {
            if (!gridObject.TryGetComponent(out GridObjectInfo gridObjectInfo))
            {
                Debug.LogError("Error in Grid Manager. You cannot remove " + gridObject.name + " from the grid as it doesn't have the GridObjectInfo component attached. Make sure this is not removed");
                return false;
            }

            _unavailableGridCellDisplayManager.RemoveGridCellImage(gridObjectInfo.GridCellIndex);
            _grid.RemoveGridObject(gridObjectInfo.GridCellIndex);
            Destroy(gridObjectInfo);
            return true;
        }

        /// <summary>
        /// Cancel the current placement of a grid object and hide the grid.
        /// </summary>
        public void CancelPlacement(bool hideGrid = true)
        {
            if (_gridObjectPlacementManager.ObjectToPlace != null)
            {
                _gridObjectVisualManager.ResetObjectMaterial();

            }

            UnSubscribeToCustomValidation();

            _gridObjectPlacementManager.ClearPlacingObject();

            if (hideGrid && _gridDisplayManager.IsDisplaying)
            {
                HideGrid();
            }
        }

        /// <summary>
        /// Updated the grid object after a rotation has been applied.
        /// </summary>
        public void HandleGridObjectRotated()
        {
            GameObject gridObject = _gridObjectPlacementManager.ObjectToPlace;

            // If the object has been placed, free up the cells
            if (gridObject.TryGetComponent(out GridObjectInfo gridObjectInfo))
            {
                _grid.SetGridObjectCellsAvailablity(gridObjectInfo.GridCellIndex, true);
            }

            _gridObjectPlacementManager.HandleGridObjectRotated();

            PlacementValidResponse placementValidResponse = IsObjectPlacementValid();
            bool placementValid = placementValidResponse.Valid;

            _gridObjectVisualManager.SetIsValid(placementValid);

            _gridCellPlacementIndicator.HandleObjectRotated(gridObject.transform.rotation.eulerAngles, _gridObjectPlacementManager.GridCellOfSelectedPosition, placementValidResponse);
        }


        /// <summary>
        /// This function provides a way to add an object to the grid without going through the placement flow. 
        /// If you are adding multiple objects to the grid at once this function MUST be awaited. Or use the function PopulateWithGridData.
        /// </summary>
        /// <param name="gridObject">The object to add to the grid</param>
        /// <param name="gridCellIndex">The cell index where the grid object should be placed. </param>
        /// <param name="objectAlignment">The alignment of the object. By default it is Upper Left</param>
        /// <returns> If the grid was successfully populated with the grid data </returns>
        public async Task<bool> AddObjectToGrid(GameObject gridObject, Vector2Int gridCellIndex, ObjectAlignment objectAlignment = ObjectAlignment.UPPER_LEFT)
        {
            bool success = await AddObjectToGridByCellAsync(gridObject, gridCellIndex, objectAlignment);

            if (!success)
            {
                Debug.LogErrorFormat("There was an error adding the object: {0} to the grid at cell index: {1} with the alignment: {2}. Ensure the position data is valid",
                    gridObject.name, gridCellIndex, objectAlignment);
            }

            return success;

        }

        /// <summary>
        /// Allows you populate the grid with multiple objects at once. 
        /// </summary>
        /// <param name="gridData">Contains the required information for placing the grid objects in the grid</param>
        /// <param name="clearGrid"> Determines if the current grid objects should be deleted.</param>
        /// <returns> If the grid was successfully populated with the grid data </returns>
        public async Task<bool> PopulateWithGridDataAsync(GridData gridData, bool clearGrid = true)
        {
            bool finised = false;
            bool success = false;

            StartCoroutine(PopulateWithGridData(gridData, clearGrid, (successResult) =>
            {
                success = successResult; 
                finised = true;
            }));

            while (!finised)
            {
                await Task.Delay(1);
            }

            return success;
        }

        public IEnumerator PopulateWithGridData(GridData gridData, bool clearGrid = true, Action<bool> successCallback = null)
        {
            if (clearGrid)
            {
                ClearGrid();
                yield return new WaitForEndOfFrame();
            }

            List<GridObjectPositionData> gridObjectPositionData = gridData.GridObjectPositionDatas;

            for (int i = 0; i < gridObjectPositionData.Count; i++)
            {
                bool success = false;

                yield return AddObjectToGridByCell(gridObjectPositionData[i].GridObject, gridObjectPositionData[i].GridCellIndex, gridObjectPositionData[i].ObjectAlignment, (successResult) =>
                {
                    success = successResult;
                });

                if (!success)
                {
                    Debug.LogErrorFormat("There was an error adding the object: {0} to the grid at cell index: {1} with the alignment: {2}. Ensure the position data is valid",
                        gridObjectPositionData[i].GridObject, gridObjectPositionData[i].GridCellIndex, gridObjectPositionData[i].ObjectAlignment);

                    successCallback?.Invoke(false);
                    yield break;
                }
            }

            successCallback?.Invoke(true);
        }

        /// <summary>
        /// This function allows for objects to be added programatically without having to go though the placement mode flow.
        /// If you are calling this function is a loop you MUST wait for each task to finish before calling this function again. This can
        /// be achieved by using the await operators.
        /// </summary>
        /// <param name="gridObject"> The object to add the grid </param>
        /// <param name="gridCellIndex"> the position on the grid where the object should spawn </param>
        /// <param name="objectAlignment"> The desired alignment of the object </param>
        /// <returns> If the placement was successul </returns>
        public async Task<bool> AddObjectToGridByCellAsync(GameObject gridObject, Vector2Int gridCellIndex, ObjectAlignment objectAlignment)
        {
            bool finised = false;
            bool success = false;

            StartCoroutine(AddObjectToGridByCell(gridObject, gridCellIndex, objectAlignment, (successResult) =>
            {
                success = successResult;
                finised = true;
            }));

            while (!finised)
            {
                await Task.Delay(1);
            }

            return success;
        }

        /// <summary>
        /// This function allows for objects to be added programatically without having to go though the placement mode flow.
        /// If you are calling this function is a loop you MUST wait for each coroutine to finish before calling this function again. This can
        /// be achieved by using the yield return operators.
        /// </summary>
        /// <param name="gridObject"> The object to add the grid </param>
        /// <param name="gridCellIndex"> the position on the grid where the object should spawn </param>
        /// <param name="objectAlignment"> The desired alignment of the object </param>
        /// <returns> If the placement was successul </returns>
        public IEnumerator AddObjectToGridByCell(GameObject gridObject, Vector2Int gridCellIndex, ObjectAlignment objectAlignment, Action<bool> callback)
        {
            Quaternion gridObjectInitialRotation = gridObject.transform.localRotation;
            gridObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

            yield return new WaitForFixedUpdate();

            // Add gridInfo component
            if (!gridObject.TryGetComponent(out GridObjectInfo gridObjectInfo))
            {
                gridObject.AddComponent<GridObjectInfo>().Setup(_gridSettings.Key, gridCellIndex, objectAlignment);
            }

            Vector2Int rawGridCellIndex = gridCellIndex;

            var defaultAlignmentTemp = _gridSettings.DefaultAlignment;
            _gridSettings.DefaultAlignment = objectAlignment;

            _gridObjectPlacementManager.Setup(this, _gridSettings, gridObject);

            Vector3 worldPositionOfPlacedObject = GridUtilities.GetWorldPositionFromCellIndex(this, rawGridCellIndex, _gridSettings, RuntimeGridPosition);

            gridObject.transform.localRotation = gridObjectInitialRotation;
            _gridObjectPlacementManager.HandleGridObjectRotated();

            _gridObjectPlacementManager.UpdateObjectPosition(worldPositionOfPlacedObject);

            yield return new WaitForEndOfFrame();

            PlacementValidResponse placementValidResponse = IsObjectPlacementValid();
            bool placementValid = placementValidResponse.Valid;

            if (!placementValid)
            {
                _gridSettings.DefaultAlignment = defaultAlignmentTemp;
                Debug.LogErrorFormat("Error placeing object {0} onto the grid at grid cell index: {1}. Invalid placement reason: {2}", gridObject.name, rawGridCellIndex, placementValidResponse.PlacementInvalidReason.ToString());
                callback?.Invoke(false);
                yield break;
            }

            _gridCellPlacementIndicator.Setup(this, gameObject.transform, _gridSettings, _gridObjectPlacementManager.ObjectBounds, _gridObjectPlacementManager.ObjectAlignment, gridObject.transform.localRotation.eulerAngles);
            Vector3 objectBounds = _gridObjectPlacementManager.ObjectBounds;

            UpdatePlacementIndicator(placementValidResponse);

            yield return new WaitForEndOfFrame();

            _unavailableGridCellDisplayManager.AddGridCellIamge(rawGridCellIndex, _gridCellPlacementIndicator.GetCloneOfHighlightCell(gameObject.transform));
            _unavailableGridCellDisplayManager.HideGridCellImages(gameObject.transform);

            _gridObjectPlacementManager.PlaceObject(rawGridCellIndex);
            bool success = _grid.AddObjectToGrid(gridObject, rawGridCellIndex, objectBounds);


            if (!success)
            {
                Debug.LogErrorFormat("Error placeing object {0} onto the grid at grid cell index: {1}.", gridObject.name, rawGridCellIndex);
                callback?.Invoke(false);
                yield break;
            }

            if (_gridSettings.ParentToGrid)
            {
                gridObject.transform.SetParent(GridTransformGO.transform, true);
            }
            else
            {
                gridObject.transform.SetParent(null);
            }

            // After unparenting the rotation needs adjusting
            gridObject.transform.localRotation = gridObjectInitialRotation;


            _gridSettings.DefaultAlignment = defaultAlignmentTemp;

            _gridObjectPlacementManager.ResetObjectPlacementContainer();

            OnGridObjectPlaced?.Invoke(gridObject);

            _gridCellPlacementIndicator.Clear();

            callback?.Invoke(true);
        }

        /// <summary>
        /// Rotate the grid to a specified rotation.
        /// </summary>
        /// <param name="rotation">The rotation to set the grid to</param>
        public void RotateGridTo(float rotation)
        {
            float newRot = rotation % 360;

            float diff = newRot - RuntimeGridRotation;

            diff = Mathf.RoundToInt(diff * 10000) / 10000;

            RuntimeGridRotation = newRot;

            if (_gridDisplayManager.IsDisplaying)
            {
                _gridDisplayManager.UpdateRotation(newRot);
            }

            _gridInputDetectorGO.transform.rotation = Quaternion.Euler(new Vector3(0, newRot, 0));
            GridTransformGO.transform.rotation = Quaternion.Euler(new Vector3(0, newRot, 0));

            if (_gridDisplayManager.IsDisplaying)
            {
                _gridObjectPlacementManager.HandleGridRotationUpdated(diff);
                _gridCellPlacementIndicator.HandleGridRotationUpdated(_gridObjectPlacementManager.ObjectToPlace.transform.rotation.eulerAngles);
            }

        }

        /// <summary>
        /// Moves the grid to a specified position.
        /// </summary>
        /// <param name="position">The position to move the grid to</param>
        public void MoveGridTo(Vector3 position)
        {
            Vector3 diff = position - RuntimeGridPosition;
            RuntimeGridPosition = position;

            if (_gridDisplayManager.IsDisplaying)
            {
                _gridDisplayManager.UpdateGridPosition(position);
            }

            GridTransformGO.transform.position = position;
            _gridInputDetectorGO.transform.position = position;
            _gridObjectPlacementManager.HandleGridPositionUpdated(diff);
        }

        /// <summary>
        /// Enables the grid input detection from the grid input definition assigned.
        /// </summary>
        public void EnableGridInput()
        {
            GridInputDefinition.EnableInput();
        }

        /// <summary>
        /// Disables the grid input detection from the grid input definition assigned.
        /// </summary>
        public void DisableGridInput()
        {
            GridInputDefinition.DisableInput();
        }

        /// <summary>
        /// Clears the grid of all current objects assigned to it.
        /// </summary>
        /// <param name="destroyGridObjects"> Determines if the grid objects being cleared should be destoryed or have the references from the grid removed. </param>
        public void ClearGrid(bool destroyGridObjects = true)
        {
            List<GridObjectPositionData> objectPositionDatas = new List<GridObjectPositionData>();

            foreach (var item in GridData.GridObjectPositionDatas)
            {
                objectPositionDatas.Add(item);
            }

            int count = objectPositionDatas.Count;

            for (int i = count - 1; i >= 0; i--)
            {
                GameObject gridObject = objectPositionDatas[i].GridObject;

                if (destroyGridObjects)
                {
                    DeleteObject(gridObject);
                }
                else
                {
                    RemoveObjectFromGrid(gridObject);
                }
            }
        }

        private void UpdatePlacementIndicator(PlacementValidResponse placementValidResponse)
        {
            _gridCellPlacementIndicator.UpdateHighlightGridCell(_gridObjectPlacementManager.GridCellOfSelectedPosition, placementValidResponse);
        }

        /// <summary>
        /// Sets the grid to paint mode. Allows for continuous placement of objects.
        /// </summary>
        /// <param name="gridObjectToPlacePrefab"> The prefab of the object to use in paint mode </param>
        public void StartPaintMode(GameObject gridObjectToPlacePrefab)
        {
            if (_paintMode)
            {
                return;
            }

            _paintMode = true;
            _paintObjectPrefab = gridObjectToPlacePrefab;

            OnGridObjectPlaced += HandleGridObjectPlaced;

            PlaceItemPaintMode();
        }

        /// <summary>
        /// End paint mode. 
        /// </summary>
        /// <param name="hideGrid"> Hide the grid when paint mode has ended. </param>
        public void EndPaintMode(bool hideGrid = true)
        {
            _paintMode = false;
            _paintObjectPrefab = null;
            OnGridObjectPlaced -= HandleGridObjectPlaced;

            GameObject currentGridObject = _gridObjectPlacementManager.ObjectToPlace;

            if (currentGridObject != null)
            {
                Destroy(currentGridObject);
            }

            CancelPlacement(hideGrid);
        }

        private void HandleGridObjectPlaced(GameObject gridObject)
        {
            UpdateOccupiedGridCellImages();

            PlaceItemPaintMode();
        }

        private void PlaceItemPaintMode()
        {
            Vector3 pos = _gridInputDefinition.InputPosition().GetValueOrDefault(RuntimeGridPosition);
            GameObject gameObjectInstance = Instantiate(_paintObjectPrefab, pos, Quaternion.identity);

            PlacementSettings placementSettings = new PlacementSettings(pos);
            EnterPlacementMode(gameObjectInstance, placementSettings);
        }

        /// <summary>
        /// Change the input definitions used by the grid.
        /// </summary>
        /// <param name="platformGridInputsDefinitionMappings"> The new platform grid input definitions to use. </param>
        public void UpdatePlatformGridInputsDefinitionMappings(List<PlatformGridInputsDefinitionMapping> platformGridInputsDefinitionMappings)
        {
            _gridInputDetector.UpdateplatformGridInputsDefinitionMappings(platformGridInputsDefinitionMappings);
        }

        /// <summary>
        /// Finds the grid objects that are surrounding a grid object
        /// </summary>
        /// <param name="gridObject">The grid object to search around</param>
        /// <param name="searchRadius">The amount of cells from the edge of the object to search </param>
        /// <returns></returns>
        public List<GameObject> GetSurroundingObjects(GameObject gridObject, int searchRadius = 1)
        {
            GridObjectInfo gridObjectInfo = gridObject.GetComponent<GridObjectInfo>();

            if (gridObjectInfo == null)
            {
                Debug.LogErrorFormat("Cannot find surrounding objects for the object {0}. It is missing the GridObjectInfo component. Check it is placed on the grid.", gridObject.name);
                return null;
            }

            Vector2Int gridCellIndex;
            Vector3 objectBounds;

            // If the target object is currently being placed we can get the bounds from there.
            if (_gridObjectPlacementManager.ObjectToPlace == gridObject)
            {
                objectBounds = _gridObjectPlacementManager.ObjectBounds;
                gridCellIndex = _gridObjectPlacementManager.GridCellOfSelectedPosition;
            }
            // Otherwise we can get the bounds from the object info.
            else
            {
                if (!gridObjectInfo.ObjectBounds.HasValue)
                {
                    Debug.LogErrorFormat("Cannot get the surroudning objects for the grid object: {0} as it does not have the object bounds property set.", gridObject.name);
                    return null;
                }

                gridCellIndex = gridObjectInfo.GridCellIndex;
                objectBounds = gridObjectInfo.ObjectBounds.Value;
            }


            List<Vector2Int> cellIndexesOfObject = GridUtilities.GetCellIndexesRequiredForObject(gridCellIndex, objectBounds, _gridSettings.CellSize);
            List<Vector2Int> searchArea = GridUtilities.GetSurroundingIndexesInclusive(_gridSettings, cellIndexesOfObject, searchRadius);
            List<GameObject> surroudningObjects = GridUtilities.GetGridObjectsWithinArea(_grid, searchArea);

            // Remove the object from the results
            if (surroudningObjects.Contains(gridObject))
            {
                surroudningObjects.Remove(gridObject);
            }

            return surroudningObjects;
        }

        /// <summary>
        /// Determines if an object is able to be placed at a certain grid cell.
        /// </summary>
        /// <param name="gridObject"> The object that will be placed at the given index </param>
        /// <param name="gridCellIndex">The grid position to check availabilty at </param>
        /// <returns>If the placement is valid</returns>
        public bool CanAddObjectAtCell(GameObject gridObject, Vector2Int gridCellIndex)
        {
            // Calculate size for object.
            var x = Instantiate(gridObject);
            GridUtilities.CalculateObjectSize(_gridSettings, x, out Vector3 objectSize, out _);
            List<Vector2Int> requiredGridCells = GridUtilities.GetCellIndexesRequiredForObject(gridCellIndex, objectSize, _gridSettings.CellSize);

            // Check the indexes don't overlap and are inside the grid
            for (int i = 0; i < requiredGridCells.Count; i++)
            {
                Vector2Int requiredCell = requiredGridCells[i];

                if (!GridUtilities.IsIndexWithinGrid(_gridSettings, requiredCell))
                {
                    return false;
                }

                if (_grid.GetGridCellFromIndex(requiredCell).GetGridObject() != null)
                {
                    return false;
                }
            }

            return true;
        }

        #region Custom Validation
        private void SubscribeToCustomValidation()
        {
            if (_gridObjectPlacementManager.ObjectToPlace == null)
            {
                return;
            }

            CustomValidator customValidation = _gridObjectPlacementManager.ObjectToPlace.GetComponent<CustomValidator>();

            if (customValidation != null)
            {
                customValidation.OnValidationChanged += HandleCustomValidationChanged;
            }
        }

        private void UnSubscribeToCustomValidation()
        {
            if (_gridObjectPlacementManager.ObjectToPlace == null)
            {
                return;
            }

            CustomValidator customValidation = _gridObjectPlacementManager.ObjectToPlace.GetComponent<CustomValidator>();

            if (customValidation != null)
            {
                customValidation.OnValidationChanged -= HandleCustomValidationChanged;
            }
        }

        private void HandleCustomValidationChanged(bool isValid)
        {
            GameObject gridObject = _gridObjectPlacementManager.ObjectToPlace;

            // If the object has been placed, free up the cells
            if (gridObject.TryGetComponent(out GridObjectInfo gridObjectInfo))
            {
                _grid.SetGridObjectCellsAvailablity(gridObjectInfo.GridCellIndex, true);
            }

            PlacementValidResponse placementValidResponse = IsObjectPlacementValid();
            bool placementValid = placementValidResponse.Valid;

            _gridObjectVisualManager.SetIsValid(placementValid);

            UpdatePlacementIndicator(placementValidResponse);
        }

        #endregion

        #region Editor Methods
        public void DisplayGridEditor()
        {
            SetupEditor();
            _gridDisplayManager.Setup(this, _gridSettings);
            DisplayGrid();
        }

        public void HideGridEditor()
        {
            GameObject currentGrid = GameObject.Find("Placement Grid Canvas " + _gridSettings.Key);

            if (currentGrid != null)
            {
                DestroyImmediate(currentGrid);
            }
        }

        /// <summary>
        /// Sets up the class when called from the editor
        /// </summary>
        private void SetupEditor()
        {
            if (!gameObject.TryGetComponent(out _gridDisplayManager))
            {
                _gridDisplayManager = gameObject.AddComponent<GridDisplayManager>();
                _gridDisplayManager.Setup(this, _gridSettings);
            }
        }
        #endregion Editor Methods
    }
}