using Hypertonic.GridPlacement.Enums;
using UnityEngine;

namespace Hypertonic.GridPlacement.Example.AddProgramatically.Models
{
    [System.Serializable]
    public class GridObjectSpawnData
    {
        public GameObject GridObject;
        public Vector2Int GridCellIndex;
        public ObjectAlignment ObjectAlignment;
        public Vector3 ObjectRotation;
    }
}