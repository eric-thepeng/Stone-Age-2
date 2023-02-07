using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    /*

    static Inventory instance;
    public static Inventory i
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<Inventory>();
            return instance;
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("Inventory Information:");
            foreach(ItemInfo ii in Backpack)
            {
                print(ii.soForm.name + "  " + ii.amount);
            }
            print("End of Inventory Information:");
        }
    }

    /// <summary>
    /// Information to be displayed on each grid.
    /// </summary>
    public class ItemInfo
    {
        public RectTransform uiBlock;
        public ItemScriptableObject soForm;
        public Sprite objectSprite, tetrisSprite;
        public int amount;

        public ItemInfo(ItemScriptableObject inSO)
        {
            uiBlock = null;
            soForm = inSO;
            objectSprite = soForm.objectSprite;
            tetrisSprite = soForm.tetrisSprite;
            amount = 1;
        }

        public void AddOne()
        {
            amount += 1;
        }

        public void MinusOne()
        {
            amount -= 1;
        }
    }

    public List<ItemInfo> Backpack = new List<ItemInfo>();

    public int AmountOf(ItemScriptableObject iso)
    {
        foreach(ItemInfo ii in Backpack)
        {
            if (ii.soForm == iso) return ii.amount;
        }
        return 0;
    }

    public void AddItem(ItemScriptableObject iso)
    {
        //add to crafting
        CraftingManager.i.AddToCrafting(iso.myPrefab);

        //add to backpack
        foreach(ItemInfo ii in Backpack)
        {
            if(ii.soForm == iso)
            {
                ii.AddOne();
                return;
            }
        }
        Backpack.Add(new ItemInfo(iso));
    }

    public void DeleteItem(ScriptableObject iso)
    {
        foreach (ItemInfo ii in Backpack)
        {
            if (ii.soForm == iso)
            {
                ii.MinusOne();
                return;
            }
        }
        print("Backpack does not have this item: " + iso);
    }
    */
}
