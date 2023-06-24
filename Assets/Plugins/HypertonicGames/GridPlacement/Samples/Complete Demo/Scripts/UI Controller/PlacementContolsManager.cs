using Hypertonic.GridPlacement.Enums;
using UnityEngine;

namespace Hypertonic.GridPlacement.Example.CompleteDemo
{
    public class PlacementContolsManager : MonoBehaviour
    {
        public delegate void GridControlManagerEvent();
        public static event GridControlManagerEvent OnObjectPlacedOnGrid;

        [SerializeField]
        private GameObject _cancelPlacementButton;

        [SerializeField]
        private GameObject _deleteObjectButton;

        private GameObject _selectedGridObject;

        private void OnEnable()
        {
            Button_ConfirmPlacement.OnConfirmPlacementPressed += HandleConfirmButtonPressed;
            Button_RotateLeft.OnRotateLeftPressed += HandleRotateLeftPressed;
            Button_RotateRight.OnRotateRightPressed += HandleRotateRightPressed;
            Button_ChangeAlignment.OnChangeAlignmentPressed += HandleChangeAlignmentPressed;
            Button_CancelPlacement.OnCancelPlacementPressed += HandleCancelPlacementPressed;
            Button_Delete.OnDeletePressed += HandleDeleteObjectPressed;
            GameManager.OnNewGridObjectCreated += HandleNewGridObjectCreated;
            ExampleGridObject.OnObjectSelected += HandleExampleGridObjectSelected;
        }

        private void OnDisable()
        {
            Button_ConfirmPlacement.OnConfirmPlacementPressed -= HandleConfirmButtonPressed;
            Button_RotateLeft.OnRotateLeftPressed -= HandleRotateLeftPressed;
            Button_RotateRight.OnRotateRightPressed -= HandleRotateRightPressed;
            Button_ChangeAlignment.OnChangeAlignmentPressed -= HandleChangeAlignmentPressed;
            Button_CancelPlacement.OnCancelPlacementPressed -= HandleCancelPlacementPressed;
            Button_Delete.OnDeletePressed -= HandleDeleteObjectPressed;
            GameManager.OnNewGridObjectCreated -= HandleNewGridObjectCreated;
            ExampleGridObject.OnObjectSelected -= HandleExampleGridObjectSelected;
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

        private void HandleNewGridObjectCreated(GameObject gridObject)
        {
            _selectedGridObject = gridObject;

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