using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_InventoryBlock : MonoBehaviour
{
    Inventory.ItemInfo itemInfo;
    public int row;
    public int column;
    public int displayAmount;
    bool mouseOver = false;

    Color normalColr= Color.white;
    Color dragColr = Color.grey;

    public bool CheckIISO(InventoryItemSO inIISO)
    {
        return inIISO == itemInfo.iiso;
    }

    public void SetUp(Inventory.ItemInfo ii, int inRow, int inColumn)
    {
        itemInfo= ii;
        row = inRow;
        column = inColumn;
        displayAmount = ii.totalAmount - ii.inUseAmount;

        //display shit
        GetComponent<SpriteRenderer>().sprite = itemInfo.iiso.inventoryIcon;
        GetComponentInChildren<TextMeshPro>().text = "" + (ii.totalAmount - ii.inUseAmount);
    }

    public void UpdateAmount()
    {
        GetComponentInChildren<TextMeshPro>().text = "" + (itemInfo.totalAmount - itemInfo.inUseAmount);
    }

    private void OnMouseEnter()
    {
        //InventoryHoverInfo.i.Display(transform.position);
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        //InventoryHoverInfo.i.Disappear();
        mouseOver = false;
    }

    private void OnMouseDown()
    {
        //InventoryHoverInfo.i.Disappear();
        CreateDrag();
    }

    private void OnMouseUp()
    {
        //if ( mouseOver ) { InventoryHoverInfo.i.Display(transform.position); }
    }

    void CreateDrag()
    {
        GetComponent<SpriteRenderer>().color = dragColr;
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
