using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HomeGrid : MonoBehaviour
{
    private GridMap<GridObject> grid;
    [SerializeField] Transform bottomLeftCorner;
    [SerializeField] Transform secondBottomLeftCorner;
    [SerializeField] Transform girdMarksContainer;
    [SerializeField] Sprite testSprite;

    [SerializeField] Material emptyMaterial;
    [SerializeField] Material occupiedMaterial;

    float cellSize;

    //BuildDragInfo buildDragInfo = null;

    class GridObject
    {

        GridMap<GridObject> myGrid;
        private int x;
        private int z;
        Transform transform = null;
        public GridObject(GridMap<GridObject> myGrid, int x, int z)
        {
            this.myGrid = myGrid;
            this.x = x;
            this.z = z;
        }

        public int getX() { return x; }
        public int getZ() { return z; }

        public override string ToString()
        {
            return "" + x + "," + z;
        }

        public void SetTransform(Transform newTransform)
        {
            transform = newTransform;
            myGrid.TriggerGridObjectChanged(x, z);
        }

        public void ClearTransform()
        {
            transform = null;
            myGrid.TriggerGridObjectChanged(x, z);
        }

        public bool CanBuild()
        {
            return transform == null;
        }
    }

    //float gridIndicationSpeed = 10;
    //Vector3 NO_GRID_INDICATION = new Vector3(-10, -10, -10);
    //Vector3 gridIndicationTargetPos;

    

    private void Start()
    {
        int gridWidth = 25;
        int gridHeight = 15;
        cellSize = (secondBottomLeftCorner.position - bottomLeftCorner.position).magnitude;
        grid = new GridMap<GridObject>(gridWidth, gridHeight, cellSize, bottomLeftCorner.position);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int z = 0; z < grid.GetHeight(); z++)
            {
                GameObject go = WorldUtility.CreateWorldText("" + x + "," + z, girdMarksContainer, grid.GetWorldPosition(x, z), 4, null, TextAnchor.UpperCenter, TMPro.TextAlignmentOptions.Center).gameObject;
                go.transform.position += new Vector3(0, 0.1f, 0);
                go.transform.rotation = Quaternion.EulerAngles(45, 0, 0);

                grid.SetValue(x,z, new GridObject(grid,x,z));

            }
        }

        HideGridLines();
        BuildingManager.i.AddHomeGrid(this);
    }

    public Vector3 GetGridWorldPositionFromPosition(Vector3 hitPoint)
    {
        return grid.GetWorldPositionFromPosition(hitPoint);//new Vector3(0, 0, 0);//TODO
    }

    public void GetGridCoordFromPosition(Vector3 hitPoint, out int x, out int y)
    {
        grid.GetXZ(hitPoint,out x,out y);
    }

    public GameObject BuildWithCoord(int x, int z, bool placeHolder = false)
    {
        print("x " + x + " z " + z);
        GridObject gro = grid.GetValue(x, z);
        if (gro == null) return null;
        if (!gro.CanBuild()) return null;

        Vector3 spawnScale = new Vector3(cellSize, cellSize, cellSize);
        Vector3 displayOffSet = new Vector3(0, 0, 0);

        BuildingISO bisoToBuild = BuildingManager.i.GetSelectedBuildingISO();
        GameObject buildingPreafab;
        if (placeHolder)
        {
            //buildingPreafab = BuildingManager.i.placeholdingBuilding;
            float shrinkScale = 0.7f;
            buildingPreafab = bisoToBuild.buildingPrefab;
            spawnScale *= shrinkScale;
            displayOffSet = new Vector3(cellSize * bisoToBuild.GetWidth(), 0, cellSize * bisoToBuild.GetHeight()) * (1-shrinkScale)/2 ; //* 0.7f;
        }
        else
        {
            if (bisoToBuild == null) return null;
            buildingPreafab = bisoToBuild.buildingPrefab;
        }

        GameObject newPlacement = Instantiate(buildingPreafab, this.transform);
        newPlacement.transform.position = grid.GetWorldPosition(gro.getX(), gro.getZ()); //grid.GetWorldPositionFromPosition(WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.HOME_GRID, true));
        newPlacement.transform.localScale = spawnScale;
        newPlacement.transform.position += displayOffSet;

        if (placeHolder) return newPlacement;

        foreach(Vector2Int v in bisoToBuild.getCoordinates())
        {
            grid.GetValue(x + v.x, z + v.y).SetTransform(newPlacement.transform);
        }

        Inventory.i.InBuildItem(bisoToBuild, true);

        if (Inventory.i.ItemInStockAmount(bisoToBuild) == 0)
        {
            BuildingManager.i.CancelSelectedBuidling();
        }

        return newPlacement;
    }

    public bool CanBuild(Vector2Int startPoint, List<Vector2Int> displacements)
    {
        Vector2Int testPoint;
        GridObject gro;
        foreach(Vector2Int dis in displacements)
        {
            testPoint = startPoint + dis;
            gro = grid.GetValue(testPoint.x, testPoint.y);
            if (gro == null) return false;
            if (!gro.CanBuild()) return false;
        }
        return true;
    }

    public void ShowGridLines()
    {
        //girdMarksContainer.gameObject.SetActive(true);
    }

    public void HideGridLines()
    {
        girdMarksContainer.gameObject.SetActive(false);
    }

}
