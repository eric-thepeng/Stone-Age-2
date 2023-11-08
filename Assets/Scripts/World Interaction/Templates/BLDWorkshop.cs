using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BLDWorkshop : BuildingInteractable
{
    public class WorkshopData
    {
        /* The set of data provided to UI_BLDWorkshop to display
         
         - display combination of materials
         - if workshop recipe exist: 
            - display product
            - display amount
         
        */
        public class ISOAndAmount
        {
            public ItemScriptableObject iso;
            public int amount;
            public ISOAndAmount(ItemScriptableObject iso, int amount)
            {
                this.iso = iso;
                this.amount = amount;
            }

            public ISOAndAmount()
            {
                Reset();
            }

            public void Reset()
            {
                this.iso = null;
                this.amount = 0;
            }
        }

        private BLDWorkshop workshop;
        public List<ISOAndAmount> materialStat;
        public ISOAndAmount productStat;
        public ISOAndAmount finishedProductStat;
        public SO_WorkshopRecipe currentWorkshopRecipe;
        public int currentWorkshopRecipeAmount;

        public WorkshopData(BLDWorkshop workshop)
        {
            this.workshop = workshop;
            ResetAll();
        }
        
        public void ResetAll()
        {
            materialStat = new List<ISOAndAmount>(){new ISOAndAmount(),new ISOAndAmount(),new ISOAndAmount()};
            productStat = new ISOAndAmount();
            finishedProductStat = new ISOAndAmount();
            currentWorkshopRecipe = null;
            currentWorkshopRecipeAmount = 0;
        }
        
        public void AssignMaterial(int index, ItemScriptableObject newISO)
        {
            materialStat[index - 1].iso = newISO;
            materialStat[index - 1].amount = 0;
            workshop.CheckIfRecipeExists();
        }

        public void AssignRecipe(SO_WorkshopRecipe wr)
        {
            currentWorkshopRecipe = wr;
            productStat.iso = wr.product;
            currentWorkshopRecipeAmount = 0;
            productStat.amount = 0;
            foreach (var isoAndAmount in materialStat)
            {
                isoAndAmount.amount = 0;
            }
            UI_BLDWorkshop.i.RefreshUI();
            //some kind of refresh
        }
        
        public void AdjustRecipeAmount(int amount)
        {
            if(amount != 1 && amount != -1) Debug.LogError("Illegal input");
            if (amount == 1)
            {
                foreach (var materialISO in currentWorkshopRecipe.materials)
                {
                    foreach (var materialISOAA in materialStat)
                    {
                        if (materialISOAA.iso == materialISO) materialISOAA.amount += amount;
                    }
                }
            }
            else
            {
                foreach (var materialISO in currentWorkshopRecipe.materials)
                {
                    foreach (var materialISOAA in materialStat)
                    {
                        if (materialISOAA.iso == materialISO) materialISOAA.amount -= amount;
                    }
                }
            }

            productStat.amount += amount;
            UI_BLDWorkshop.i.RefreshUI();
        }

        public ItemScriptableObject[] GetCurrentMaterialArray()
        {
            return new ItemScriptableObject[] {materialStat[0].iso,materialStat[1].iso,materialStat[2].iso};
        }

        public void FinishOneProduction()
        {
            
        }
    }

    enum State
    {Idle, Assigning
    }

    private State state = State.Idle;

    private bool isWorking = false;
    
    [SerializeField] private GameObject workshopCraftingUI = null;
    private WorkshopCraftingController workshopCraftingController;

    public SO_WorkshopRecipe.WorkshopType workshopType;

    public WorkshopData workshopData;

    private void Awake()
    {
        workshopCraftingController = new WorkshopCraftingController(this, workshopCraftingUI);
        workshopData = new WorkshopData(this);
    }

    private void Start()
    {
        currentInteraction = new InteractionType(InteractionType.TypeName.Click, ClickEvent);
    }

    private void Update()
    {
        workshopCraftingController.UpdateCraftingUI(Time.deltaTime);
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
        workshopData.ResetAll();
    }
    
    //implemented
    public void CheckIfRecipeExists()
    {
        SO_WorkshopRecipe[] allWorkshopRecipes = Resources.LoadAll<SO_WorkshopRecipe>("World Interaction/Workshop Recipes");
        foreach (SO_WorkshopRecipe wr in allWorkshopRecipes)
        {
            if(!wr.AvailableInWorkshops[workshopType])continue;
            //Check if there is a match.
            if (wr.CheckMaterialMatch(workshopData.GetCurrentMaterialArray()))
            {
                workshopData.AssignRecipe(wr);
                return;
            }
        }
        //UpdateRecipe(null);
    }

    public void StartCrafting()
    {
        workshopCraftingController.StartAndSetUpCrafting();
        ExitUI(); //put this after wwc.StartAndSetUpCrafting to avoid currentRecipe reset
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"> +1 or -1 </param>
    public void AdjustProductAmountClicked(int amount)
    {
        if (amount > 0) // put iso from inventory into workshop, return if not sufficient
        {
            foreach (ItemScriptableObject iso in workshopData.currentWorkshopRecipe.materials)
            {
                if (Inventory.i.GetISOInstockAmount(iso) <= 0) return;
            }
            foreach (ItemScriptableObject iso in workshopData.currentWorkshopRecipe.materials)
            {
                Inventory.i.InWorkshopItem(iso, true);
            }
        }
        else // putting iso into inventory
        {
            if(workshopData.currentWorkshopRecipeAmount <= 0) return;
            foreach (ItemScriptableObject iso in workshopData.currentWorkshopRecipe.materials)
            {
                Inventory.i.InWorkshopItem(iso, false);
            }
        }
        workshopData.AdjustRecipeAmount(amount);
        //workshopCraftingController.AdjustCurrentCraftingAmount(amount);
    }

    public void AdjustProductAmountUI(int amount)
    {
        //UI_BLDWorkshop.i.UpdateProductAndRecipeAmount(currentMaterialsArray[0]!=null, currentMaterialsArray[1]!=null, currentMaterialsArray[2]!=null, amount);
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

    public void StartAndSetUpCrafting()
    {
        SO_WorkshopRecipe recipeToCraft = workshop.workshopData.currentWorkshopRecipe;
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
        return currentCraftingAmount;
    }
    
}
