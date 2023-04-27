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
        public ItemScriptableObject iso;
        public int totalAmount;
        public int inUseAmount;
        public int inBuildAmount;

        public int inStockAmount { get { return totalAmount - inUseAmount - inBuildAmount; } }
        public ItemScriptableObject.Category category
        {
            get { return iso.category;}
        }

        public ItemInfo(ItemScriptableObject newISO)
        {
            iso = newISO;
            totalAmount = 1;
            inUseAmount = 0;
            inBuildAmount = 0;
        }
    }

    public List<ItemInfo> catRawMaterial = new List<ItemInfo>();
    public List<ItemInfo> catCraftMaterial = new List<ItemInfo>();
    public List<ItemInfo> catFood = new List<ItemInfo>();
    public List<ItemInfo> catTool = new List<ItemInfo>();
    public List<ItemInfo> catFurniture = new List<ItemInfo>();
    public List<ItemInfo> catObject = new List<ItemInfo>();

    public List<ItemInfo> catTemporary = new List<ItemInfo>();

    public void AddInventoryItem(ItemScriptableObject newISO)
    {
        UI_Harvest.i.AddItem(newISO, 1);
        foreach (ItemInfo ii in CategoryToList(newISO.category))
        {
            if (ii.iso == newISO)
            {
                ii.totalAmount += 1;
                print("added amount: " + newISO.name);
                UI_Inventory.i.UpdateItemDisplay(ii);
                return;
            }
        }
        ItemInfo newII = new ItemInfo(newISO);
        CategoryToList(newISO.category).Add(newII);
        print("added new: " + newISO.name);
        UI_Inventory.i.UpdateItemDisplay(newII);
    }

    public int ItemInStockAmount(ItemScriptableObject newISO)
    {
        if (GetItemInfo(newISO) == null) return 0;
        return GetItemInfo(newISO).inStockAmount;
    }

/// <summary>
/// true: total -> inUse     false: inUse -> total
/// </summary>
/// <param name="iiso"></param>
/// <param name="use"></param>
    public void InUseItem(ItemScriptableObject iso, bool use)
    {
        if (use)
        {
            GetItemInfo(iso).inUseAmount += 1;
        }
        else
        {
            GetItemInfo(iso).inUseAmount -= 1;
        }
        UI_Inventory.i.UpdateItemDisplay(GetItemInfo(iso));
    }

    public void InBuildItem(BuildingISO biso, bool use)
    {
        if (use)
        {
            GetItemInfo(biso).inBuildAmount += 1;
        }
        else
        {
            GetItemInfo(biso).inBuildAmount -= 1;
        }
        UI_Inventory.i.UpdateItemDisplay(GetItemInfo(biso));
    }

    public void MergeCreateItem(ItemScriptableObject iso)
    {
        AddInventoryItem(iso);
        InUseItem(iso, true);
    }

    public void UseItemFromStock(ItemScriptableObject iso)
    {
        GetItemInfo(iso).totalAmount -= 1;
        UI_Inventory.i.UpdateItemDisplay(GetItemInfo(iso));
    }

    ItemInfo GetItemInfo(ItemScriptableObject iso)
    {
        List<ItemInfo> list = CategoryToList(iso.category);
        foreach(ItemInfo ii in list)
        {
            if (ii.iso == iso) return ii;
        }
        return null;
    }

    public List<ItemInfo> CategoryToList(ItemScriptableObject.Category cat)
    {
        /*
        if (cat == InventoryItemSO.Category.RawMaterial) return catRawMaterial;
        else if (cat == InventoryItemSO.Category.CraftMaterial) return catCraftMaterial;
        else if (cat == InventoryItemSO.Category.Food) return catFood;
        else if (cat == InventoryItemSO.Category.Tool) return catTool;
        else if (cat == InventoryItemSO.Category.Furniture) return catFurniture;
        else if (cat == InventoryItemSO.Category.Object) return catObject;
        return null;*/
        return catTemporary;
    }
}
