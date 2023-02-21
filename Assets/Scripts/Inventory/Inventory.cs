using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    static Inventory instance = null;
    public static Inventory i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Inventory>();
            }
            return instance;
        }
    }

    public class ItemInfo {
        public InventoryItemSO iiso;
        public int totalAmount;
        public int inUseAmount;
        public int inStockAmount { get { return totalAmount - inUseAmount; } }
        public InventoryItemSO.Category category
        {
            get { return iiso.category;}
        }

        public ItemInfo(InventoryItemSO newIISO)
        {
            iiso = newIISO;
            totalAmount = 1;
            inUseAmount = 0;
        }
    }

    public List<ItemInfo> catRawMaterial = new List<ItemInfo>();
    public List<ItemInfo> catCraftMaterial = new List<ItemInfo>();
    public List<ItemInfo> catFood = new List<ItemInfo>();
    public List<ItemInfo> catTool = new List<ItemInfo>();
    public List<ItemInfo> catFurniture = new List<ItemInfo>();
    public List<ItemInfo> catObject = new List<ItemInfo>();

    public void AddInventoryItem(InventoryItemSO newIISO)
    {
        
        foreach(ItemInfo ii in CategoryToList(newIISO.category))
        {
            if (ii.iiso == newIISO)
            {
                ii.totalAmount += 1;
                print("added amount: " + newIISO.name);
               // UI_Inventory.i.UpdateItemDisplay(ii);
                return;
            }
        }
        ItemInfo newII = new ItemInfo(newIISO);
        CategoryToList(newIISO.category).Add(newII);
        print("added new: " + newIISO.name);
        //UI_Inventory.i.UpdateItemDisplay(newII);
    }

/// <summary>
/// true: total -> inUse     false: inUse -> total
/// </summary>
/// <param name="iiso"></param>
/// <param name="use"></param>
    public void InUseItem(InventoryItemSO iiso, bool use)
    {
        if (use)
        {
            GetItemInfo(iiso).inUseAmount += 1;
        }
        else
        {
            GetItemInfo(iiso).inUseAmount -= 1;
        }
        //UI_Inventory.i.UpdateItemDisplay(GetItemInfo(iiso));
    }

    ItemInfo GetItemInfo(InventoryItemSO iiso)
    {
        List<ItemInfo> list = CategoryToList(iiso.category);
        foreach(ItemInfo ii in list)
        {
            if (ii.iiso == iiso) return ii;
        }
        return null;
    }

    public List<ItemInfo> CategoryToList(InventoryItemSO.Category cat)
    {
        if (cat == InventoryItemSO.Category.RawMaterial) return catRawMaterial;
        else if (cat == InventoryItemSO.Category.CraftMaterial) return catCraftMaterial;
        else if (cat == InventoryItemSO.Category.Food) return catFood;
        else if (cat == InventoryItemSO.Category.Tool) return catTool;
        else if (cat == InventoryItemSO.Category.Furniture) return catFurniture;
        else if (cat == InventoryItemSO.Category.Object) return catObject;
        return null;
    }
}
