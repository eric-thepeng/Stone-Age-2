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
        public int displayAmount { get { return totalAmount - inUseAmount; } }
        public ItemScriptableObject.Category category
        {
            get { return iso.category;}
        }

        public ItemInfo(ItemScriptableObject newISO)
        {
            iso = newISO;
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

    public List<ItemInfo> catTemporary = new List<ItemInfo>();

    public void AddInventoryItem(ItemScriptableObject newISO)
    {
        
        foreach(ItemInfo ii in CategoryToList(newISO.category))
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
        //UI_Inventory.i.UpdateItemDisplay(GetItemInfo(iiso));
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
