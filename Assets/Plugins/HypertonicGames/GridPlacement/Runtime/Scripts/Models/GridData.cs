using System.Collections.Generic;

namespace Hypertonic.GridPlacement.Models
{
    [System.Serializable]
    public class GridData
    {
        public string GridKey;
        public List<GridObjectPositionData> GridObjectPositionDatas;

        public GridData()
        {

        }

        public GridData(List<GridObjectPositionData> gridObjectPositionDatas)
        {
            GridObjectPositionDatas = gridObjectPositionDatas;
        }

        public GridData(string gridKey, List<GridObjectPositionData> gridObjectPositionDatas)
        {
            GridKey = gridKey;
            GridObjectPositionDatas = gridObjectPositionDatas;
        }
    }
}