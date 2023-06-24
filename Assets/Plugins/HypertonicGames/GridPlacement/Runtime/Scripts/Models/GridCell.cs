using Hypertonic.GridPlacement.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Hypertonic.GridPlacement.Models
{
    public class GridCell : IGridCell
    {
        public List<GridCell> Neighbours { get; private set; } = new List<GridCell>();

        private Vector2Int _gridPosition;

        private GameObject _gridObject;
        private bool _isAvailable = true;

        public void Initialise(int x, int y)
        {
            _gridPosition = new Vector2Int(x, y);
        }

        public void SetObject(GameObject gridObject)
        {
            _gridObject = gridObject;
            _isAvailable = false;
        }

        public void ClearObject()
        {
            _gridObject = null;
            _isAvailable = true;
        }

        public GameObject GetGridObject()
        {
            return _gridObject;
        }

        public bool IsAvailable()
        {
            return _isAvailable;
        }

        public void SetAvailabilty(bool isAvailable)
        {
            _isAvailable = isAvailable;
        }

        public void AddNeighbourIndex(GridCell neighbourCell)
        {
            if (neighbourCell == this) return;

            Neighbours.Add(neighbourCell);
        }

        public List<Vector2Int> GetNeighbourCellIndexes()
        {
            List<Vector2Int> cellIndexes = new List<Vector2Int>();

            for(int i = 0; i < Neighbours.Count; i++)
            {
                cellIndexes.Add(Neighbours[i]._gridPosition);
            }

            return cellIndexes;
        }

        public void Clear()
        {
            _gridObject = null;
            _isAvailable = true;

            foreach (var cell in Neighbours)
            {
                cell.ClearObject();
            }

            Neighbours.Clear();
        }
    }
}