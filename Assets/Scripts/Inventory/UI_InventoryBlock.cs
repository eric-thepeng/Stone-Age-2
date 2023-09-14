using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Hypertonic.GridPlacement;

public class UI_InventoryBlock : MonoBehaviour
{
    Inventory.ItemInfo itemInfo;
    [ReadOnly] public int row;
    [ReadOnly] public int column;
    [ReadOnly] public int displayAmount;
    bool mouseOver = false;

    Color normalColr= Color.white;
    Color dragColr = Color.grey;

    [SerializeField] SpriteRenderer itemSprite;
    [SerializeField] TextMeshPro displayNumber;
    [SerializeField] SpriteRenderer numberBackground;

    private DragInventoryItem dii = null;

    public bool CheckIISO(ItemScriptableObject inISO)
    {
        return inISO == itemInfo.iso;
    }

    public ItemScriptableObject GetISO()
    {
        if (itemInfo == null) return null;
        return itemInfo.iso;
    }

    public int GetDisplayAmount()
    {
        return itemInfo.inStockAmount;
    }

    public void Initialize(int row, int col)
    {
        this.row = row;
        this.column = col;
    }

    public void SetUpDisplay(Inventory.ItemInfo ii)
    {
        itemInfo = ii;
        displayAmount = ii.inStockAmount;

        //display shit
        itemSprite.gameObject.SetActive(true);
        displayNumber.gameObject.SetActive(true);
        numberBackground.gameObject.SetActive(true);
        itemSprite.sprite = itemInfo.iso.iconSprite;
        UpdateDisplayAmount();

    }

    public void ClearDisplay()
    {
        itemInfo = null;
        displayAmount = 0;
        itemSprite.gameObject.SetActive(false);
        displayNumber.gameObject.SetActive(false);
        numberBackground.gameObject.SetActive(false);
    }

    public void UpdateDisplayAmount()
    {
        displayNumber.text = "" + GetDisplayAmount();
    }

    private void OnMouseEnter()
    {
        if (itemInfo == null) return;
        CraftingManager.i.mouseEnterInventoryBlock(this);
        //InventoryHoverInfo.i.Display(transform.position);
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        if (itemInfo == null) return;
        CraftingManager.i.mouseExitInventoryBlock();
        //InventoryHoverInfo.i.Disappear();
        mouseOver = false;
    }

    private void OnMouseDown()
    {
        if (itemInfo == null) return;
        
        // create tetris
        if (PlayerState.IsCrafting())
        {
            //InventoryHoverInfo.i.Disappear();
            CreateTetrisDrag();
        }
        
        // building item
        else if (PlayerState.IsBuilding())
        {
            if (!(itemInfo.iso is BuildingISO)) return;

            if ((BuildingISO)this.GetISO() == BuildingManager.i.GetSelectedBuildingISO())
            {
                BuildingManager.i.CancelSelectedBuidling();

                SetSelectedBackground(false);
                GridManagerAccessor.GridManager.CancelPlacement(false);

            } else
            {
                BuildingManager.i.SetSelectedBuilding(this);// (BuildingISO)itemInfo.iso);

                GridManagerAccessor.GridManager.CancelPlacement();

                Vector3 _position = GridManagerAccessor.GridManager.GetGridPosition();

                GameObject objectToPlace = Instantiate(((BuildingISO)itemInfo.iso).GetBuildingPrefab(), _position, new Quaternion());

                objectToPlace.name = ((BuildingISO)itemInfo.iso).GetBuildingPrefab().name;
                GridManagerAccessor.GridManager.CancelPlacement(false);
                GridManagerAccessor.GridManager.EnterPlacementMode(objectToPlace);
                //GridManagerAccessor.GridManager.StartPaintMode(((BuildingISO)itemInfo.iso).GetBuildingPrefab());

                SetSelectedBackground(true);
            }


        }
        
        // workshop item
        else if (PlayerState.IsAllocatingBackpack())
        {
            CreateIconDrag();
        }

        // 06/09/23 - new grid items
        //else if (GridManagerAccessor.GridManager.IsPlacingGridObject)
        //{
        //    if (itemInfo.iso is BuildingISO)
        //    {
        //        //BuildingManager.i.SetSelectedBuilding(this);// (BuildingISO)itemInfo.iso);
        //        //SetSelectedBackground(true);


        //        //GridManagerAccessor.GridManager.CancelPlacement();

        //        //GameObject objectToPlace = Instantiate(((BuildingISO)itemInfo.iso).GetBuildingPrefab(), GridManagerAccessor.GridManager.GetGridPosition(), new Quaternion());

        //        //objectToPlace.name = ((BuildingISO)itemInfo.iso).GetBuildingPrefab().name;
        //        //GridManagerAccessor.GridManager.EnterPlacementMode(objectToPlace);
        //        GridManagerAccessor.GridManager.CancelPlacement(false);
        //        GridManagerAccessor.GridManager.StartPaintMode(((BuildingISO)itemInfo.iso).GetBuildingPrefab());
                
        //    }

        //}

    }

    public void SetSelectedBackground(bool newState)
    {
        transform.Find("Selected Background").gameObject.SetActive(newState);
    }

    private void OnMouseUp()
    {
        //if ( mouseOver ) { InventoryHoverInfo.i.Display(transform.position); }
    }

    void CreateTetrisDrag()
    {
        //itemSprite.color = dragColr;
        if (CraftingManager.i.isPanelOpen()) //TETRIS
        {
            GameObject newTetris = CraftingManager.i.CreateTetris(itemInfo.iso, WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.UI_BACKGROUND, true), CraftingManager.CreateFrom.INVENTORY);
            newTetris.GetComponent<DragInventoryItem>().SetUp(this);
        }
    }

    void CreateIconDrag()
    {
        DraggingISOIconManager.i.CreateDraggingISOIcon(itemInfo.iso);
    }

    public void PlaceDrag()
    {
        itemSprite.color = normalColr;
    }

}
