using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] float horizontalDisplacement;
    [SerializeField] float verticalDisplacement;

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
                go.transform.localPosition += new Vector3(i * horizontalDisplacement, -j * verticalDisplacement, 0);
                go.gameObject.name = "IB_" + i + "_" + j;
                go.SetActive(true);
                go.GetComponent<UI_InventoryBlock>().Initialize(i,j);
                allInventoryBlocks.Add(go.GetComponent<UI_InventoryBlock>());
            }
        }
    }

    public void DisplayItemDetail(ItemScriptableObject isoToDisplay)
    {
        //Vector3 toSet = WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.UI_BACKGROUND, true); //new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, PanelTransform.Find("NameUI").transform.position.z);
        transform.Find("Item Detail UI").gameObject.SetActive(true);
        transform.Find("Item Detail UI").Find("Tetris Pic").gameObject.GetComponent<SpriteRenderer>().sprite = isoToDisplay.tetrisSprite;
        transform.Find("Item Detail UI").Find("Tetris Name").gameObject.GetComponent<TextMeshPro>().text = isoToDisplay.tetrisHoverName;
        /*
        PanelTransform.Find("NameUI").gameObject.GetComponentInChildren<TextMeshPro>().text = uiib.GetISO().tetrisHoverName;
        PanelTransform.Find("NameUI").Find("TetrisUI").Find("TetrisPic").gameObject.GetComponent<SpriteRenderer>().sprite = uiib.GetISO().tetrisSprite;
        PanelTransform.Find("NameUI").gameObject.GetComponentInChildren<TextMeshPro>().sortingLayerID = PanelTransform.Find("NameUI").GetComponentInChildren<SpriteRenderer>().sortingLayerID;
        PanelTransform.Find("NameUI").gameObject.GetComponentInChildren<TextMeshPro>().sortingOrder = PanelTransform.Find("NameUI").GetComponentInChildren<SpriteRenderer>().sortingOrder + 1;
    */
    }

    public void CancelDisplayItemDetail()
    {
        transform.Find("Item Detail UI").gameObject.SetActive(false);
    }

}
