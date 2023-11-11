using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BLDWorkshop : BuildingInteractable
{
    enum State
    {
        Idle, Assigning
    }

    private State state = State.Idle;

    private bool isWorking = false;
    
    [SerializeField] private GameObject workshopCraftingControllerUI = null;
    private WorkshopCraftingController workshopCraftingController;

    public SO_WorkshopRecipe.WorkshopType workshopType;

    public WorkshopData workshopData;

    private void Awake()
    {
        workshopCraftingController = new WorkshopCraftingController(this, workshopCraftingControllerUI);
        workshopData = new WorkshopData(this);
    }

    private void Start()
    {
        currentInteraction = new InteractionType(InteractionType.TypeName.Click, ClickEvent);
    }

    private void Update()
    {
        workshopCraftingController.UpdateCraftingUI(Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.L))
        {
            print(workshopData.GetStatusInString());
        }
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
            PlayerState.OpenCloseAllocatingBackpack(false);
            if (workshopCraftingController.isCrafting)
            {
                
            }
            else
            {
                ClearAllMaterialAndProduct();
            }
        }
        else
        {
            Debug.LogError("Exit workshop UI while UI is not opened?");
        }
    }
    
    #endregion

    private void ClearAllMaterialAndProduct()
    {
        foreach (var isoaa in workshopData.materialStat)
        {
            for (int i = 0; i < isoaa.amount; i++)
            {
                if(isoaa.iso != null) Inventory.i.InWorkshopItem(isoaa.iso, false);
            }
        }
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
        workshopData.AssignRecipe(null);
    }

    public void StartCrafting()
    {
        workshopCraftingController.StartAndSetUpCrafting();
        ExitUI(); //put this after workshopCraftingController.StartAndSetUpCrafting to avoid currentRecipe reset
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

}

public class WorkshopCraftingController
{
    // dependencies
    private BLDWorkshop workshop;
    private SO_WorkshopRecipe craftingWR;
    public bool isCrafting;
    private GameObject ui;
    private UI_ISOIconDisplayBox productDisplayBox;
    private Slider circularSlider;

    // crafting calculations
    private float currentCraftingTime;
    
    private int craftingAmountFinished;

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
        circularSlider = ui.GetComponentInChildren<Slider>();
        isCrafting = true;
        productDisplayBox.Display(craftingWR.product, false, 0,false);

        craftingAmountFinished = 0;
        currentCraftingTime = 0;
    }
    
    public void ExitCrafting()
    {
        isCrafting = false;
        HideCraftingUI();
    }
    
    public void UpdateCraftingUI(float deltaTime)
    {
        if(!isCrafting) return;
        if(GetCraftingAmountLeft() == 0) ExitCrafting();
        currentCraftingTime += deltaTime;
        circularSlider.value = currentCraftingTime / craftingWR.workTime;
        if (currentCraftingTime >= craftingWR.workTime)
        {
            currentCraftingTime = 0;
            SpawnProduct();
        }
    }

    public int GetCraftingAmountLeft()
    {
        return workshop.workshopData.currentWorkshopRecipeAmount;
    }

    public void ShowCraftingUI()
    {
        ui.SetActive(true);
    }
    

    public void HideCraftingUI()
    {
        productDisplayBox.Display(null, false, default, false);
        ui.SetActive(false);
    }

    public void SpawnProduct()
    {
        craftingAmountFinished += 1;
        workshop.workshopData.FinishOneProduction();
        productDisplayBox.Display(craftingWR.product, false, craftingAmountFinished,false);
        Inventory.i.AddInventoryItem(craftingWR.product);
    }


}

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
        if (currentWorkshopRecipe == null)
        {
            productStat.iso = null;
        }
        else
        {
            productStat.iso = wr.product;
        }
        productStat.amount = 0;
        currentWorkshopRecipeAmount = 0;
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
        //INCREASE RECIPE AMOUNT
        if (amount == 1) 
        {
        }
        //REDUCE RECIPE AMOUNT
        else
        {
            if(currentWorkshopRecipeAmount == 0) return;
        }
        
        foreach (var materialISOAA in materialStat)
        {
            materialISOAA.amount += amount;
        }
        currentWorkshopRecipeAmount += amount;
        productStat.amount += amount;

        UI_BLDWorkshop.i.RefreshUI();
    }

    public ItemScriptableObject[] GetCurrentMaterialArray()
    {
        return new ItemScriptableObject[] {materialStat[0].iso,materialStat[1].iso,materialStat[2].iso};
    }

    public void FinishOneProduction()
    {
        AdjustRecipeAmount(-1);
    }

    public string GetStatusInString()
    {
        string output = "";

        for (int i = 0; i < 3; i++)
        {
            output += "Material " + (i + 1) + " : " + (materialStat[i].iso == null
                ? "null" : materialStat[i].iso.tetrisHoverName);
            if (materialStat[i].iso != null)
            {
                output += " amount: " +materialStat[i].amount;
            }

            output += "\n";
        }
        
        output += "Product : " + (productStat.iso == null
                            ? "null" : productStat.iso.tetrisHoverName) + "\n";

        output += "Current Recipe: " + currentWorkshopRecipe + " \n";
        
        output += "RecipeAmount: " + currentWorkshopRecipeAmount;
        
        return output;
    } 
}

