using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Inventory : MonoBehaviour
{
    /* HERE
    public bool showing = true;
    public Transform itemSlotContainer, itemSlotTemplate, itemDescriptionContainer, itemDescriptionImage;
    UI_Displayer displayControl;


    Dictionary<Vector2, Inventory.ItemInfo> itemCoord = new Dictionary<Vector2, Inventory.ItemInfo>();
    Vector2 selectedCoord;

    static UI_Inventory instance;
    public static UI_Inventory i
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<UI_Inventory>();
            return instance;
        }
    }

    
    private void Awake()
    {
        displayControl = GetComponent<UI_Displayer>();

        itemSlotContainer = transform.Find("ItemSlotContainer");
        itemSlotTemplate = itemSlotContainer.Find("ItemSlotTemplate");

        itemDescriptionContainer = transform.Find("ItemDescriptionContainer");
        itemDescriptionImage = itemDescriptionContainer.Find("Image");

    }


    private void Start()
    {
        CloseBackpack();
        displayControl.afterHide = AfterClose;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Backspace))
        {
            if (showing) CloseBackpack();
            else OpenBackpack();
        }

        if (!showing) return;

        if (Input.GetKeyDown(KeyCode.A)) ItemMoveSelectedCoord(new Vector2(-1,0));
        if (Input.GetKeyDown(KeyCode.D)) ItemMoveSelectedCoord(new Vector2(1, 0));
        if (Input.GetKeyDown(KeyCode.W)) ItemMoveSelectedCoord(new Vector2(0, 1));
        if (Input.GetKeyDown(KeyCode.S)) ItemMoveSelectedCoord(new Vector2(0,-1));
    }

    
    public void CloseBackpack()
    {
        displayControl.HidePanel();
        showing = false;
    }

    public void OpenBackpack()
    {
        displayControl.ShowPanel();
        SetUpBackpack();
        showing = true;
    }

    public void AfterClose()
    {
        /*
        foreach (KeyValuePair<Vector2, Inventory.Item> p in itemCoord)
        {
            //Destroy(p.Value.uiBlock.gameObject);
            //p.Value.uiBlock = null;
        }
        itemCoord = new Dictionary<Vector2, Inventory.Item>();
        showing = false;
        equipmentDescriptionContainer.gameObject.SetActive(false);
        charmDescriptionContainer.gameObject.SetActive(false);
        */
    //} HERE

/*HERE
    public void SetUpBackpack()
    {
        int x = 0;
        int y = 0;
        int itemSlotCellSize = 120;
        itemCoord = new Dictionary<Vector2, Inventory.ItemInfo>();
        foreach (Inventory.ItemInfo ii in Inventory.i.Backpack)
        {
            ii.uiBlock = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            ii.uiBlock.gameObject.SetActive(true);
            ii.uiBlock.Find("Image").GetComponent<Image>().sprite = ii.objectSprite;
            ii.uiBlock.anchoredPosition += new Vector2(x * itemSlotCellSize, y * itemSlotCellSize);
            itemCoord.Add(new Vector2(x, y), ii);
            x++;
            if (x >= 6)
            {
                x = 0;
                y--;
            }
        }
        selectedCoord = new Vector2(0, 0);
        ItemMoveSelectedCoord(new Vector2(0, 0));
    }

    void ItemMoveSelectedCoord(Vector2 delta)
    {
      /*
        if (!itemCoord.ContainsKey(delta + selectedCoord)) return;
        itemCoord[selectedCoord].uiBlock.Find("WhenSelected").gameObject.SetActive(false);
        selectedCoord += delta;
        itemCoord[selectedCoord].uiBlock.Find("WhenSelected").gameObject.SetActive(true);
      */
        
    //}HERE
}
