using UnityEngine;

namespace Hypertonic.GridPlacement.Interfaces
{
    public interface IGridCell
    {
        void Initialise(int x, int y);

        void SetObject(GameObject gridObject);
    }
}