using Hypertonic.GridPlacement.Example.BasicDemo;
using Hypertonic.GridPlacement.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.MultipleGrids
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private GridSettings _gridSettingsA;

        [SerializeField]
        private GridSettings _gridSettingsB;

        [SerializeField]
        private Button _selectGridAButton;

        [SerializeField]
        private Button _selectGridBButton;

        [SerializeField]
        private Text _selectedGridText;

        private void Awake()
        {
            CreateGridManagers();
        }

        private void OnEnable()
        {
            _selectGridAButton.onClick.AddListener(HandleSelectGridAButtonPressed);
            _selectGridBButton.onClick.AddListener(HandleSelectGridBButtonPressed);
            ExampleGridObject.OnObjectSelected += HandleExampleGridObjectSelected;
        }

        private void OnDisable()
        {
            _selectGridAButton.onClick.RemoveListener(HandleSelectGridAButtonPressed);
            _selectGridBButton.onClick.RemoveListener(HandleSelectGridBButtonPressed);
            ExampleGridObject.OnObjectSelected -= HandleExampleGridObjectSelected;
        }

        private void CreateGridManagers()
        {
            GameObject gridManagerObjectA = new GameObject("Grid Manager A");
            GridManager gridManagerA = gridManagerObjectA.AddComponent<GridManager>();
            gridManagerA.Setup(_gridSettingsA);


            GameObject gridManagerObjectB = new GameObject("Grid Manager B");
            GridManager gridManagerB = gridManagerObjectB.AddComponent<GridManager>();
            gridManagerB.Setup(_gridSettingsB);
        }

        private void HandleSelectGridAButtonPressed()
        {
            GridManagerAccessor.SetSelectedGridManager(_gridSettingsA.Key);
            _selectedGridText.text = "Selected Grid: Grid A";
        }

        private void HandleSelectGridBButtonPressed()
        {
            GridManagerAccessor.SetSelectedGridManager(_gridSettingsB.Key);
            _selectedGridText.text = "Selected Grid: Grid B";
        }

        private void HandleExampleGridObjectSelected(GameObject gameObject)
        {
            string gridKeyOfSelectedObject = gameObject.GetComponent<GridObjectInfo>().GridKey;
            GridManagerAccessor.SetSelectedGridManager(gridKeyOfSelectedObject);
        }
    }
}