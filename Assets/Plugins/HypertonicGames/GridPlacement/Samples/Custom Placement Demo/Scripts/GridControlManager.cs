using Hypertonic.GridPlacement.Enums;
using Hypertonic.GridPlacement.Models;
using UnityEngine;

namespace Hypertonic.GridPlacement.Example.CustomPlacement
{
    /// <summary>
    /// This is an example of how you could implement a manager that handles user input for interating
    /// with a grid and performs the relevent actions on the grid manager.
    /// </summary>
    public class GridControlManager : MonoBehaviour
    {
        public delegate void GridControlManagerEvent();
        public static event GridControlManagerEvent OnObjectPlacedOnGrid;

        [SerializeField]
        private GameObject _cancelPlacementButton;

        [SerializeField]
        private GameObject _deleteObjectButton;

        [SerializeField]
        private PlacementSettingsManager _placementSettingsManager;

        private GameObject _selectedGridObject;

        private void Start()
        {
            Button_ConfirmPlacement.OnConfirmPlacementPressed += HandleConfirmButtonPressed;
            Button_RotateLeft.OnRotateLeftPressed += HandleRotateLeftPressed;
            Button_RotateRight.OnRotateRightPressed += HandleRotateRightPressed;
            Button_ChangeAlignment.OnChangeAlignmentPressed += HandleChangeAlignmentPressed;
            Button_CancelPlacement.OnCancelPlacementPressed += HandleCancelPlacementPressed;
            Button_Delete.OnDeletePressed += HandleDeleteObjectPressed;
            Button_GridObjectSelectionOption.OnOptionSelected += HandleGridObjectOptionSelected;
            ExampleGridObject.OnObjectSelected += HandleExampleGridObjectSelected;
            Button_GridObjectSelectionOption.OnOptionSelected += HandleGridObjectSelected;
        }

        private void HandleConfirmButtonPressed()
        {
            bool placed = GridManagerAccessor.GridManager.ConfirmPlacement();

            if (placed)
            {
                OnObjectPlacedOnGrid?.Invoke();
                _selectedGridObject = null;
            }
        }

        private void HandleGridObjectSelected(GameObject objectPrefab)
        {
            GameObject objectToPlace = Instantiate(objectPrefab, GridManagerAccessor.GridManager.GetGridPosition(), new Quaternion());
            _selectedGridObject = objectToPlace;

            objectToPlace.name = objectPrefab.name;

            if (!objectToPlace.TryGetComponent(out ExampleGridObject gridObject))
            {
                objectToPlace.AddComponent<ExampleGridObject>();
            }

            PlacementSettings placementSettings = _placementSettingsManager.GetPlacementSettings();

             GridManagerAccessor.GridManager.EnterPlacementMode(objectToPlace, placementSettings);
        }

        private void HandleRotateLeftPressed()
        {
            _selectedGridObject.transform.Rotate(new Vector3(0, -90, 0));

            GridManagerAccessor.GridManager.HandleGridObjectRotated();
        }

        private void HandleRotateRightPressed()
        {
            _selectedGridObject.transform.Rotate(new Vector3(0, 90, 0));

            GridManagerAccessor.GridManager.HandleGridObjectRotated();
        }

        private void HandleChangeAlignmentPressed(ObjectAlignment objectAlignment)
        {
            GridManagerAccessor.GridManager.ChangeAlignment(objectAlignment);
        }

        private void HandleCancelPlacementPressed()
        {
            GridManagerAccessor.GridManager.CancelPlacement();
            _selectedGridObject = null;
        }

        private void HandleDeleteObjectPressed()
        {
            GridManagerAccessor.GridManager.DeleteObject(_selectedGridObject);
            _selectedGridObject = null;
        }

        private void HandleGridObjectOptionSelected(GameObject gridObject)
        {
            _cancelPlacementButton.SetActive(true);
            _deleteObjectButton.SetActive(false);
        }

        private void HandleExampleGridObjectSelected(GameObject gridObject)
        {
            _selectedGridObject = gridObject;

            _cancelPlacementButton.SetActive(false);
            _deleteObjectButton.SetActive(true);

            GridManagerAccessor.GridManager.ModifyPlacementOfGridObject(gridObject);
        }
    }
}