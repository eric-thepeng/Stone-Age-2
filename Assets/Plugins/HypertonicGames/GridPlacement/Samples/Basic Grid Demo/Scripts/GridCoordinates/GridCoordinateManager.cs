using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.BasicDemo.GridCoordinates
{
    public class GridCoordinateManager : MonoBehaviour
    {
        [SerializeField]
        private GridManager _gridManager;

        [SerializeField]
        private Text _gridCoordinates;

        private void Awake()
        {
            _gridCoordinates.text = string.Empty;
        }

        private void OnEnable()
        {
            _gridManager.OnObjectPositionUpdated += HandleGridObjectPositionUpdated;
            _gridManager.OnGridObjectPlaced += HandleGridObjectPlaced;
        }

        private void OnDisable()
        {
            _gridManager.OnObjectPositionUpdated -= HandleGridObjectPositionUpdated;
            _gridManager.OnGridObjectPlaced -= HandleGridObjectPlaced;
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