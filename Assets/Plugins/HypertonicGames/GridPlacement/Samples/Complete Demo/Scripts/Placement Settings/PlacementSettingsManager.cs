using Hypertonic.GridPlacement.Example.CompleteDemo.Buttons.PlacementSettings;
using Hypertonic.GridPlacement.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.CompleteDemo
{

    public class PlacementSettingsManager : MonoBehaviour
    {
        [SerializeField]
        private Dropdown _placementTypeDropDown;

        [SerializeField]
        private GameObject _gridCellInputUI;

        [SerializeField]
        private InputField _gridCellInputX;

        [SerializeField]
        private InputField _gridCellInputY;

        [SerializeField]
        private GameObject _worldPositionInputUI;

        [SerializeField]
        private InputField _worldPositionX;

        [SerializeField]
        private InputField _worldPositionZ;


        private Vector2Int? _initialGridCellCoordinates = null;
        private Vector3? _initialWorldPosition = null;


        private PlacementSettingsType _selectedSettingsType;

        private void OnEnable()
        {
            Button_ConfirmPlacementSettings.OnConfirmPlacementPressed += HandleConfirmPlacementSettingsPressed;

            _placementTypeDropDown.onValueChanged.AddListener(HandleDropDownValueChanged);
        }

        private void OnDisable()
        {
            Button_ConfirmPlacementSettings.OnConfirmPlacementPressed -= HandleConfirmPlacementSettingsPressed;

            _placementTypeDropDown.onValueChanged.RemoveListener(HandleDropDownValueChanged);
        }

        public PlacementSettings GetPlacementSettings()
        {
            switch (_selectedSettingsType)
            {
                case PlacementSettingsType.GRID_CELL:
                    return GetGridCellInputPlacementSettings();
                case PlacementSettingsType.WORLD_POSITION:
                    return GetWorldPositionPlacementSettings();
                default:
                    return null;
            }
        }

        private void HandleConfirmPlacementSettingsPressed()
        {
            _initialGridCellCoordinates = GetGridCellInput();
            _initialWorldPosition = GetWorldPositionInput();
        }

        private void HandleDropDownValueChanged(int value)
        {
            _selectedSettingsType = (PlacementSettingsType)value;

            switch (_selectedSettingsType)
            {
                case PlacementSettingsType.GRID_CELL:
                    ShowGridCellInput();
                    break;
                case PlacementSettingsType.WORLD_POSITION:
                    ShowWorldPositionInput();
                    break;
            }
        }

        private void ShowGridCellInput()
        {
            _worldPositionInputUI.SetActive(false);
            _gridCellInputUI.SetActive(true);
        }

        private void ShowWorldPositionInput()
        {
            _gridCellInputUI.SetActive(false);
            _worldPositionInputUI.SetActive(true);
        }

        /// <summary>
        /// Obtains the world position from the user input on the UI
        /// </summary>
        /// <returns>A vector 3 that is made up of the 2 input fields for the world position inputs</returns>
        private Vector3 GetWorldPositionInput()
        {
            if (!int.TryParse(_worldPositionX.text, out int x))
            {
                Debug.LogError("Input field for world position X is invalid");
                return Vector3.zero;
            }

            if (!int.TryParse(_worldPositionZ.text, out int z))
            {
                Debug.LogError("Input field for world position Z is invalid");
                return Vector3.zero;
            }

            return new Vector3(x, 0, z);
        }

        /// <summary>
        /// Obtains the grid cell coordinates from the user input on the UI
        /// </summary>
        /// <returns>A Vector2Int that is made up of the 2 input fields for the grid cell position inputs</returns>
        private Vector2Int GetGridCellInput()
        {
            if (!int.TryParse(_gridCellInputX.text, out int x))
            {
                Debug.LogError("Input field for grid cell coordinate X is invalid");
                return Vector2Int.zero;
            }

            if (!int.TryParse(_gridCellInputY.text, out int y))
            {
                Debug.LogError("Input field for grid cell coordinate Y is invalid");
                return Vector2Int.zero;
            }

            return new Vector2Int(x, y);
        }

        /// <summary>
        /// Used to obtain a placement settings object with the grid coordinates if the coordinates have been set in the UI. Otherwise a null object is returned which will
        /// result in the GridSettings default initial placement cell index being used.
        /// </summary>
        /// <returns>Placement settings with the initial cell coordinates or null </returns>
        private PlacementSettings GetGridCellInputPlacementSettings()
        {
            if (!_initialGridCellCoordinates.HasValue)
                return null;

            return new PlacementSettings(_initialGridCellCoordinates.Value);
        }


        /// <summary>
        /// Used to obtain a placement settings object with the world position if the world position have been set in the UI. Otherwise a null object is returned which will
        /// result in the GridSettings default initial placement cell index being used.
        /// </summary>
        /// <returns>Placement settings with the initial world position or null </returns>
        private PlacementSettings GetWorldPositionPlacementSettings()
        {
            if (!_initialWorldPosition.HasValue)
                return null;

            return new PlacementSettings(_initialWorldPosition.Value);
        }
    }
}