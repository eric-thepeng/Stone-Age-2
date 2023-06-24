using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.CompleteDemo
{
    /// <summary>
    /// This example manager is responsbile for updating the nearby object count of an object being placed. The text is hidden when the grid isn't being shown in this 
    /// example. However you can check the nearby objects of any object that's been placed at anytime.
    /// </summary>
    public class NearbyObjectManager : MonoBehaviour
    {
        [Tooltip("The amount of cells away from the object to search for")]
        [SerializeField]
        private int _searchRadius = 1;

        [SerializeField]
        // As well as setting a default, the UI can also change the search radius
        private InputField _searchRadiusInput;

        [SerializeField]
        private Text _nearbyObjectDisplayText;

        private void Start()
        {
            if (_nearbyObjectDisplayText == null)
            {
                Debug.LogError("The nearby object display text was not set");
                return;
            }
            _nearbyObjectDisplayText.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            GridManager.OnGridManagerSetup += HandleGridManagerSetup;
            GridManager.OnGridManagerDestroyed += HandleGridManagerDestroyed;
            _searchRadiusInput.onValueChanged.AddListener(HandleSearchRadiusInputChanged);
        }

        private void OnDisable()
        {
            GridManager.OnGridManagerSetup -= HandleGridManagerSetup;
            GridManager.OnGridManagerDestroyed -= HandleGridManagerDestroyed;
            _searchRadiusInput.onValueChanged.RemoveListener(HandleSearchRadiusInputChanged);
        }

        private void HandleGridManagerSetup(GridManager gridManager)
        {
            gridManager.OnObjectPositionUpdated += HandleObjectPositionUpdated;
            gridManager.OnGridShown += HandleGridShown;
            gridManager.OnGridHidden += HandleGridHidden;
        }

        private void HandleGridManagerDestroyed(GridManager gridManager)
        {
            gridManager.OnObjectPositionUpdated -= HandleObjectPositionUpdated;
            gridManager.OnGridShown -= HandleGridShown;
            gridManager.OnGridHidden -= HandleGridHidden;
        }
        
        private void HandleGridShown()
        {
            _nearbyObjectDisplayText.gameObject.SetActive(true);
        }

        private void HandleGridHidden()
        {
            _nearbyObjectDisplayText.gameObject.SetActive(false);
        }

        private void HandleObjectPositionUpdated(Vector2Int gridPosition)
        {
            GridManager gridManager = GridManagerAccessor.GridManager;

            if (!gridManager.IsPlacingGridObject)
            {
                return;
            }

            // For this example we're going to see what objects are near the object being placed. However you can check the 
            // nearby objects of any object provided it's been placed on the grid.
            List<GameObject> nearbyGameObjects = gridManager.GetSurroundingObjects(gridManager.ObjectToPlace, _searchRadius);

            _nearbyObjectDisplayText.text = string.Format("Nearyby Object Count: {0}", nearbyGameObjects.Count);
        }

        private void HandleSearchRadiusInputChanged(string value)
        {
            if(int.TryParse(value, out int searchRadius))
            {
                _searchRadius = searchRadius;
            }
        }
    }
}