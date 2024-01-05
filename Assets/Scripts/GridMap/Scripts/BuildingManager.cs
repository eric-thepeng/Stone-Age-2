using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Hypertonic.GridPlacement;
using Hypertonic.GridPlacement.Example.AddProgramatically.Models;
using Hypertonic.GridPlacement.Models;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static Cinemachine.CinemachineTransposer;
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

    public bool buildingMode = false;
    public GameObject placeholdingBuilding;

    private bool editing;
    //public bool deleting;
    private bool HandItemRemoveable;

    private bool modifying = false;

    public GridOperationManager gridOperationManager;
    public GameObject particlePrefab;

    Vector3 hitPoint;
    float gridCellSize;

    [SerializeField]
    private Transform initialTransform;

    public float duration = 0.1f;


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

    //class BuildDragInfo
    //{
    //    HomeGrid homeGrid;
    //    Vector2Int startCoord;
    //    Vector2Int endCoord;
    //    List<Vector2Int> keyCoords;
    //    List<GameObject> placeholders;
    //    BuildingISO biso;

    //    public BuildDragInfo(Vector2Int firstSpot, HomeGrid hg, BuildingISO inputBISO)
    //    {
    //        homeGrid = hg;
    //        startCoord = firstSpot;
    //        endCoord = startCoord;
    //        keyCoords = new List<Vector2Int>();
    //        placeholders = new List<GameObject>();
    //        biso = inputBISO;
    //        CalculateAllCoords();
    //    }

    //    public void SetEndPosition(Vector2Int newEndCoord)
    //    {
    //        if (endCoord == newEndCoord) return;
    //        endCoord = newEndCoord;
    //        CalculateAllCoords();
    //    }

    //    public List<Vector2Int> GetKeyCoords()
    //    {
    //        return keyCoords;
    //    }

    //    void CalculateAllCoords()
    //    {
    //        if (biso == null) return;
    //        keyCoords.Clear();

    //        int xDir = biso.GetWidth();
    //        int yDir = biso.GetHeight();

    //        Vector2Int counter = new Vector2Int(startCoord.x, startCoord.y);

    //        if(counter.x <= endCoord.x)
    //        {
    //            AddCoord(counter);

    //            while (counter.x + xDir <= endCoord.x)
    //            {
    //                counter += new Vector2Int(xDir, 0);
    //                AddCoord(counter);
    //            }
    //        }
    //        else
    //        {
    //            AddCoord(counter);

    //            while (counter.x - xDir >= endCoord.x)
    //            {
    //                counter -= new Vector2Int(xDir, 0);
    //                AddCoord(counter);
    //            } 
    //        }

    //        if (counter.y <= endCoord.y)
    //        {
    //            while (counter.y + yDir <= endCoord.y)
    //            {
    //                counter += new Vector2Int(0, yDir);
    //                AddCoord(counter);
    //            }
    //        }
    //        else
    //        {
    //            while (counter.y - yDir >= endCoord.y)
    //            {
    //                counter -= new Vector2Int(0, yDir);
    //                AddCoord(counter);
    //            }
    //        }
    //        SpawnPlaceholders();
    //    }

    //    void AddCoord(Vector2Int coord)
    //    {
    //        if (!homeGrid.CanBuild(coord, biso.getCoordinates())) return;
    //        if (keyCoords.Count == Inventory.i.ItemInStockAmount(biso)) return;
    //        keyCoords.Add(coord);
    //    }

    //    void SpawnPlaceholders()
    //    {
    //        DestroyPlaceholders();
    //        foreach (Vector2Int coord in keyCoords)
    //        {
    //            placeholders.Add(homeGrid.BuildWithCoord(coord.x, coord.y, true));
    //        }
    //        UI_BuildingPointer.i.SetPrebuildUseAmount(keyCoords.Count);
    //    }

    //    public void DestroyPlaceholders()
    //    {
    //        for (int i = placeholders.Count - 1; i >= 0; i--)
    //        {
    //            Destroy(placeholders[i]);
    //        }
    //        placeholders.Clear();
    //    }

    //}

    //BuildDragInfo buildDragInfo = null;
    // private Quaternion targetRotation;
    
    // Vector3 originalScale;
    Quaternion orginalQuaternion;
    private int rotationAngle;
    private void Start()
    {
        gridCellSize = (transform.Find("Bottom Left Corner").position - transform.Find("Second Bottom Left Corner").position).magnitude; //(secondBottomLeftCorner.position - bottomLeftCorner.position).magnitude;
        gridIndication = new GridIndication(transform.Find("Grid Indication"), new Vector3(gridCellSize, 0.05f, gridCellSize));

        gridOperationManager = FindObjectOfType<GridOperationManager>(); 
        
    }


    public bool mouseInPlacementMode = false;

    private bool switchedPlacementMode = false;
    private bool currentPlacementMode = false;

    public bool SwitchedPlacementMode { get => switchedPlacementMode; set => switchedPlacementMode = value; }
    public bool CurrentPlacementMode { get => currentPlacementMode; set => currentPlacementMode = value; }

    void Update()
    {
        if (switchedPlacementMode)
        {
            switchedPlacementMode = false;
        } else
        {
            if (currentPlacementMode != GridManagerAccessor.GridManager.IsPlacingGridObject)
            {
                currentPlacementMode = GridManagerAccessor.GridManager.IsPlacingGridObject;
                switchedPlacementMode = true;
            }
        }

        if (!buildingMode) return; //return if not building mode

        if (!WorldUtility.TryMouseHitPoint(WorldUtility.LAYER.HOME_GRID, true)) //hide indicator if mouse is not on grid
        {
            gridIndication.Hide();
            return;
        }

        HomeGrid hg = WorldUtility.GetMouseHitObject(WorldUtility.LAYER.HOME_GRID, true).GetComponent<HomeGrid>();

        hitPoint = WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.HOME_GRID, true);


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;


        if (Input.GetMouseButtonDown(0))
        {
            bool _rayHit = Physics.Raycast(ray, out hitInfo);
            if (modifying)
            {

                if (_rayHit)
                {
                    EditingProcessHitItem(hitInfo);
                }
            }
            //else if (GetSelectedBuildingISO() != null && Inventory.i.ItemInStockAmount(GetSelectedBuildingISO()) > 0)
            //{
            //    GridManagerAccessor.GridManager.EndPaintMode(false);
            //    GridManagerAccessor.GridManager.StartPaintMode(GetSelectedBuildingISO().GetBuildingPrefab());
            //}
        }

        if (Input.GetMouseButtonUp(0))
        {
            mouseInPlacementMode = true;
        }

        GameObject _selectedGridObject = GridManagerAccessor.GridManager.ObjectToPlace;
        // originalScale = _selectedGridObject.transform.localScale;
        // orginalQuaternion = _selectedGridObject.transform.rotation;
        
        if (Input.GetKeyDown(KeyCode.Q))
        {if (GridManagerAccessor.GridManager.IsPlacingGridObject)
            {
                // targetRotation = _selectedGridObject.transform.rotation * Quaternion.Euler(0, 90, 0);
                if (rotationAngle == 0)
                {
                    // originalScale = _selectedGridObject.transform.localScale;
                    orginalQuaternion = _selectedGridObject.transform.rotation;
                }

                rotationAngle -= 90;
                // 使用DOVirtual.Float根据动画曲线缩放物体
                DOVirtual.Float(0f, 1f, GridManagerAccessor.GridManager.GridSettings.animationDuration, (float value) =>
                {
                    // float scaleValue = GridManagerAccessor.GridManager.GridSettings.animationCurve.Evaluate(value);
                    // Vector3 newScale = _selectedGridObject.transform.localScale;
                    // newScale.x = originalScale.x * scaleValue;
                    // newScale.y = originalScale.y * scaleValue;
                    // newScale.z = originalScale.z * scaleValue;
                    _selectedGridObject.transform.rotation =  Quaternion.Euler(0, orginalQuaternion.eulerAngles.y + value * rotationAngle, 0);
                    // _selectedGridObject.transform.localScale = newScale;
                }).OnComplete(() =>
                {
                    // 动画完成后恢复原始尺寸
                    // _selectedGridObject.transform.localScale = originalScale;
                    _selectedGridObject.transform.rotation = Quaternion.Euler(0, orginalQuaternion.eulerAngles.y + rotationAngle, 0);
                    GridManagerAccessor.GridManager.HandleGridObjectRotated();
                    rotationAngle = 0;
                    // originalScale = _selectedGridObject.transform.localScale;
                    orginalQuaternion = _selectedGridObject.transform.rotation;
                });
                
                // StartCoroutine(SmoothRotateObject(_selectedGridObject, targetRotation));
            }
        }
        else
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (GridManagerAccessor.GridManager.IsPlacingGridObject)
            {
                // targetRotation = _selectedGridObject.transform.rotation * Quaternion.Euler(0, 90, 0);
                if (rotationAngle == 0)
                {
                    // originalScale = _selectedGridObject.transform.localScale;
                    orginalQuaternion = _selectedGridObject.transform.rotation;
                }

                rotationAngle += 90;
                // 使用DOVirtual.Float根据动画曲线缩放物体
                DOVirtual.Float(0f, 1f, GridManagerAccessor.GridManager.GridSettings.animationDuration, (float value) =>
                {
                    // float scaleValue = GridManagerAccessor.GridManager.GridSettings.animationCurve.Evaluate(value);
                    // Vector3 newScale = _selectedGridObject.transform.localScale;
                    // newScale.x = originalScale.x * scaleValue;
                    // newScale.y = originalScale.y * scaleValue;
                    // newScale.z = originalScale.z * scaleValue;
                    _selectedGridObject.transform.rotation =  Quaternion.Euler(0, orginalQuaternion.eulerAngles.y + value * rotationAngle, 0);
                    // _selectedGridObject.transform.localScale = newScale;
                }).OnComplete(() =>
                {
                    // 动画完成后恢复原始尺寸
                    // _selectedGridObject.transform.localScale = originalScale;
                    _selectedGridObject.transform.rotation = Quaternion.Euler(0, orginalQuaternion.eulerAngles.y + rotationAngle, 0);
                    GridManagerAccessor.GridManager.HandleGridObjectRotated();
                    rotationAngle = 0;
                    // originalScale = _selectedGridObject.transform.localScale;
                    orginalQuaternion = _selectedGridObject.transform.rotation;
                });
                
                // StartCoroutine(SmoothRotateObject(_selectedGridObject, targetRotation));
            }
        }


        //if (buildDragInfo == null) //NEW BUILD DRAG
        //{



        if (Input.GetMouseButton(0))
        {
            //Debug.Log(mouseInPlacementMode);
            if (mouseInPlacementMode && GetSelectedBuildingISO() != null && !WorldUtility.TryMouseHitPoint(WorldUtility.LAYER.UI_BACKGROUND, true) && WorldUtility.GetMouseHitObject(WorldUtility.LAYER.HOME_GRID, true))
            {
                BuildingISO selectedISO = GetSelectedBuildingISO();
                if (Inventory.i.ItemInStockAmount(selectedISO) > 0)
                {
                    PlaceableObject placeableObject = GridManagerAccessor.GridManager.ObjectToPlace.GetComponent<PlaceableObject>();

                    Quaternion _rotation = GridManagerAccessor.GridManager.ObjectToPlace.transform.rotation;

                    bool _confirm = GridManagerAccessor.GridManager.ConfirmPlacement();
                    if (_confirm)
                    {
                        placeableObject.EnableEffects();
                        ObjectMorphing(placeableObject.transform, GridManagerAccessor.GridManager.GridSettings.animationCurve,
                            GridManagerAccessor.GridManager.GridSettings.animationDuration);
                        Instantiate(particlePrefab, hitPoint, new Quaternion());
                        
                        placeableObject.GetComponent<GridValidator>().enabled = false;
                        placeableObject.GetComponent<NavMeshObstacle>().enabled = true;

                        Inventory.i.InBuildItem(selectedISO, true);
                        GridManagerAccessor.GridManager.ObjectToPlace.transform.rotation = _rotation;

                        GridManagerAccessor.GridManager.HandleGridObjectRotated();
                    }
                }

                if (selectedISO != null && Inventory.i.ItemInStockAmount(selectedISO) > 0)
                {
                    //GridManagerAccessor.GridManager.EndPaintMode(false);

                    //GridManagerAccessor.GridManager.StartPaintMode(selectedISO.GetBuildingPrefab());
                }
                else
                {
                    GridManagerAccessor.GridManager.EndPaintMode(true);
                    i.CancelSelectedBuidling();
                    Debug.Log("CancelSelectedBuidling");

                    CloseBuildingMode();
                }

            }
        }
        else if (Input.GetMouseButtonDown(1))
        {

            bool _rayHit = Physics.Raycast(ray, out hitInfo);
            if (!WorldUtility.TryMouseHitPoint(WorldUtility.LAYER.UI_BACKGROUND, true) && WorldUtility.GetMouseHitObject(WorldUtility.LAYER.HOME_GRID, true))
            {
                if (modifying)
                {
                    if (_rayHit)
                    {
                        if (GridManagerAccessor.GridManager.ObjectToPlace.GetComponent<PlaceableObject>().containsTag("EmptyObject"))
                        {
                            bool _hit = DeletingProcessHitItem(hitInfo);
                            Debug.Log(_hit);
                            if (!_hit) ToggleModifying();
                        }
                        else
                        {
                            DeleteHandItem();
                        }
                    }
                }
                else
                {
                    GridManagerAccessor.GridManager.EndPaintMode(true);
                    buildingMode = false;
                    if (GetSelectedBuildingISO() != null)
                    {
                        i.CancelSelectedBuidling();
                        Debug.Log("CancelSelectedBuidling");
                    }

                    CloseBuildingMode();
                    gridOperationManager.EndPaintMode();
                }

            }
        }


    }

    IEnumerator SmoothRotateObject(GameObject obj, Quaternion targetRot)
    {
        float elapsed = 0f;
        Quaternion startRotation = obj.transform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            obj.transform.rotation = Quaternion.Slerp(startRotation, targetRot, elapsed / duration);
            yield return null;
        }
        obj.transform.rotation = targetRot;  // 确保旋转完全完成
        GridManagerAccessor.GridManager.HandleGridObjectRotated();
    }

    public void ObjectMorphing(Transform objectTransform, AnimationCurve animationCurve, float animationDuration)
    {
        Vector3 originalScale = objectTransform.localScale;
        // 使用DOVirtual.Float根据动画曲线缩放物体
        DOVirtual.Float(0f, 1f, animationDuration, (float value) =>
        {
            float scaleValue = animationCurve.Evaluate(value);
            Vector3 newScale = objectTransform.localScale;
            newScale.x = originalScale.x * scaleValue;
            newScale.y = originalScale.y * scaleValue;
            newScale.z = originalScale.z * scaleValue;
            objectTransform.localScale = newScale;
        }).OnComplete(() =>
        {
            // 动画完成后恢复原始尺寸
            objectTransform.localScale = originalScale;
        });
    }

    public void OpenBuildingMode()
    {
        buildingMode = true;
        //PlayerState.EnterState(PlayerState.State.Building);
        //PlayerState.ChangeInventoryPanel(false);
        PlayerState.state = PlayerState.State.Building;

        foreach (HomeGrid hg in allHomeGrids)
        {
            hg.ShowGridLines();
        }
    }

    public void CloseBuildingMode()
    {
        buildingMode = false;
        //PlayerState.ChangeInventoryPanel(true);
        PlayerState.state = PlayerState.State.Browsing;

        foreach (HomeGrid hg in allHomeGrids)
        {
            hg.HideGridLines();
        }

        //foreach (Obstacle item in FindObjectsOfType<Obstacle>())
        //{
        //    foreach (BoxCollider collider in item.GetComponents<BoxCollider>())
        //    {
        //        collider.enabled = false;
        //    }
        //}

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
        //Debug.Log((BuildingISO)uiib.GetISO());
    }

    public void CancelSelectedBuidling()
    {
        if(selectedUIIB !=null)selectedUIIB.SetSelectedBackground(false);
        selectedUIIB = null;
        UI_BuildingPointer.i.TurnOff();
    }


    public void StartEditingChildMode()
    {

        CancelSelectedBuidling();
        GridManagerAccessor.GridManager.EndPaintMode(true);
        editing = true;
        //editingIndicator.gameObject.SetActive(true);
        //if (deleting)
        //{
        //    ToggleDeleting();
        //}

        gridOperationManager.StartPaintMode();
    }


    public bool previousInventoryPanelOpen = false;

    public void ToggleModifying()
    {
        if (modifying)
        {
            CloseModifyMode();
        }
        else if (!modifying)
        {
            previousInventoryPanelOpen = PlayerState.isInventoryPanelOpen();
            buildingMode = true;

            CancelSelectedBuidling();
            GridManagerAccessor.GridManager.EndPaintMode(true);
            modifying = true;
            gridOperationManager.StartPaintMode();
            //PlayerState.ExitState();
            PlayerState.state = PlayerState.State.Building;

            // edited close inventory panel
            //PlayerState.ChangeInventoryPanel(false);

            StartEditingChildMode();
        }
    }

    public void EditingProcessHitItem(RaycastHit hitInfo)
    {

        if (hitInfo.collider.GetComponent<PlaceableObject>() != null)
        {

            if (GridManagerAccessor.GridManager.IsPlacingGridObject)
            {
                PlaceableObject placeableObject = GridManagerAccessor.GridManager.ObjectToPlace.GetComponent<PlaceableObject>();
                if (!placeableObject.containsTag("EmptyObject"))
                {
                    if (GridManagerAccessor.GridManager.ConfirmPlacement())
                    {

                        placeableObject.EnableEffects();
                        ObjectMorphing(placeableObject.transform, GridManagerAccessor.GridManager.GridSettings.animationCurve,
                            GridManagerAccessor.GridManager.GridSettings.animationDuration);
                        
                        Instantiate(particlePrefab, hitPoint, new Quaternion());
                        //GridUtilities.GetCellIndexesRequiredForObject

                        placeableObject.GetComponent<GridValidator>().enabled = false;
                        placeableObject.GetComponent<NavMeshObstacle>().enabled = true;
                        GridManagerAccessor.GridManager.EndPaintMode(true);

                        gridOperationManager.StartPaintMode();
                    }

                }
                else
                {
                    //GridManagerAccessor.GridManager.CancelPlacement(false);
                    GridManagerAccessor.GridManager.EndPaintMode(true);

                    Instantiate(particlePrefab, hitPoint, new Quaternion());
                    GridManagerAccessor.GridManager.ModifyPlacementOfGridObject(hitInfo.collider.gameObject);
                    GridValidator placingObject = hitInfo.collider.gameObject.GetComponent<GridValidator>();
                    placingObject.enabled = true;
                    placingObject.FindTilemap();
                    placeableObject.GetComponent<NavMeshObstacle>().enabled = false;
                    hitInfo.collider.gameObject.GetComponent<PlaceableObject>().DisableEffects();
                }

                //CheckHandItemRemoveable();

            }
            else
            {

                Debug.LogError("BuildingManager tries to process editing, but it is not currently in placing mode!");
                //GridManagerAccessor.GridManager.ModifyPlacementOfGridObject(hitInfo.collider.gameObject);
            }

        }
    }


    public bool DeleteHandItem()
    {
        if (GridManagerAccessor.GridManager.IsPlacingGridObject &&
            GridManagerAccessor.GridManager.ObjectToPlace.GetComponent<PlaceableObject>().containsTag("EmptyObject"))
        {
            HandItemRemoveable = false;
        }
        else
        {
            HandItemRemoveable = true;
        }

        GameObject handleItem = GridManagerAccessor.GridManager.ObjectToPlace;
        if (HandItemRemoveable)
        {
            if (!handleItem.GetComponent<PlaceableObject>().containsTag("EmptyObject"))
            {
                Instantiate(particlePrefab, hitPoint, new Quaternion());
                GridManagerAccessor.GridManager.DeleteObject(handleItem);
                GridManagerAccessor.GridManager.EndPaintMode(true);

                //GridManagerAccessor.GridManager.ModifyPlacementOfGridObject(hitInfo.collider.gameObject);
                Inventory.i.AddInventoryItem(handleItem.GetComponent<PlaceableObject>().GetBuildingISO());

                gridOperationManager.StartPaintMode();
                //CheckHandItemRemoveable();
                return true;
            }
            else
            {
                Debug.LogError("Wrong writeup, causes DeleteHandItem to be triggered invalid");
                return false;
            }
        } else
        {
            return false;
        }
    }

    public bool DeletingProcessHitItem(RaycastHit hitInfo)
    {
        if (hitInfo.collider.GetComponent<PlaceableObject>() != null)
        {

            if (GridManagerAccessor.GridManager.IsPlacingGridObject)
            {
                if (!GridManagerAccessor.GridManager.ObjectToPlace.GetComponent<PlaceableObject>().containsTag("EmptyObject"))
                {
                    Debug.LogError("BuildingManager tries to process deleting, but an object is handling in the hand!");

                }
                else
                {
                    Instantiate(particlePrefab, hitPoint, new Quaternion());
                    GridManagerAccessor.GridManager.DeleteObject(hitInfo.collider.gameObject);
                    GridManagerAccessor.GridManager.EndPaintMode(true);

                    //GridManagerAccessor.GridManager.ModifyPlacementOfGridObject(hitInfo.collider.gameObject);
                    Inventory.i.AddInventoryItem(hitInfo.collider.gameObject.GetComponent<PlaceableObject>().GetBuildingISO());

                    gridOperationManager.StartPaintMode();
                    return true;
                }


            }

            else
            {
                Debug.LogError("BuildingManager tries to process deleting, but it is not currently in placing mode!");
                //GridManagerAccessor.GridManager.ModifyPlacementOfGridObject(hitInfo.collider.gameObject);
            }

        }
        return false;
    }

    public void CloseModifyMode()
    {
        if (modifying && !GridManagerAccessor.GridManager.ObjectToPlace.GetComponent<PlaceableObject>().containsTag("EmptyObject") && !GridManagerAccessor.GridManager.ConfirmPlacement())
        {
            GameObject handleItem = GridManagerAccessor.GridManager.ObjectToPlace;
            Inventory.i.AddInventoryItem(handleItem.GetComponent<PlaceableObject>().GetBuildingISO());
        }
        GridManagerAccessor.GridManager.EndPaintMode(true);
        modifying = false;
        editing = false;

        buildingMode = false;

        PlayerState.state = PlayerState.State.Browsing;
        PlayerState.ChangeInventoryPanel(previousInventoryPanelOpen);
    }

}
