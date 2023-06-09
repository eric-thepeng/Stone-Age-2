using Hypertonic.GridPlacement.Models;
using System;
using UnityEngine;

namespace Hypertonic.GridPlacement.Example.CompleteDemo
{
    public class GameManager : MonoBehaviour
    {
        // An event to let other manager's know that an object has been instantiated to add to the grid.
        public static Action<GameObject> OnNewGridObjectCreated;
        
        [SerializeField]
        private GridSettings _gridSettings;

        [SerializeField]
        private PlacementSettingsManager _placementSettingsManager;

        private void OnEnable()
        {
            Button_GridObjectSelectionOption.OnOptionSelected += HandleAddObjectToGridPressed;
        }

        private void OnDisable()
        {
            Button_GridObjectSelectionOption.OnOptionSelected -= HandleAddObjectToGridPressed;
        }

        private void Start()
        {
            CreateGridManager();
        }

        private void CreateGridManager()
        {
            // Create the grid manager progrmatically
            GridManager gridManager = new GameObject("Grid Manager").AddComponent<GridManager>();

            // Set up the grid manager with the settings
            gridManager.Setup(_gridSettings);
        }

        private void HandleAddObjectToGridPressed(GameObject gridObjectPrefab)
        {
            GameObject objectToPlace = Instantiate(gridObjectPrefab, GridManagerAccessor.GridManager.GetGridPosition(), new Quaternion());

            objectToPlace.name = gridObjectPrefab.name;

            if (!objectToPlace.TryGetComponent(out ExampleGridObject gridObject))
            {
                objectToPlace.AddComponent<ExampleGridObject>();
            }

            OnNewGridObjectCreated?.Invoke(objectToPlace);

            PlacementSettings placementSettings = _placementSettingsManager.GetPlacementSettings();

            GridManagerAccessor.GridManager.EnterPlacementMode(objectToPlace, placementSettings);
        }
    }
}