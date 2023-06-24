using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.CompleteDemo.GridCoordinates
{
    public class GridCoordinateManager : MonoBehaviour
    {
        [SerializeField]
        private Text _gridCoordinates;

        private void Awake()
        {
            _gridCoordinates.text = string.Empty;
        }

        private void OnEnable()
        {
            GridManager.OnGridManagerSetup += HandleGridManagerSetup;
            GridManager.OnGridManagerDestroyed += HandleGridManagerDestroyed;
        }

        private void OnDisable()
        {
            GridManager.OnGridManagerSetup -= HandleGridManagerSetup;
            GridManager.OnGridManagerDestroyed -= HandleGridManagerDestroyed;
        }

        private void HandleGridManagerSetup(GridManager gridManager)
        {
            gridManager.OnObjectPositionUpdated += HandleGridObjectPositionUpdated;
            gridManager.OnGridObjectPlaced += HandleGridObjectPlaced;
        }

        private void HandleGridManagerDestroyed(GridManager gridManager)
        {
            gridManager.OnObjectPositionUpdated -= HandleGridObjectPositionUpdated;
            gridManager.OnGridObjectPlaced -= HandleGridObjectPlaced;
        }

        private void HandleGridObjectPositionUpdated(Vector2Int coordinates)
        {
            _gridCoordinates.text = string.Format("X: {0}  |  Y: {1}", coordinates.x, coordinates.y);
        }

        private void HandleGridObjectPlaced(GameObject gameObject)
        {
            _gridCoordinates.text = string.Empty;
        }
    }
}