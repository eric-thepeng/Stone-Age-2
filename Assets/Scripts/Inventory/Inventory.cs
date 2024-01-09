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
        public PlayerStat totalAmount;
        public PlayerStat inUseAmount;
        public PlayerStat inWorkshopAmount;
        public PlayerStat inBuildAmount;

        public PlayerStat inStockAmount; //{ get { return totalAmount - inUseAmount - inWorkshopAmount - inBuildAmount; } }
        public ItemScriptableObject.Category category
        {
            get { return iso.category;}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newISO"></param>
        /// <param name="initialInstockAmount">only put 1 or 0</param>
        public ItemInfo(ItemScriptableObject newISO, int initialInstockAmount)
        {
            iso = newISO;
            totalAmount = new PlayerStat(initialInstockAmount);
            inUseAmount = new PlayerStat(0);
            inWorkshopAmount = new PlayerStat(0);
            inBuildAmount = new PlayerStat(0);
            
            inStockAmount = new PlayerStat(initialInstockAmount); //default to 1, same as totalAmount
            
            totalAmount.SubscribeStatDelta(PositiveChangeInStockAmount);
            inUseAmount.SubscribeStatDelta(NegativeChangeInStockAmount);
            inWorkshopAmount.SubscribeStatDelta(NegativeChangeInStockAmount);
            inBuildAmount.SubscribeStatDelta(NegativeChangeInStockAmount);
        }

        public void PositiveChangeInStockAmount(int delta)
        {
            inStockAmount.ChangeAmount(delta);
        }

        public void NegativeChangeInStockAmount(int delta)
        {
            inStockAmount.ChangeAmount(-delta);
        }
        
    }

    private List<ItemInfo> CatListBuilding = new List<ItemInfo>();
    private List<ItemInfo> CatListMaterial = new List<ItemInfo>();

    public void AddInventoryItem(ItemScriptableObject newISO, int amount = 1)
    {
        if (amount == 0) return;
        UI_Harvest.i.AddItem(newISO, amount);
        PlayerStatsMonitor.isoTotalGainedPlayerStatCollection.GetPlayerStat(newISO).ChangeAmount(amount);
        foreach (ItemInfo ii in CategoryToList(newISO.category))
        {
            // Find existing II
            if (ii.iso == newISO)
            {
                ii.totalAmount.ChangeAmount(amount);
                //print("added amount: " + newISO);
                UI_Inventory.i.UpdateItemDisplay(ii);
                return;
            }
        }
        
        // No existing II
        ItemInfo newII = CreateNewItemInfo(newISO,1);
        if (amount == 1) return;
        newII.totalAmount.ChangeAmount(amount-1);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newISO"></param>
    /// <param name="initialInStockAmount">only put 0 or 1</param>
    private ItemInfo CreateNewItemInfo(ItemScriptableObject newISO, int initialInStockAmount)
    {
        ItemInfo newII = new ItemInfo(newISO, initialInStockAmount);
        CategoryToList(newISO.category).Add(newII);
        return newII;
    }

    public int ItemInStockAmount(ItemScriptableObject newISO)
    {
        if (GetItemInfo(newISO) == null) return 0;
        return GetItemInfo(newISO).inStockAmount.GetAmount();
    }
    
    /// <summary>
    /// Exists in form of Tetris
    /// </summary>
    /// <param name="iso"></param>
    /// <param name="use"></param>
    public void InUseItem(ItemScriptableObject iso, bool use)
    {
        if (use)
        {
            GetItemInfo(iso).inUseAmount.ChangeAmount(1);
        }
        else
        {
            GetItemInfo(iso).inUseAmount.ChangeAmount(-1);
        }
        UI_Inventory.i.UpdateItemDisplay(GetItemInfo(iso));
    }

    /// <summary>
    /// Exists in crafting in Workshop
    /// </summary>
    public void InWorkshopItem(ItemScriptableObject iso, bool use)
    {
        if (use)
        {
            GetItemInfo(iso).inWorkshopAmount.ChangeAmount(1);
        }
        else
        {
            GetItemInfo(iso).inWorkshopAmount.ChangeAmount(-1);
        }
        UI_Inventory.i.UpdateItemDisplay(GetItemInfo(iso));
    }
    
    /// <summary>
    /// Player's inventory count, triggers when an inventory item is built or when a built item returns to inventory.
    /// </summary>
    /// <param name="biso">Target BISO that changes</param>
    /// <param name="use">True: From inventory being built. False: From built go back to inventory./param>
    public void InBuildItem(BuildingISO biso, bool use)
    {
        if (use)
        {
            GetItemInfo(biso).inBuildAmount.ChangeAmount(1);
            PlayerStatsMonitor.bisoTotalBuiltPlayerStatCollection.GetPlayerStat(biso).ChangeAmount(1);
        }
        else
        {
            GetItemInfo(biso).inBuildAmount.ChangeAmount(-1);
            PlayerStatsMonitor.bisoTotalBuiltPlayerStatCollection.GetPlayerStat(biso).ChangeAmount(-1);
        }
        UI_Inventory.i.UpdateItemDisplay(GetItemInfo(biso));
    }

    public void MergeCreateItem(ItemScriptableObject iso)
    {
        AddInventoryItem(iso);
        InUseItem(iso, true);
    }

    public void UseItemFromStock(ItemScriptableObject iso, int amount = 1)
    {
        GetItemInfo(iso).totalAmount.ChangeAmount(-amount);
        UI_Inventory.i.UpdateItemDisplay(GetItemInfo(iso));
    }

    public bool SpendResourceSet(ResourceSet rSet)
    {
        //check if have enough resource
        foreach(ResourceSet.ResourceAmount rsra in rSet.resources)
        {
            if (GetISOInstockAmount(rsra.iso) < rsra.amount) return false;
        }

        //check and use spirit point
        if (!SpiritPoint.i.Use(rSet.spiritPoint)) return false;

        //use resource
        foreach (ResourceSet.ResourceAmount rsra in rSet.resources)
        {
            UseItemFromStock(rsra.iso, rsra.amount);
        }

        return true;
    }

    public int GetISOInstockAmount(ItemScriptableObject iso)
    {
        return GetISOInstockPlayerStat(iso).GetAmount();
    }

    public PlayerStat GetISOInstockPlayerStat(ItemScriptableObject iso)
    {
        ItemInfo ii = GetItemInfo(iso);
        if (ii == null)
        {
            ii = CreateNewItemInfo(iso, 0);
        }
        return ii.inStockAmount;
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
        if (cat == ItemScriptableObject.Category.Material) return CatListMaterial;
        if (cat == ItemScriptableObject.Category.Building) return CatListBuilding;
        Debug.LogError("There is a ISO Category without a CatList in Inventory");
        return CatListBuilding;
    }


}
