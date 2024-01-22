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
    int maxRows = 6;
    [SerializeField] float horizontalDisplacement;
    [SerializeField] float verticalDisplacement;
    [SerializeField] private GameObject categoryIndicatorBuildings;
    [SerializeField] private GameObject categoryIndicatorMaterials;

    [SerializeField] private GameObject itemDetailUI;
    [SerializeField] private GameObject inventoryBlocksParent;

    public bool resetBackground = false;

    private void Awake()
    {
        CreateInventoryBlocks();
    }

    public void DisplayCategory(ItemScriptableObject.Category cat)
    {
        SetCategoryDisplayIndicator(cat);
        displayingCategory = cat;
        foreach(UI_InventoryBlock ib in allInventoryBlocks)
        {
            ib.ClearDisplay();
        }
        int j = 0;
        for(int i = 0; i < Inventory.i.CategoryToList(cat).Count; i++)
        {
            if(Inventory.i.CategoryToList(cat)[i].inStockAmount.GetAmount() > 0)
            {
                Inventory.ItemInfo itemInfoToDisplay = Inventory.i.CategoryToList(cat)[i];
                //print("blocks count: " + allInventoryBlocks.Count + ", count: " + Inventory.i.CategoryToList(cat).Count + ", i: " + i);
                allInventoryBlocks[j].SetUpDisplay(itemInfoToDisplay);
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
                GameObject go = Instantiate(inventoryBlockTemplate, inventoryBlocksParent.transform);
                go.transform.localPosition += new Vector3(i * horizontalDisplacement, -j * verticalDisplacement, 0);
                go.gameObject.name = "IB_" + i + "_" + j;
                go.tag = "InventoryBlock";
                go.SetActive(true);
                go.GetComponent<UI_InventoryBlock>().Initialize(i,j);
                allInventoryBlocks.Add(go.GetComponent<UI_InventoryBlock>());
            }
        }
    }

    public void DisplayItemDetail(ItemScriptableObject isoToDisplay)
    {

        //Vector3 toSet = WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.UI_BACKGROUND, true); //new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, PanelTransform.Find("NameUI").transform.position.z);
        itemDetailUI.gameObject.SetActive(true);

        GameObject[] allInventoryBlock = GameObject.FindGameObjectsWithTag("InventoryBlock");
        foreach (GameObject block in allInventoryBlock)
        {
            //if (block.transform.Find("itemSprite").iso.name == isoToDisplay.tetrisHoverName)
            //{

            //}

        }
        //itemDetailUI.transform.localPosition = ;



        //itemDetailUI.transform.Find("Tetris Pic").gameObject.GetComponent<SpriteRenderer>().sprite = isoToDisplay.tetrisSprite;
        itemDetailUI.transform.Find("Tetris Name").gameObject.GetComponent<TextMeshPro>().text = isoToDisplay.tetrisHoverName;
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

    public void ButtonDisplayCategoryBuildings()
    {
        DisplayCategory(ItemScriptableObject.Category.Building);
    }
    
    public void ButtonDisplayCategoryMaterials()
    {
        DisplayCategory(ItemScriptableObject.Category.Material);
    }
    
    public void SetCategoryDisplayIndicator(ItemScriptableObject.Category displayCategory)
    {
        if (displayCategory == ItemScriptableObject.Category.Building)
        {
            categoryIndicatorBuildings.SetActive(true);
            categoryIndicatorMaterials.SetActive(false);
        }
        else if (displayCategory == ItemScriptableObject.Category.Material)
        {
            categoryIndicatorBuildings.SetActive(false);
            categoryIndicatorMaterials.SetActive(true);
        }
    }

}
