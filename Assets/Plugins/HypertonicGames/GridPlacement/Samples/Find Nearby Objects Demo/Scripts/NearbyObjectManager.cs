using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.BasicDemo.NearbyObjects
{
    public class NearbyObjectManager : MonoBehaviour
    {
        [Tooltip("The amount of cells away from the object to search for")]
        [SerializeField]
        private int _searchRadius = 1;

        [SerializeField]
        private Button _searchForNearByObjectsButton;


        private void Start()
        {
            if(_searchForNearByObjectsButton == null)
            {
                Debug.LogError("search for nearyby objects not set");
                return;
            }
            _searchForNearByObjectsButton.onClick.AddListener(HandleSearchButtonClicked);
        }

        private void HandleSearchButtonClicked()
        {
            GridManager gridManager = GridManagerAccessor.GridManager;

            if(!gridManager.IsPlacingGridObject)
            {
                return;
            }
            // For this example we're going to see what objects are near the object being placed. However you can check the 
            // nearby objects of any object provided it's been placed on the grid.
            List<GameObject> nearbyGameObjects = gridManager.GetSurroundingObjects(gridManager.ObjectToPlace, _searchRadius);

            Debug.Log("NEARBY OBJECTS FOUND. Nearby object count: " + nearbyGameObjects.Count);
        }
    }
}