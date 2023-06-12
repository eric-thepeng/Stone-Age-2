using Hypertonic.GridPlacement.Enums;
using UnityEngine;

namespace Hypertonic.GridPlacement.Models
{
    [System.Serializable]
    public class GridObjectPositionData
    {
        public GameObject GridObject;
        public Vector2Int GridCellIndex;
        public ObjectAlignment ObjectAlignment;

        public GridObjectPositionData(GameObject gridObject, Vector2Int gridCellIndex, ObjectAlignment objectAlignment)
        {
            GridObject = gridObject;
            GridCellIndex = gridCellIndex;
            ObjectAlignment = objectAlignment;
        }
    }
}