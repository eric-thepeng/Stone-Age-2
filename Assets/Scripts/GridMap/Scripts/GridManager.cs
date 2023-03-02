using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private GridMap<GridObject> grid;
    [SerializeField] Transform bottomLeftCorner;
    [SerializeField] Transform secondBottomLeftCorner;
    [SerializeField] Transform girdMarksContainer;

    class GridObject
    {
        GridMap<GridObject> myGrid;
        private int x;
        private int z;

        public GridObject(GridMap<GridObject> myGrid, int x, int z)
        {
            this.myGrid = myGrid;
            this.x = x;
            this.z = z;
        }

        public override string ToString()
        {
            return "" + x + "," + z;
        }
    }

    private void Start()
    {
        int gridWidth = 20;
        int gridHeight = 10;
        float cellSize = (secondBottomLeftCorner.position - bottomLeftCorner.position).magnitude;
        grid = new GridMap<GridObject>(gridWidth, gridHeight, cellSize, bottomLeftCorner.position);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int z = 0; z < grid.GetHeight(); z++)
            {
                GameObject go = WorldUtility.CreateWorldText("" + x + "," + z, girdMarksContainer, grid.GetWorldPosition(x, z), 5, null, TextAnchor.UpperCenter, TMPro.TextAlignmentOptions.Center).gameObject;
                go.transform.position += new Vector3(0, 0.1f, 0);
                go.transform.rotation = Quaternion.EulerAngles(45, 0, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
