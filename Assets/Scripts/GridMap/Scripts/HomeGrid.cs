using System.Collections;
using System.Collections.Generic;
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

    bool buildDrag = false;
    
    class BuildDragInfo
    {
        Vector3Int startCoord;
        Vector3Int endCoord;
        List<Vector3Int> allCoords;

        public BuildDragInfo(Vector3Int firstSpot)
        {
            startCoord = firstSpot;
            endCoord = startCoord;
            allCoords = new List<Vector3Int>();
            allCoords.Add(startCoord);
        }

        public void SetEndPosition(Vector3Int newEndCoord)
        {
            if (endCoord == newEndCoord) return;
            endCoord = newEndCoord;
            CalculateAllCoords();
        }

        void CalculateAllCoords()
        {
            allCoords.Clear();
            int xDir = (int)Mathf.Sign(endCoord.x - startCoord.x);
            int yDir = (int)Mathf.Sign(endCoord.y - startCoord.y);
            for(int i = startCoord.x; i != endCoord.x; i += xDir)
            {
                allCoords.Add(new Vector3Int(i,startCoord.y, startCoord.z));
            }/*
            for ()
            {
                allCoords.Add(new Vector3Int())
            }*/
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

        if (buildDrag)
        {

        }
        else
        {

        }

        if (Input.GetMouseButtonDown(0))
        {
            GridObject gro = grid.GetValue(x, z);
            if (gro == null) return;
            if (gro.CanBuild())
            {
                ClickBuild(gro);
            }
            else
            {
            }
        }
    }

    void ClickBuild(GridObject gro)
    {
        BuildingISO bisoToBuild = BuildingManager.i.GetSelectedBuildingISO();

        if (bisoToBuild == null) return;

        GameObject newPlacement = Instantiate(bisoToBuild.buildingPrefab, this.transform);
        newPlacement.transform.position = grid.GetWorldPositionFromPosition(WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.HOME_GRID, true));
        newPlacement.transform.localScale = new Vector3(cellSize, cellSize, cellSize);

        gro.SetTransform(newPlacement.transform);

        Inventory.i.InBuildItem(bisoToBuild, true);

        if(Inventory.i.ItemInStockAmount(bisoToBuild) == 0)
        {
            BuildingManager.i.CancelSelectedBuidling();
        }
        /*

        //gridIndication.GetComponentInChildren<MeshRenderer>().materials[0] = emptyMaterial;
        GameObject newPlacement = Instantiate(new GameObject("sprite", typeof(SpriteRenderer)), this.transform);
        newPlacement.GetComponent<SpriteRenderer>().sprite = testSprite;
        newPlacement.transform.rotation = Quaternion.EulerAngles(45, 0, 0);
        newPlacement.transform.position = grid.GetWorldPositionFromPosition(WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.HOME_GRID, true));
        newPlacement.transform.localScale = new Vector3(cellSize / 1.65f, cellSize / 1.65f, 1f);

        gro.SetTransform(newPlacement.transform);*/
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
