using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hypertonic.GridPlacement;
using Hypertonic.GridPlacement.Example.AddProgramatically.Models;
using Hypertonic.GridPlacement.Models;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;
using static Inventory;

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

    public bool editing;
    public bool deleting;
    public bool HandItemRemoveable;

    public GameObject gridOperationManager;
    public GameObject particlePrefab;

    float gridCellSize;

    [SerializeField]
    private Transform initialTransform;


    [SerializeField]
    private Transform editingIndicator;

    [SerializeField]
    private Transform deletingIndicator;

    [SerializeField]
    private Transform removeHandItemIndicator;

    [SerializeField]
    private Transform editingButton;

    [SerializeField]
    private Transform deletingButton;

    [SerializeField]
    private Transform removeHandItemButton;


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

            for (int i = displayingBlocks.Count-1; i>=0; i--)
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
            //indicationHolder.gameObject.SetActive(true);
            //indicationTemplate.gameObject.SetActive(true);

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


        editingButton.gameObject.SetActive(false);
        deletingButton.gameObject.SetActive(false);
        removeHandItemButton.gameObject.SetActive(false);
        editingIndicator.gameObject.SetActive(false);
        deletingIndicator.gameObject.SetActive(false);
        removeHandItemIndicator.gameObject.SetActive(false);
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

        //gridIndication.Display(hg.GetGridWorldPositionFromPosition(hitPoint), GetSelectedBuildingISO());

        hg.GetGridCoordFromPosition(hitPoint, out int x, out int z);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Input.GetMouseButtonDown(0))
        {
            bool _rayHit = Physics.Raycast(ray, out hitInfo);
            if (GetSelectedBuildingISO() != null && Inventory.i.ItemInStockAmount(GetSelectedBuildingISO()) > 0)
            {
                //GridManagerAccessor.GridManager.CancelPlacement();
                GridManagerAccessor.GridManager.EndPaintMode(false);

                //Vector3 _position = GridManagerAccessor.GridManager.GetGridPosition();
                //_position.y -= 10;

                //GameObject objectToPlace = Instantiate(GetSelectedBuildingISO().GetBuildingPrefab(), _position, new Quaternion());

                //objectToPlace.name = GetSelectedBuildingISO().GetBuildingPrefab().name;
                GridManagerAccessor.GridManager.StartPaintMode(GetSelectedBuildingISO().GetBuildingPrefab());
            } else if (editing)
            {

                // 如果射线与Collider相交，则返回true并将碰撞信息存储在hitInfo中
                if (_rayHit)
                {
                    EditingProcessHitItem(hitInfo);
                }
            } else if (deleting)
            {
                if (_rayHit)
                {
                    DeletingProcessHitItem(hitInfo);
                }
            }
        }
        //if (buildDragInfo == null) //NEW BUILD DRAG
        //{

        if (Input.GetMouseButton(0))
        {
            if (GetSelectedBuildingISO() != null && !WorldUtility.TryMouseHitPoint(WorldUtility.LAYER.UI_BACKGROUND, true) && WorldUtility.GetMouseHitObject(WorldUtility.LAYER.HOME_GRID, true))
            {
                BuildingISO selectedISO = GetSelectedBuildingISO();
                if (Inventory.i.ItemInStockAmount(selectedISO) > 0)
                {
                    bool _confirm = GridManagerAccessor.GridManager.ConfirmPlacement();
                    if (_confirm)
                    {
                        Instantiate(particlePrefab, hitPoint, new Quaternion());
                        Inventory.i.InBuildItem(selectedISO, true);
                        
                        //print(Inventory.i.ItemInStockAmount(GetSelectedBuildingISO()));
                    }
                }

                if (selectedISO != null && Inventory.i.ItemInStockAmount(selectedISO) > 0)
                {

                    //GridManagerAccessor.GridManager.CancelPlacement();
                    GridManagerAccessor.GridManager.EndPaintMode(false);

                    //Vector3 _position = new Vector3(0, -10, 0);
                    //GameObject objectToPlace = Instantiate(GetSelectedBuildingISO().GetBuildingPrefab(), _position, new Quaternion());

                    //objectToPlace.name = GetSelectedBuildingISO().GetBuildingPrefab().name;
                    //GridManagerAccessor.GridManager.EnterPlacementMode(objectToPlace);

                    GridManagerAccessor.GridManager.StartPaintMode(selectedISO.GetBuildingPrefab());
                }
                else
                {
                    //GridManagerAccessor.GridManager.CancelPlacement(false);
                    GridManagerAccessor.GridManager.EndPaintMode(false);
                    i.CancelSelectedBuidling();
                    Debug.Log("CancelSelectedBuidling");

                    gridOperationManager.GetComponent<GridOperationManager>().StartPaintMode();

                }

                //buildDragInfo = new BuildDragInfo(new Vector2Int(x,z), hg, GetSelectedBuildingISO());

            }
        }
        //}
        //else
        //{
        //    buildDragInfo.SetEndPosition(new Vector2Int(x, z)); //RESET BUILD DRAG
        //    if (Input.GetMouseButtonUp(0)) //LIFT MOUSE AND BUILD
        //    {
        //        _ = PlaceObjectsToGrid(buildDragInfo);

        //        //foreach (Vector2Int i in buildDragInfo.GetKeyCoords())
        //        //{
        //        //    //print(i);
        //            //hg.BuildWithCoord(i.x, i.y);

        //        //}

        //        buildDragInfo.DestroyPlaceholders();
        //        buildDragInfo = null;
        //        UI_BuildingPointer.i.SetPrebuildUseAmount(0);
        //    }
        //}
    }


    public void OpenBuilding()
    {
        building = true;
        editingButton.gameObject.SetActive(true);
        deletingButton.gameObject.SetActive(true);
        removeHandItemButton.gameObject.SetActive(true);
        editingIndicator.gameObject.SetActive(false);
        deletingIndicator.gameObject.SetActive(false);
        removeHandItemIndicator.gameObject.SetActive(false);

        foreach (HomeGrid hg in allHomeGrids)
        {
            hg.ShowGridLines();
        }
    }

    public void CloseBuilding()
    {
        building = false;
        editingButton.gameObject.SetActive(false);
        deletingButton.gameObject.SetActive(false);
        removeHandItemButton.gameObject.SetActive(false);

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
        if (selectedUIIB == null || selectedUIIB.GetISO() == null) return null;
        ItemScriptableObject returnISO = selectedUIIB.GetISO();
        return returnISO is BuildingISO ? (BuildingISO)returnISO :null;
    }

    public void SetSelectedBuilding(UI_InventoryBlock uiib)
    {
        if(selectedUIIB != null)selectedUIIB.SetSelectedBackground(false);
        selectedUIIB = uiib;
        Debug.Log(uiib);
        UI_BuildingPointer.i.SetUp((BuildingISO)uiib.GetISO());
        Debug.Log((BuildingISO)uiib.GetISO());
    }

    public void CancelSelectedBuidling()
    {
        if(selectedUIIB !=null)selectedUIIB.SetSelectedBackground(false);
        selectedUIIB = null;
        UI_BuildingPointer.i.TurnOff();
    }

    public void ToggleEditing()
    {
        if (!GridManagerAccessor.GridManager.IsPlacingGridObject)
        {
            gridOperationManager.GetComponent<GridOperationManager>().StartPaintMode();
            Debug.LogWarning("Grid Manager is not in Paint Mode! Please figure it out to avoid future problems.");
        }

        if (editing && GridManagerAccessor.GridManager.ObjectToPlace.GetComponent<GridObjectTags>().containsTag("EmptyObject"))
        {
            editing = false;
            editingIndicator.gameObject.SetActive(false);
        } else if (!editing)
        {
            CancelSelectedBuidling();
            GridManagerAccessor.GridManager.EndPaintMode(false);
            editing = true;
            editingIndicator.gameObject.SetActive(true);
            if (deleting)
            {
                ToggleDeleting();
            }

            gridOperationManager.GetComponent<GridOperationManager>().StartPaintMode();
        }
    }

    public void EditingProcessHitItem(RaycastHit hitInfo)
    {

        // 如果碰撞到的是自己所属的Collider，做出响应
        if (hitInfo.collider.GetComponent<GridObjectTags>() != null)
        {

            if (GridManagerAccessor.GridManager.IsPlacingGridObject)
            {
                if (!GridManagerAccessor.GridManager.ObjectToPlace.GetComponent<GridObjectTags>().containsTag("EmptyObject"))
                {
                    if (GridManagerAccessor.GridManager.ConfirmPlacement())
                    {
                        GridManagerAccessor.GridManager.EndPaintMode(false);

                        gridOperationManager.GetComponent<GridOperationManager>().StartPaintMode();
                        //Instantiate(particlePrefab, hitPoint, new Quaternion());
                        //editing = false;
                        //GridManagerAccessor.GridManager.emptycube
                    }

                }
                else
                {
                    //GridManagerAccessor.GridManager.CancelPlacement(false);
                    GridManagerAccessor.GridManager.EndPaintMode(false);

                    GridManagerAccessor.GridManager.ModifyPlacementOfGridObject(hitInfo.collider.gameObject);
                }

                CheckHandItemRemoveable();

            }
            else
            {

                Debug.LogError("BuildingManager tries to process editing, but it is not currently in placing mode!");
                //GridManagerAccessor.GridManager.ModifyPlacementOfGridObject(hitInfo.collider.gameObject);
            }

        }
    }

    public void ToggleDeleting()
    {
        if (deleting && GridManagerAccessor.GridManager.ObjectToPlace.GetComponent<GridObjectTags>().containsTag("EmptyObject"))
        {
            deleting = false;
            deletingIndicator.gameObject.SetActive(false);
        }
        else if (!deleting)
        {
            CancelSelectedBuidling();
            GridManagerAccessor.GridManager.EndPaintMode(false);
            gridOperationManager.GetComponent<GridOperationManager>().StartPaintMode();

            if (editing)
            {
                ToggleEditing();
            }

            deleting = true;
            deletingIndicator.gameObject.SetActive(true);

        }
    }

    public void CheckHandItemRemoveable()
    {
        if (GridManagerAccessor.GridManager.IsPlacingGridObject &&
            GridManagerAccessor.GridManager.ObjectToPlace.GetComponent<GridObjectTags>().containsTag("EmptyObject"))
        {
            HandItemRemoveable = false;
            removeHandItemIndicator.gameObject.SetActive(false);
        } else
        {
            HandItemRemoveable = true;
            removeHandItemIndicator.gameObject.SetActive(true);
        }
    }

    public void DeleteHandItem()
    {
        GameObject handleItem = GridManagerAccessor.GridManager.ObjectToPlace;
        if (HandItemRemoveable)
        {
            if (!handleItem.GetComponent<GridObjectTags>().containsTag("EmptyObject"))
            {
                GridManagerAccessor.GridManager.DeleteObject(handleItem);
                GridManagerAccessor.GridManager.EndPaintMode(false);

                //GridManagerAccessor.GridManager.ModifyPlacementOfGridObject(hitInfo.collider.gameObject);
                Inventory.i.AddInventoryItem(handleItem.GetComponent<GridObjectTags>().GetBuildingISO());

                gridOperationManager.GetComponent<GridOperationManager>().StartPaintMode();
                CheckHandItemRemoveable();
            }
            else
            {
                Debug.LogError("Wrong writeup, causes DeleteHandItem to be triggered invalid");
            }

        }
    }

    public void DeletingProcessHitItem(RaycastHit hitInfo)
    {

        // 如果碰撞到的是自己所属的Collider，做出响应
        if (hitInfo.collider.GetComponent<GridObjectTags>() != null)
        {

            if (GridManagerAccessor.GridManager.IsPlacingGridObject)
            {
                if (!GridManagerAccessor.GridManager.ObjectToPlace.GetComponent<GridObjectTags>().containsTag("EmptyObject"))
                {
                    Debug.LogError("BuildingManager tries to process deleting, but an object is handling in the hand!");

                }
                else
                {
                    GridManagerAccessor.GridManager.DeleteObject(hitInfo.collider.gameObject);
                    GridManagerAccessor.GridManager.EndPaintMode(false);

                    //GridManagerAccessor.GridManager.ModifyPlacementOfGridObject(hitInfo.collider.gameObject);
                    Inventory.i.AddInventoryItem(hitInfo.collider.gameObject.GetComponent<GridObjectTags>().GetBuildingISO());

                    gridOperationManager.GetComponent<GridOperationManager>().StartPaintMode();
                }


            }

            else
            {
                Debug.LogError("BuildingManager tries to process deleting, but it is not currently in placing mode!");
                //GridManagerAccessor.GridManager.ModifyPlacementOfGridObject(hitInfo.collider.gameObject);
            }

        }
    }


}
