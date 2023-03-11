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

    public bool CheckIISO(ItemScriptableObject inISO)
    {
        return inISO == itemInfo.iso;
    }

    public ItemScriptableObject GetISO()
    {
        return itemInfo.iso;
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
        displayNumber.text = "" + ii.inStockAmount;

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
        if (CraftingManager.i.isPanelOpen()) //TETRIS
        {
            GameObject newTetris = CraftingManager.i.CreateTetris(itemInfo.iso, WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.UI_BACKGROUND, true), CraftingManager.CreateFrom.INVENTORY);
            newTetris.GetComponent<DragInventoryItem>().SetUp(this);
        }
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
