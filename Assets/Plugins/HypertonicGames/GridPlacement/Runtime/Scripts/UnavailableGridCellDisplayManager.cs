using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement
{
    /// <summary>
    /// Used to manage the objects that display which grid cells are available.
    /// Storing the objects that cover numerous cells is more efficient that creating and showing images for each cell.
    /// </summary>
    public class UnavailableGridCellDisplayManager : MonoBehaviour
    {
        private Dictionary<Vector2Int, GameObject> _gridCellImages = new Dictionary<Vector2Int, GameObject>();
        private GridSettings _gridSettings;

        public void Setup(GridSettings gridSettings)
        {
            _gridSettings = gridSettings;
        }

        public void AddGridCellIamge(Vector2Int gridCellIndex, GameObject gridCellImage)
        {
            if(_gridCellImages.ContainsKey(gridCellIndex))
            {
                _gridCellImages[gridCellIndex] = gridCellImage;
            }
            else
            {
                gridCellImage.GetComponent<Image>().color = _gridSettings.CellColourPlaced;
                _gridCellImages.Add(gridCellIndex, gridCellImage);
            }
        }

        public void RemoveGridCellImage(Vector2Int gridCellIndex)
        {
            if (_gridCellImages.ContainsKey(gridCellIndex))
            {
                Destroy(_gridCellImages[gridCellIndex]);
                _gridCellImages.Remove(gridCellIndex);
            }
        }

        public void ShowGridCellImages(Transform parent)
        {
            foreach(var gridCellImage in _gridCellImages.Values)
            {
                if(gridCellImage == null)
                {
                    continue;
                }

                gridCellImage.SetActive(true);
                gridCellImage.transform.SetParent(parent, false);
            }
        }

        public void HideGridCellImages(Transform parent)
        {
            foreach (var gridCellImage in _gridCellImages.Values)
            {
                if (gridCellImage == null)
                {
                    continue;
                }

                gridCellImage.SetActive(false);
                gridCellImage.transform.SetParent(parent, false);
            }
        }
    }
}