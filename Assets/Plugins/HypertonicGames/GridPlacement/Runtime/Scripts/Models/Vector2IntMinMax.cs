using UnityEngine;

namespace Hypertonic.GridPlacement.Models
{
    [System.Serializable]
    public class Vector2IntMinMax 
    {
        public Vector2Int Min;
        public Vector2Int Max;

        public Vector2IntMinMax(Vector2Int min, Vector2Int max)
        {
            Min = min;
            Max = max;
        }
    }
}
