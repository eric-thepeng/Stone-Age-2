using UnityEngine;

namespace Hypertonic.GridPlacement.Models
{
    [System.Serializable]
    public struct Vector3MinMax
    {
        public Vector3 Min;
        public Vector3 Max;

        public Vector3MinMax(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }
    }
}