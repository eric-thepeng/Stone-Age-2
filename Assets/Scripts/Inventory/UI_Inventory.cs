using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
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
    [SerializeField] List<UI_InventoryBlock> allInventoryBlocks = new List<UI_InventoryBlock>();
    ItemScriptableObject.Category displayingCategory;
    int maxColumns = 4;
    int maxRows = 5;
    [SerializeField] float displacement;

    public bool resetBackground = false;

    [SerializeField] GameObject dragRawMaterial;
    [SerializeField] GameObject dragCraftMaterial;
    [SerializeField] GameObject dragFood;
    [SerializeField] GameObject dragFurniture;
    [SerializeField] GameObject dragObject;
    [SerializeField] GameObject dragTool;

    private void Awake()
    {
        CreateInventoryBlocks();
    }

    private void Start()
    {
        
    }

    public void DisplayCategory(ItemScriptableObject.Category cat)
    {
        displayingCategory = cat;
        foreach(UI_InventoryBlock ib in allInventoryBlocks)
        {
            ib.ClearDisplay();
        }
        int j = 0;
        for(int i = 0; i < Inventory.i.CategoryToList(cat).Count; i++)
        {
            if(Inventory.i.CategoryToList(cat)[i].inStockAmount > 0)
            {
                //print("blocks count: " + allInventoryBlocks.Count + ", count: " + Inventory.i.CategoryToList(cat).Count + ", i: " + i);
                allInventoryBlocks[j].SetUpDisplay(Inventory.i.CategoryToList(cat)[i]);
                j++;
            }
        }
    }

    
    public void UpdateItemDisplay(Inventory.ItemInfo ii)
    {
        DisplayCategory(displayingCategory);
        /*
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
        }*/
    }
    
    public void CreateInventoryBlocks()
    {
        print("recreate background blocks");
        allInventoryBlocks.Clear();
        //make new background
        for(int j = 0; j < maxRows; j++)
        {
            for(int i = 0; i < maxColumns; i++)
            {
                GameObject go = Instantiate(inventoryBlockTemplate, transform.Find("Inventory Blocks"));
                go.transform.localPosition += new Vector3(i * displacement, -j * displacement, 0);
                go.gameObject.name = "IB_" + i + "_" + j;
                go.SetActive(true);
                go.GetComponent<UI_InventoryBlock>().Initialize(i,j);
                allInventoryBlocks.Add(go.GetComponent<UI_InventoryBlock>());
            }
        }
    }

}
