using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BLDWorkshop : BuildingInteractable
{
    public class WorkshopData
    {
        public struct ISOAndAmount
        {
            public ItemScriptableObject iso;
            public int amount;
            public ISOAndAmount(ItemScriptableObject iso, int amount)
            {
                this.iso = iso;
                this.amount = amount;
            }
        }

        public List<ISOAndAmount> materialStat;
        public ISOAndAmount productStat;

    }

    enum State
    {Idle, Assigning
    }

    private State state = State.Idle;

    private bool isWorking = false;

    private ItemScriptableObject[] currentMaterialsArray = new ItemScriptableObject[3]{null, null, null};

    private SO_WorkshopRecipe currentRecipe = null;

    [SerializeField] private GameObject workshopCraftingUI = null;
    private WorkshopCraftingController wcc;

    public SO_WorkshopRecipe.WorkshopType workshopType;

    private void Awake()
    {
        wcc = new WorkshopCraftingController(this, workshopCraftingUI);
    }

    private void Start()
    {
        currentInteraction = new InteractionType(InteractionType.TypeName.Click, ClickEvent);
    }

    private void Update()
    {
        wcc.UpdateCraftingUI(Time.deltaTime);
    }

    #region interaction logic

    private void ClickEvent()
    {
        if (state == State.Idle)
        {
            EnterUI();
        }
    }

    protected override void TurnOnHighlight()
    {
        if(state == State.Assigning) return;
        base.TurnOnHighlight();
    }
    

    public void EnterUI()
    {
        UI_BLDWorkshop.i.TurnOnUI(this);
        state = State.Assigning;
        PlayerState.OpenCloseAllocatingBackpack(true);
        CameraManager.i.MoveToDisplayLocation(transform.position + new Vector3(0,0,15), 100f);
    }

    public void ExitUI()
    {
        if (state == State.Assigning)
        {
            currentInteraction = new InteractionType(InteractionType.TypeName.Click, ClickEvent);
            
            UI_BLDWorkshop.i.TurnOffUI();
            state = State.Idle;
            ClearAllMaterialAndProduct();
            PlayerState.OpenCloseAllocatingBackpack(false);
        }
        else
        {
            Debug.LogError("Exit workshop UI while UI is not opened?");
        }
    }
    
    #endregion

    private void ClearAllMaterialAndProduct()
    {
        currentMaterialsArray = new ItemScriptableObject[3]{null, null, null};
        currentRecipe = null;
        UI_BLDWorkshop.i.ClearAllMaterialAndProductIcon();
    }

    public void UpdateMaterialList(ItemScriptableObject iso, int index)
    {
        currentMaterialsArray[index - 1] = iso;
        CheckMaterialListMatch();
    }

    private void CheckMaterialListMatch()
    {
        SO_WorkshopRecipe[] allWorkshopRecipes = Resources.LoadAll<SO_WorkshopRecipe>("World Interaction/Workshop Recipes");
        foreach (SO_WorkshopRecipe wr in allWorkshopRecipes)
        {
            //Check if there is a match.
            if (wr.CheckMaterialMatch(currentMaterialsArray))
            {
                UpdateProductAndRecipe(wr);
                return;
            }
        }
        UpdateProductAndRecipe(null);
    }

    private void UpdateProductAndRecipe(SO_WorkshopRecipe wr = null)
    {
        UI_BLDWorkshop.i.UpdateProductIcon(wr?.product);
        if (wr != null)
        {
            UI_BLDWorkshop.i.UpdateProductAndRecipeAmount(currentMaterialsArray[0]!=null, currentMaterialsArray[1]!=null, currentMaterialsArray[2]!=null, 0);
            wcc.ResetCurrentCraftingAmount();
        }
        else
        {
            UI_BLDWorkshop.i.ClearProductAndRecipeAmount();
        }
        currentRecipe = wr;
    }

    public void StartCrafting()
    {
        wcc.StartAndSetUpCrafting(currentRecipe);
        ExitUI(); //put this after wwc.StartAndSetUpCrafting to avoid currentRecipe reset
    }
    
    public void AdjustProductAmountClicked(int amount)
    {
        if (amount > 0) // putting iso into workshop
        {
            foreach (ItemScriptableObject iso in currentRecipe.materials)
            {
                if (Inventory.i.GetISOInstockAmount(iso) <= 0) return;
            }
            foreach (ItemScriptableObject iso in currentRecipe.materials)
            {
                Inventory.i.InWorkshopItem(iso, true);
            }
        }
        else // putting iso into inventory
        {
            if(wcc.GetCurrentCraftingAmount() < amount || wcc.GetCurrentCraftingAmount() <= 0) return;
            foreach (ItemScriptableObject iso in currentRecipe.materials)
            {
                Inventory.i.InWorkshopItem(iso, false);
            }
        }
        
        wcc.AdjustCurrentCraftingAmount(amount);
    }

    public void AdjustProductAmountUI(int amount)
    {
        UI_BLDWorkshop.i.UpdateProductAndRecipeAmount(currentMaterialsArray[0]!=null, currentMaterialsArray[1]!=null, currentMaterialsArray[2]!=null, amount);
    }
    
}

public class WorkshopCraftingController
{
    private BLDWorkshop workshop;
    private SO_WorkshopRecipe craftingWR;
    public bool isCrafting;
    private GameObject ui;
    private UI_ISOIconDisplayBox productDisplayBox;
    private CircularUI circularUI;

    private float currentCraftingTime = 0;
    private int currentCraftingAmount = 0;
    
    public WorkshopCraftingController(BLDWorkshop workshop, GameObject wwcUI)
    {
        this.workshop = workshop;
        ui = wwcUI;
    }

    public void StartAndSetUpCrafting(SO_WorkshopRecipe recipeToCraft)
    {
        craftingWR = recipeToCraft;
        currentCraftingTime = 0;
        ShowCraftingUI();
        productDisplayBox = ui.GetComponentInChildren<UI_ISOIconDisplayBox>();
        circularUI = ui.GetComponentInChildren<CircularUI>();
        isCrafting = true;
        productDisplayBox.Display(craftingWR.product, false);
    }
    
    public void ExitCrafting()
    {
        isCrafting = false;
        HideCraftingUI();
    }
    
    public void UpdateCraftingUI(float deltaTime)
    {
        if(!isCrafting) return;
        if(currentCraftingAmount == 0) ExitCrafting();
        currentCraftingTime += deltaTime;
        circularUI.SetCircularUIPercentage(currentCraftingTime * 100f/ craftingWR.workTime,false);
        if (currentCraftingTime >= craftingWR.workTime)
        {
            currentCraftingTime = 0;
            AdjustCurrentCraftingAmount(-1);
            SpawnProduct();
        }
    }

    public void ShowCraftingUI()
    {
        ui.SetActive(true);
    }
    

    public void HideCraftingUI()
    {
        productDisplayBox.Clear();
        ui.SetActive(false);
    }

    public void SpawnProduct()
    {
        Inventory.i.AddInventoryItem(craftingWR.product);
    }

    public int GetCurrentCraftingAmount()
    {
        return currentCraftingAmount;
    }
    
    public void ResetCurrentCraftingAmount()
    {
        currentCraftingAmount = 0;
    }

    public int AdjustCurrentCraftingAmount(int delta)
    {
        currentCraftingAmount += delta;
        //change inventory
        workshop.AdjustProductAmountUI(currentCraftingAmount);
        return currentCraftingAmount;
    }
    
}
