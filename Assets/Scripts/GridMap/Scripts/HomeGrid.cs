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

    Transform gridIndication;
    float cellSize;

    BuildDragInfo buildDragInfo = null;
    
    class BuildDragInfo
    {
        HomeGrid homeGrid;
        Vector3Int startCoord;
        Vector3Int endCoord;
        List<Vector3Int> allCoords;
        List<GameObject> placeholders;

        public BuildDragInfo(Vector3Int firstSpot, HomeGrid hg)
        {
            homeGrid = hg;
            startCoord = firstSpot;
            endCoord = startCoord;
            allCoords = new List<Vector3Int>();
            placeholders = new List<GameObject>();
            //allCoords.Add(startCoord);
            CalculateAllCoords();
        }

        public void SetEndPosition(Vector3Int newEndCoord)
        {
            if (endCoord == newEndCoord) return;
            endCoord = newEndCoord;
            CalculateAllCoords();
        }

        public List<Vector3Int> GetAllCoords()
        {
            return allCoords;
        }

        void CalculateAllCoords()
        {
            allCoords.Clear();
            int xDir = endCoord.x > startCoord.x ? 1 : -1 ;
            int yDir = endCoord.z > startCoord.z ? 1 : -1;

            for (int i = startCoord.x; i != endCoord.x; i += xDir)
            {
                allCoords.Add(new Vector3Int(i,startCoord.y, startCoord.z));
            }
            if (!allCoords.Contains(new Vector3Int(endCoord.x, startCoord.y, startCoord.z))) allCoords.Add(new Vector3Int(endCoord.x, startCoord.y, startCoord.z));
            for (int k = startCoord.z; k != endCoord.z; k += yDir)
            {
                if (k == startCoord.z) continue;
                allCoords.Add(new Vector3Int(endCoord.x, startCoord.y, k));
            }
            if(!allCoords.Contains(endCoord)) allCoords.Add(endCoord);


            //spawn placeholders
            SpawnPlaceholders();
        }

        void SpawnPlaceholders()
        {
            DestroyPlaceholders();
            foreach(Vector3Int coord in allCoords)
            {
                placeholders.Add(homeGrid.BuildWithCoord(coord.x, coord.z, true));
            }
        }

        public void DestroyPlaceholders()
        {
            for(int i = placeholders.Count-1; i>=0; i--)
            {
                Destroy(placeholders[i]);
            }
            placeholders.Clear();
        }

    }

    //float gridIndicationSpeed = 10;
    //Vector3 NO_GRID_INDICATION = new Vector3(-10, -10, -10);
    //Vector3 gridIndicationTargetPos;

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

        gridIndication = transform.Find("GridIndication");
        gridIndication.localScale = new Vector3(cellSize, gridIndication.localScale.y ,cellSize);
        gridIndication.gameObject.SetActive(false);
        HideGridLines();
        BuildingManager.i.AddHomeGrid(this);
    }


    // Update is called once per frame
    void Update()
    {
        if (!BuildingManager.i.building) return; //return if not building mode

        if (!WorldUtility.TryMouseHitPoint(WorldUtility.LAYER.HOME_GRID, true)) //hide indicator if mouse is not on grid
        {
            gridIndication.gameObject.SetActive(false);
            return;
        }

        grid.GetXZ(WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.HOME_GRID, true), out int x, out int z);
        gridIndication.gameObject.SetActive(true);
        gridIndication.position = grid.GetWorldPosition(x, z);

        if (buildDragInfo == null) //new build drag
        {
            if (Input.GetMouseButtonDown(0))
            {
                buildDragInfo = new BuildDragInfo(new Vector3Int(x,0,z), this);
            }
        }
        else
        {
            buildDragInfo.SetEndPosition(new Vector3Int(x, 0, z));
            if (Input.GetMouseButtonUp(0))
            {
                foreach (Vector3Int i in buildDragInfo.GetAllCoords())
                {
                    print(i);
                    BuildWithCoord(i.x, i.z);
                }
                buildDragInfo.DestroyPlaceholders();
                buildDragInfo = null;
            }
        }
    }

    public GameObject BuildWithCoord(int x, int z, bool placeHolder = false)
    {
        print("x " + x + " z " + z);
        GridObject gro = grid.GetValue(x, z);
        if (gro == null) return null;
        if (!gro.CanBuild()) return null;

        BuildingISO bisoToBuild = BuildingManager.i.GetSelectedBuildingISO();
        GameObject buildingPreafab;
        if (placeHolder)
        {
            buildingPreafab = BuildingManager.i.placeholdingBuilding;
        }
        else
        {
            if (bisoToBuild == null) return null;
            buildingPreafab = bisoToBuild.buildingPrefab;
        }

        GameObject newPlacement = Instantiate(buildingPreafab, this.transform);
        newPlacement.transform.position = grid.GetWorldPosition(gro.getX(), gro.getZ()); //grid.GetWorldPositionFromPosition(WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.HOME_GRID, true));
        newPlacement.transform.localScale = new Vector3(cellSize, cellSize, cellSize);

        if (placeHolder) return newPlacement;

        gro.SetTransform(newPlacement.transform);
        Inventory.i.InBuildItem(bisoToBuild, true);

        if (Inventory.i.ItemInStockAmount(bisoToBuild) == 0)
        {
            BuildingManager.i.CancelSelectedBuidling();
        }

        return newPlacement;
    }

    public void ShowGridLines()
    {
        girdMarksContainer.gameObject.SetActive(true);
    }

    public void HideGridLines()
    {
        girdMarksContainer.gameObject.SetActive(false);
    }

}
