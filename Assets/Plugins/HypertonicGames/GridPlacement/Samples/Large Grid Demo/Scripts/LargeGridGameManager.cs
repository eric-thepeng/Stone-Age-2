using System.Collections;
using UnityEngine;

namespace Hypertonic.GridPlacement.Example.LargeGridDemo
{
    public class LargeGridGameManager : MonoBehaviour
    {
        [SerializeField]
        private GridSettings _gridSettings;

        [SerializeField]
        private GameObject _gameUI;

        [SerializeField]
        private GameObject _loadingUI;

        private bool _gridGenerationFinished = false;

        private void Start()
        {
            // If not already disable the game UI
            _gameUI.SetActive(false);

            _loadingUI.SetActive(true);

            StartCoroutine(InitialiseApp());
        }

        private IEnumerator InitialiseApp()
        {
            // Create the Grid Manager at the start of the games life cycle.
            GridManager gridManager = new GameObject("Grid Manager").AddComponent<GridManager>();
            gridManager.Setup(_gridSettings);

            // Add a listener to detect when the grid has finished populating.
            gridManager.OnPlacementGridPopulated += HandlePlacementGridPopulated;

            // Wait for the Grid to be populated.
            while (!_gridGenerationFinished)
            {
                yield return null;
            }

            // Remove the listener now that the grid has been populated
            gridManager.OnPlacementGridPopulated -= HandlePlacementGridPopulated;
            
            // Now grid has been populated continue with the game flow.
            _gameUI.SetActive(true);
            _loadingUI.SetActive(false);

            Debug.LogFormat("Finished generating a grid with {0} grid cells", _gridSettings.AmountOfCellsX * _gridSettings.AmountOfCellsY);
        }

        /// <summary>
        /// This is the handler for when the grid has been populated. It sets the _gridGenerationFinished to true, allowing the while loop to exit.
        /// </summary>
        private void HandlePlacementGridPopulated()
        {
            _gridGenerationFinished = true;
        }
    }
}
