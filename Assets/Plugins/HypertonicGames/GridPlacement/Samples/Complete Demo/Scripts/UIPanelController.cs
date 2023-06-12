using Hypertonic.GridPlacement.Enums;
using Hypertonic.GridPlacement.Example.CompleteDemo.Buttons.NearbyObjectCount;
using Hypertonic.GridPlacement.Example.CompleteDemo.Buttons.PlacementSettings;
using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.CompleteDemo
{
    /// <summary>
    /// This UIPanelController for the sample scene is responsible for displaying and hiding the different UI in the scene.
    /// </summary>
    public class UIPanelController : MonoBehaviour
    {
        [SerializeField]
        private Button _addAnObjectButton;

        [SerializeField]
        private GameObject _objectSelectionUI;

        [SerializeField]
        private GameObject _gridObjectAlignmentControls;

        [SerializeField]
        private GameObject _gridPlacementControls;

        [SerializeField]
        private GameObject _optionsMenu;

        [SerializeField]
        private GameObject _nearbyObjectsUI;

        [SerializeField]
        private GameObject _placementSettingsUI;

        private void Start()
        {
            _addAnObjectButton.onClick.AddListener(HandleAddAnObjectButtonPressed);

            GameManager.OnNewGridObjectCreated += HandleNewGridObjectCreated;
            Button_CloseObjectSelection.OnCloseButtonPressed += HandleCloseObjectSelectionPressed;
            Button_OpenChangeAlignmentOptions.OnOpenChangeAlignmentOptionEvent += HandleOpenChangeAlignmentPressed;
            Button_ChangeAlignment.OnChangeAlignmentPressed += HandleChangeAlignmentPressed;
            Button_CancelPlacement.OnCancelPlacementPressed += HandleCancelPlacementPressed;
            PlacementContolsManager.OnObjectPlacedOnGrid += HandleObjectPlacedOnGrid;
            ExampleGridObject.OnObjectSelected += HandleExampleGridObjectSelected;
            Button_OpenOptionsMenu.OnOpenOptionsMenuPressed += HandleOptionsMenuPressed;
            Button_DisplayChangeNearbyObjectsOptions.OnDisplayChangeNearbyObjectsOptions += HandleDisplayChangeNearbyObjectsObjectsPressed;
            Button_CloseNearbyObjectOptions.OnCloseMenuPressed += HandleCloseNearbyObjectOptionsPressed;
            Button_DisplayPlacementOptions.OnDisplayPlacementOptionsPressed += HandleDisplayPlacementOptionsPressed;
            Button_ConfirmPlacementSettings.OnConfirmPlacementPressed += HandleConfirmPlacementSettingPressed;
        }

        private void OnDestroy()
        {
            _addAnObjectButton.onClick.RemoveListener(HandleAddAnObjectButtonPressed);

            GameManager.OnNewGridObjectCreated -= HandleNewGridObjectCreated;
            Button_CloseObjectSelection.OnCloseButtonPressed -= HandleCloseObjectSelectionPressed;
            Button_OpenChangeAlignmentOptions.OnOpenChangeAlignmentOptionEvent -= HandleOpenChangeAlignmentPressed;
            Button_ChangeAlignment.OnChangeAlignmentPressed -= HandleChangeAlignmentPressed;
            Button_CancelPlacement.OnCancelPlacementPressed -= HandleCancelPlacementPressed;
            PlacementContolsManager.OnObjectPlacedOnGrid -= HandleObjectPlacedOnGrid;
            ExampleGridObject.OnObjectSelected -= HandleExampleGridObjectSelected;
            Button_OpenOptionsMenu.OnOpenOptionsMenuPressed -= HandleOptionsMenuPressed;
            Button_DisplayChangeNearbyObjectsOptions.OnDisplayChangeNearbyObjectsOptions -= HandleDisplayChangeNearbyObjectsObjectsPressed;
            Button_CloseNearbyObjectOptions.OnCloseMenuPressed -= HandleCloseNearbyObjectOptionsPressed;
            Button_DisplayPlacementOptions.OnDisplayPlacementOptionsPressed -= HandleDisplayPlacementOptionsPressed;
            Button_ConfirmPlacementSettings.OnConfirmPlacementPressed -= HandleConfirmPlacementSettingPressed;
        }

        private void HandleNewGridObjectCreated(GameObject gridObject)
        {
            _objectSelectionUI.SetActive(false);
            _gridPlacementControls.SetActive(true);
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
            _gridPlacementControls.SetActive(false);
        }

        private void HandleChangeAlignmentPressed(ObjectAlignment objectAlignment)
        {
            _gridObjectAlignmentControls.SetActive(false);
        }

        private void HandleObjectPlacedOnGrid()
        {
            GridManagerAccessor.GridManager.OnGridObjectDeleted -= HandleGridObjectDeleted;

            _gridPlacementControls.SetActive(false);
        }

        private void HandleExampleGridObjectSelected(GameObject gridObject)
        {
            GridManagerAccessor.GridManager.OnGridObjectDeleted += HandleGridObjectDeleted;

            _gridPlacementControls.SetActive(true);
        }

        private void HandleGridObjectDeleted()
        {
            GridManagerAccessor.GridManager.OnGridObjectDeleted -= HandleGridObjectDeleted;

            _gridPlacementControls.SetActive(false);
        }

        private void HandleOptionsMenuPressed()
        {
            _optionsMenu.SetActive(true);
        }

        private void HandleDisplayChangeNearbyObjectsObjectsPressed()
        {
            _optionsMenu.SetActive(false);
            _nearbyObjectsUI.SetActive(true);
        }

        private void HandleCloseNearbyObjectOptionsPressed()
        {
            _nearbyObjectsUI.SetActive(false);
        }

        private void HandleDisplayPlacementOptionsPressed()
        {
            _placementSettingsUI.SetActive(true);
            _optionsMenu.SetActive(false);
        }

        private void HandleConfirmPlacementSettingPressed()
        {
            _placementSettingsUI.SetActive(false);
        }
    }
}