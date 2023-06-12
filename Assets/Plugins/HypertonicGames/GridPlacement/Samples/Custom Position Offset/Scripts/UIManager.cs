using Hypertonic.GridPlacement.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.PositionOffsetDemo
{
    /// <summary>
    /// This UIManager for the sample scene is responsible for displaying and hiding the different UI in the scene.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private Button _addAnObjectButton;

        [SerializeField]
        private GameObject _headerUI;

        [SerializeField]
        private GameObject _objectSelectionUI;

        [SerializeField]
        private GameObject _gridObjectAlignmentControls;

        [SerializeField]
        private GameObject _gridControls;

        private void Start()
        {
            _addAnObjectButton.onClick.AddListener(HandleAddAnObjectButtonPressed);

            Button_GridObjectSelectionOption.OnOptionSelected += HandleGridObjectOptionSelected;
            Button_CloseObjectSelection.OnCloseButtonPressed += HandleCloseObjectSelectionPressed;
            Button_OpenChangeAlignmentOptions.OnOpenChangeAlignmentOptionEvent += HandleOpenChangeAlignmentPressed;
            Button_ChangeAlignment.OnChangeAlignmentPressed += HandleChangeAlignmentPressed;
            Button_CancelPlacement.OnCancelPlacementPressed += HandleCancelPlacementPressed;
            CustomPlacementManager.OnObjectPlacedOnGrid += HandleObjectPlacedOnGrid;
            ExampleGridObject.OnObjectSelected += HandleExampleGridObjectSelected;
            GridControlManager.OnConfirmButtonPressed += HandleConfirmButtonPressed;
        }

        private void OnDestroy()
        {
            _addAnObjectButton.onClick.RemoveListener(HandleAddAnObjectButtonPressed);

            Button_GridObjectSelectionOption.OnOptionSelected -= HandleGridObjectOptionSelected;
            Button_CloseObjectSelection.OnCloseButtonPressed -= HandleCloseObjectSelectionPressed;
            Button_OpenChangeAlignmentOptions.OnOpenChangeAlignmentOptionEvent -= HandleOpenChangeAlignmentPressed;
            Button_ChangeAlignment.OnChangeAlignmentPressed -= HandleChangeAlignmentPressed;
            Button_CancelPlacement.OnCancelPlacementPressed -= HandleCancelPlacementPressed;
            CustomPlacementManager.OnObjectPlacedOnGrid -= HandleObjectPlacedOnGrid;
            ExampleGridObject.OnObjectSelected -= HandleExampleGridObjectSelected;
            GridControlManager.OnConfirmButtonPressed -= HandleConfirmButtonPressed;
        }

        private void HandleGridObjectOptionSelected(GameObject gridObject)
        {
            _objectSelectionUI.SetActive(false);
            _gridControls.SetActive(true);
            _headerUI.SetActive(false);
        }

        private void HandleAddAnObjectButtonPressed()
        {
            _objectSelectionUI.SetActive(true);
        }

        private void HandleCloseObjectSelectionPressed()
        {
            _objectSelectionUI.SetActive(false);
        }

        private void HandleOpenChangeAlignmentPressed()
        {
            _gridObjectAlignmentControls.SetActive(true);
        }

        private void HandleCancelPlacementPressed()
        {
            _gridControls.SetActive(false);
            _headerUI.SetActive(true);
        }

        private void HandleChangeAlignmentPressed(ObjectAlignment objectAlignment)
        {
            _gridObjectAlignmentControls.SetActive(false);
        }

        private void HandleObjectPlacedOnGrid()
        {
            GridManagerAccessor.GridManager.OnGridObjectDeleted -= HandleGridObjectDeleted;

            _gridControls.SetActive(false);
            _headerUI.SetActive(true);
        }

        private void HandleExampleGridObjectSelected(GameObject gridObject)
        {
            GridManagerAccessor.GridManager.OnGridObjectDeleted += HandleGridObjectDeleted;

            _gridControls.SetActive(true);
            _headerUI.SetActive(false);
        }

        private void HandleGridObjectDeleted()
        {
            GridManagerAccessor.GridManager.OnGridObjectDeleted -= HandleGridObjectDeleted;

            _gridControls.SetActive(false);
            _headerUI.SetActive(true);
        }

        private void HandleConfirmButtonPressed()
        {
            _gridControls.SetActive(false);
        }
    }
}