using UnityEngine;

namespace Hypertonic.GridPlacement.Models
{
    /// <summary>
    /// Defines settings to provide more control for where the object is initially placed when it enters placementmode.
    /// </summary>
    [System.Serializable]
    public class PlacementSettings
    {
        public readonly Vector2Int? GridCellIndex = null;
        public readonly Vector3? WorldPosition = null;

        /// <summary>
        /// Constructor that sets the grid cell index to place the object at when it first enters placement mode.
        /// </summary>
        /// <param name="gridCellIndex">The grid coordinates to place the object at. </param>
        public PlacementSettings(Vector2Int gridCellIndex)
        {
            GridCellIndex = gridCellIndex;
        }

        /// <summary>
        /// Constructor that sets the spawn position of the object. 
        /// The Y property is ignored. 
        /// </summary>
        /// <param name="worldPosition">The world position to place the object at. </param>
        public PlacementSettings(Vector3 worldPosition)
        {
            WorldPosition = worldPosition;
        }
    }
}