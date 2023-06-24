using Hypertonic.GridPlacement.Enums;
using UnityEngine;

namespace Hypertonic.GridPlacement.Models
{
    public class GridObjectInfo : MonoBehaviour
    {
        public string GridKey { get; private set; }

        public Vector2Int GridCellIndex { get; private set; }

        public ObjectAlignment ObjectAlignment { get; private set; }

        public Vector3? ObjectBounds { get; set; }

        public Vector3? PositionOffset { get; set; }

        public bool HasBeenPlaced => !string.IsNullOrEmpty(GridKey);

        public void Setup(string gridKey, Vector2Int gridCellIndex, ObjectAlignment objectAlignment)
        {
            GridKey = gridKey;
            GridCellIndex = gridCellIndex;
            ObjectAlignment = objectAlignment;
        }
    }
}