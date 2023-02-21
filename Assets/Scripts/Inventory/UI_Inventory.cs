using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UI_Inventory : MonoBehaviour
{
    static UI_Inventory instance = null;
    public static UI_Inventory i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_Inventory>();
            }
            return instance;
        }
    }

    [SerializeField] GameObject inventoryBlockTemplate;
    [SerializeField] GameObject inventoryBlockBackgroundTemplate;
    List<GameObject> allBlockBackgrounds = new List<GameObject>();
    InventoryItemSO.Category displayingCategory;
    [SerializeField] List<UI_InventoryBlock> displayingBlocks = new List<UI_InventoryBlock>();
    int maxColumns = 3;
    int maxRows = 12;
    [SerializeField] float displacement;

    public bool resetBackground = false;

    [SerializeField] GameObject dragRawMaterial;
    [SerializeField] GameObject dragCraftMaterial;
    [SerializeField] GameObject dragFood;
    [SerializeField] GameObject dragFurniture;
    [SerializeField] GameObject dragObject;
    [SerializeField] GameObject dragTool;

    //public InventoryDrag dragging = null;


    private void Update()
    {
        if (!resetBackground)
        {
            resetBackground = true;
            CreateInventoryBlocksBackground();
        }
    }
    /*
    public InventoryDrag NewDragging(InventoryItemSO iiso)
    {
        InventoryItemSO.Category cat = iiso.category;
        GameObject go = null;
        if (cat == InventoryItemSO.Category.RawMaterial) go = Instantiate(dragRawMaterial);
        else if (cat == InventoryItemSO.Category.CraftMaterial) go = Instantiate(dragCraftMaterial);
        else if (cat == InventoryItemSO.Category.Food) go = Instantiate(dragFood);
        else if (cat == InventoryItemSO.Category.Tool) go = Instantiate(dragTool);
        else if (cat == InventoryItemSO.Category.Furniture) go = Instantiate(dragFurniture);
        else if (cat == InventoryItemSO.Category.Object) go = Instantiate(dragObject);
        dragging = go.GetComponent<InventoryDrag>();
        return dragging;
    }*/

    public void DisplayCategory(InventoryItemSO.Category cat)
    {
        displayingCategory = cat;
        foreach(UI_InventoryBlock ib in displayingBlocks)
        {
            Destroy(ib.gameObject);
        }
       displayingBlocks.Clear();
        foreach (Inventory.ItemInfo ii in Inventory.i.CategoryToList(cat))
        {
            if (ii.inStockAmount != 0)
            {
                CreateNewBlock(ii);
            }
        }
    }

     void CreateNewBlock(Inventory.ItemInfo ii)
    {
        GameObject newBlockGO = Instantiate(inventoryBlockTemplate, transform.Find("InventoryBlocks"));
        UI_InventoryBlock newIB = newBlockGO.GetComponent<UI_InventoryBlock>();
        newIB.SetUp(ii, displayingBlocks.Count / maxColumns, displayingBlocks.Count % maxColumns);
        displayingBlocks.Add(newIB);
        newBlockGO.transform.position = new Vector3(newBlockGO.transform.position.x + newIB.column * displacement,
                                                    newBlockGO.transform.position.y - newIB.row * displacement,
                                                    newBlockGO.transform.position.z);
        newBlockGO.SetActive(true);
    }

    public void UpdateItemDisplay(Inventory.ItemInfo ii)
    {
        if (ii.iiso.category != displayingCategory) return;
        if(ii.inStockAmount == 0)
        {
            foreach (UI_InventoryBlock ib in displayingBlocks)
            {
                if (ib.CheckIISO(ii.iiso))
                {
                    UI_InventoryBlock temp = ib;
                    displayingBlocks.Remove(temp);
                    Destroy(temp.gameObject);
                    break;
                }
            }
            DisplayCategory(displayingCategory);
            return;
        }
        foreach(UI_InventoryBlock ib in displayingBlocks)
        {
            if (ib.CheckIISO(ii.iiso))
            {
                ib.UpdateAmount();
                return;
            }
        }
        CreateNewBlock(ii);
    }

    public void CreateInventoryBlocksBackground()
    {
        print("recreate background blocks");
        print(maxRows + " " + maxColumns);

        //delete origional background
        /*
        if (allBlockBackgrounds!=null && allBlockBackgrounds.Count != 0)
        {
            foreach (GameObject go in allBlockBackgrounds)
            {
                go.SetActive(false);
            }
        }*/

        //make new background
        for(int i = 0; i<maxColumns; i++)
        {
            for(int j = 0; j<maxRows; j++)
            {
                GameObject go = Instantiate(inventoryBlockBackgroundTemplate, transform.Find("InventoryBlocksBackground"));
                go.transform.position = new Vector3(go.transform.position.x + i * displacement,
                                                        go.transform.position.y - j * displacement,
                                                        go.transform.position.z);
                go.SetActive(true);
                allBlockBackgrounds.Add(go);
            }
        }
    }

}
