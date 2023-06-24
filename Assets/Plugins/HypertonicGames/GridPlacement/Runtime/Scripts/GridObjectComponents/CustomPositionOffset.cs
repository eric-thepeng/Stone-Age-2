using UnityEngine;

namespace Hypertonic.GridPlacement.GridObjectComponents
{
    public class CustomPositionOffset : MonoBehaviour
    {
        public delegate void CustomPositionOverrideEvent(Vector3 position);
        public event CustomPositionOverrideEvent OnPositionChanged;

        /// <summary>
        /// Sets the desired position offset for an object being placed, instead of the default which makes the object flush to the grid.
        /// </summary>
        [SerializeField]
        private Vector3 _customPositionOverride;

        public Vector3 PositionOverride { get => _customPositionOverride; }

        public void SetCustomPositionOverride(Vector3 position)
        {
            _customPositionOverride = position;

            OnPositionChanged?.Invoke(position);
        }
    }
}
