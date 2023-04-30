using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    static BuildingManager instance;
    public static BuildingManager i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BuildingManager>();
            }
            return instance;
        }
    }

    UI_InventoryBlock selectedUIIB = null;
    List<HomeGrid> allHomeGrids = new List<HomeGrid>();

    public bool building = false;
    public GameObject placeholdingBuilding;

    float gridCellSize; 

    class GridIndication
    {
        Transform indicationHolder;
        Transform indicationTemplate;
        List<Transform> displayingBlocks;

        Vector3 currentDisplayStartPosition = new Vector3(0,0,0); 

        public GridIndication(Transform indicationGameObject, Vector3 targetScale)
        {
            indicationHolder = indicationGameObject;
            indicationTemplate = indicationHolder.transform.Find("Indication Block Template");
            indicationTemplate.transform.localScale = targetScale * 0.95f;
            displayingBlocks = new List<Transform>();
            Hide();
        }

        public void Hide()
        {
            indicationTemplate.gameObject.SetActive(false);
            indicationHolder.gameObject.SetActive(false);

            for(int i = displayingBlocks.Count-1; i>=0; i--)
            {
                Destroy(displayingBlocks[i].gameObject);
            }

            displayingBlocks.Clear();
            currentDisplayStartPosition = new Vector3(0, 0, 0);
        }

        public void Display(Vector3 startPosition, BuildingISO biso = null)
        {
            //reduce repetition
            if (startPosition == currentDisplayStartPosition) return;
            Hide();
            currentDisplayStartPosition = startPosition;

            //set things active
            indicationHolder.gameObject.SetActive(true);
            indicationTemplate.gameObject.SetActive(true);

            //add to display coords
            List<Vector2Int> coordsToPlace = new List<Vector2Int>();
            if(biso == null)
            {
                coordsToPlace.Add(new Vector2Int(0, 0));
                //defaul
                /*
                coordsToPlace.Add(new Vector2Int(1, 0));
                coordsToPlace.Add(new Vector2Int(0, 1));
                coordsToPlace.Add(new Vector2Int(1, 1));*/
            }
            else
            {
                foreach (Vector2Int coord in biso.getCoordinates())
                {
                    coordsToPlace.Add(coord);
                }
            }

            //display
            foreach(Vector2Int coord in coordsToPlace)
            {
                Transform newBlock = Instantiate(indicationTemplate.gameObject, indicationHolder).transform;
                displayingBlocks.Add(newBlock);
                newBlock.position = startPosition + new Vector3(BuildingManager.i.gridCellSize * coord.x, 0, BuildingManager.i.gridCellSize * coord.y) + 0.5f * new Vector3(BuildingManager.i.gridCellSize, 0, BuildingManager.i.gridCellSize);
            }

            //hide template
            indicationTemplate.gameObject.SetActive(false);
        }
    }

    GridIndication gridIndication;

    class BuildDragInfo
    {
        HomeGrid homeGrid;
        Vector2Int startCoord;
        Vector2Int endCoord;
        List<Vector2Int> keyCoords;
        List<GameObject> placeholders;
        BuildingISO biso;

        public BuildDragInfo(Vector2Int firstSpot, HomeGrid hg, BuildingISO inputBISO)
        {
            homeGrid = hg;
            startCoord = firstSpot;
            endCoord = startCoord;
            keyCoords = new List<Vector2Int>();
            placeholders = new List<GameObject>();
            biso = inputBISO;
            CalculateAllCoords();
        }

        public void SetEndPosition(Vector2Int newEndCoord)
        {
            if (endCoord == newEndCoord) return;
            endCoord = newEndCoord;
            CalculateAllCoords();
        }

        public List<Vector2Int> GetKeyCoords()
        {
            return keyCoords;
        }

        void CalculateAllCoords()
        {
            if (biso == null) return;
            keyCoords.Clear();

            int xDir = biso.GetWidth();
            int yDir = biso.GetHeight();

            Vector2Int counter = new Vector2Int(startCoord.x, startCoord.y);

            if(counter.x <= endCoord.x)
            {
                AddCoord(counter);

                while (counter.x + xDir <= endCoord.x)
                {
                    counter += new Vector2Int(xDir, 0);
                    AddCoord(counter);
                }
            }
            else
            {
                AddCoord(counter);

                while (counter.x - xDir >= endCoord.x)
                {
                    counter -= new Vector2Int(xDir, 0);
                    AddCoord(counter);
                } 
            }

            if (counter.y <= endCoord.y)
            {
                while (counter.y + yDir <= endCoord.y)
                {
                    counter += new Vector2Int(0, yDir);
                    AddCoord(counter);
                }
            }
            else
            {
                while (counter.y - yDir >= endCoord.y)
                {
                    counter -= new Vector2Int(0, yDir);
                    AddCoord(counter);
                }
            }
            SpawnPlaceholders();
        }

        void AddCoord(Vector2Int coord)
        {
            if (!homeGrid.CanBuild(coord, biso.getCoordinates())) return;
            if (keyCoords.Count == Inventory.i.ItemInStockAmount(biso)) return;
            keyCoords.Add(coord);
        }

        void SpawnPlaceholders()
        {
            DestroyPlaceholders();
            foreach (Vector2Int coord in keyCoords)
            {
                placeholders.Add(homeGrid.BuildWithCoord(coord.x, coord.y, true));
            }
            UI_BuildingPointer.i.SetPrebuildUseAmount(keyCoords.Count);
        }

        public void DestroyPlaceholders()
        {
            for (int i = placeholders.Count - 1; i >= 0; i--)
            {
                Destroy(placeholders[i]);
            }
            placeholders.Clear();
        }

    }

    BuildDragInfo buildDragInfo = null;


    private void Start()
    {
        gridCellSize = (transform.Find("Bottom Left Corner").position - transform.Find("Second Bottom Left Corner").position).magnitude; //(secondBottomLeftCorner.position - bottomLeftCorner.position).magnitude;
        gridIndication = new GridIndication(transform.Find("Grid Indication"), new Vector3(gridCellSize, 0.05f, gridCellSize));
    }

    void Update()
    {
        if (!building) return; //return if not building mode

        
        if (!WorldUtility.TryMouseHitPoint(WorldUtility.LAYER.HOME_GRID, true)) //hide indicator if mouse is not on grid
        {
            gridIndication.Hide();
            return;
        }

        //set up homegrid, hitpoint, and display
        HomeGrid hg = WorldUtility.GetMouseHitObject(WorldUtility.LAYER.HOME_GRID, true).GetComponent<HomeGrid>();

        Vector3 hitPoint = WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.HOME_GRID, true);

        gridIndication.Display(hg.GetGridWorldPositionFromPosition(hitPoint), GetSelectedBuildingISO());

        hg.GetGridCoordFromPosition(hitPoint, out int x, out int z);

        if (buildDragInfo == null) //NEW BUILD DRAG
        {
            if (Input.GetMouseButtonDown(0))
            {
                buildDragInfo = new BuildDragInfo(new Vector2Int(x,z), hg, GetSelectedBuildingISO());
            }
        }
        else
        {
            buildDragInfo.SetEndPosition(new Vector2Int(x, z)); //RESET BUILD DRAG
            if (Input.GetMouseButtonUp(0)) //LIFT MOUSE AND BUILD
            {
                foreach (Vector2Int i in buildDragInfo.GetKeyCoords())
                {
                    print(i);
                    hg.BuildWithCoord(i.x, i.y);
                }
                buildDragInfo.DestroyPlaceholders();
                buildDragInfo = null;
                UI_BuildingPointer.i.SetPrebuildUseAmount(0);
            }
        }
    }

    public void OpenBuilding()
    {
        building = true;
        foreach(HomeGrid hg in allHomeGrids)
        {
            hg.ShowGridLines();
        }
    }

    public void CloseBuilding()
    {
        building = false;
        foreach (HomeGrid hg in allHomeGrids)
        {
            hg.HideGridLines();
        }
        CancelSelectedBuidling();
    }

    public void AddHomeGrid(HomeGrid hg)
    {
        allHomeGrids.Add(hg);
    }

    public BuildingISO GetSelectedBuildingISO()
    {
        if (selectedUIIB == null) return null;
        ItemScriptableObject returnISO = selectedUIIB.GetISO();
        return returnISO is BuildingISO ? (BuildingISO)returnISO :null;
    }

    public void SetSelectedBuilding(UI_InventoryBlock uiib)
    {
        if(selectedUIIB != null)selectedUIIB.SetSelectedBackground(false);
        selectedUIIB = uiib;
        UI_BuildingPointer.i.SetUp((BuildingISO)uiib.GetISO());
    }

    public void CancelSelectedBuidling()
    {
        if(selectedUIIB !=null)selectedUIIB.SetSelectedBackground(false);
        selectedUIIB = null;
        UI_BuildingPointer.i.TurnOff();
    }





    
}
