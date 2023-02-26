using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public bool CheckIISO(InventoryItemSO inIISO)
    {
        return inIISO == itemInfo.iiso;
    }

    public void Initialize(int row, int col)
    {
        this.row = row;
        this.column = col;
    }

    public void SetUpDisplay(Inventory.ItemInfo ii)
    {
        itemInfo = ii;
        displayAmount = ii.displayAmount;

        //display shit
        itemSprite.gameObject.SetActive(true);
        displayNumber.gameObject.SetActive(true);
        numberBackground.gameObject.SetActive(true);
        itemSprite.sprite = itemInfo.iiso.inventoryIcon;
        displayNumber.text = "" + ii.displayAmount;

    }

    public void ClearDisplay()
    {
        itemInfo = null;
        displayAmount = 0;
        itemSprite.gameObject.SetActive(false);
        displayNumber.gameObject.SetActive(false);
        numberBackground.gameObject.SetActive(false);
    }

    public void UpdateAmount()
    {
        displayNumber.text = "" + displayAmount;
    }

    private void OnMouseEnter()
    {
        //InventoryHoverInfo.i.Display(transform.position);
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        //InventoryHoverInfo.i.Disappear();
        //mouseOver = false;
    }

    private void OnMouseDown()
    {
        print("mouse down");
        //InventoryHoverInfo.i.Disappear();
        CreateDrag();
    }

    private void OnMouseUp()
    {
        //if ( mouseOver ) { InventoryHoverInfo.i.Display(transform.position); }
    }

    void CreateDrag()
    {
        itemSprite.color = dragColr;
        if (CraftingManager.i.isPanelOpen())
        {
            print("do this");
            //dii = Instantiate(itemInfo.iiso.iso.myPrefab, CraftingManager.).GetComponent<DragInventoryItem>();
            //dii.SetUp(this);
        }
        //InventoryDrag ID = UI_Inventory.i.NewDragging(itemInfo.iiso);
       // ID.SetUpDragUI(itemInfo.iiso, this);
    }

    public void PlaceDrag()
    {
        GetComponent<SpriteRenderer>().color = normalColr;
    }

    public void CancelDrag()
    {
        GetComponent<SpriteRenderer>().color = normalColr;
    }

}
