using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLDFoodStorage : BuildingInteractable
{
    enum State
    {
        Idle, Assigning
    }

    private State state = State.Idle;

    private bool isWorking = false;
    
    // [SerializeField] private GameObject workshopCraftingControllerUI = null;
    // private WorkshopCraftingController workshopCraftingController;

    // public SO_WorkshopRecipe.WorkshopType workshopType;

    public WorkshopData workshopData;

    public float harvestLongPressTime = 1.5f;

    private void Awake()
    {
        // workshopCraftingController = new WorkshopCraftingController(this, workshopCraftingControllerUI);
        // workshopData = new WorkshopData(this);
    }

    private void Start()
    {
        SetInteractionActionToOpen();
    }

    private void Update()
    {
        // workshopCraftingController.UpdateCrafting(Time.deltaTime);
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
        ChangeGlobalAllowInteraction(false);
        UI_BLDFoodStorage.i.TurnOnUI(this);
        state = State.Assigning;
        PlayerState.OpenCloseAllocatingBackpack(true);
        CameraManager.i.MoveToDisplayLocation(transform.position + new Vector3(0,0,5), 65f);
        allowInteraction = false;
        // workshopCraftingController.PauseCrafting();
    }

    public void ExitUI()
    {
        if (state == State.Assigning)
        {
            ChangeGlobalAllowInteraction(true);

            SetInteractionActionToOpen();
            
            UI_BLDFoodStorage.i.TurnOffUI();
            state = State.Idle;
            PlayerState.OpenCloseAllocatingBackpack(false);
            // if (workshopCraftingController.isCrafting)
            // {
            //     
            // }
            // else
            // {
            //     ClearAllMaterialAndProduct();
            // }

            allowInteraction = true;
            // workshopCraftingController.ResumeCrafting();

        }
        else
        {
            Debug.LogError("Exit workshop UI while UI is not opened?");
        }
    }
    
    #endregion

    // private void ClearAllMaterialAndProduct()
    // {
    //     foreach (var isoaa in workshopData.materialStat)
    //     {
    //         for (int i = 0; i < isoaa.amount; i++)
    //         {
    //             if(isoaa.iso != null) Inventory.i.InWorkshopItem(isoaa.iso, false);
    //         }
    //     }
    //     workshopData.ResetAll();
    // }
    //
    //implemented
    // public void CheckIfRecipeExists()
    // {
    //     SO_WorkshopRecipe[] allWorkshopRecipes = Resources.LoadAll<SO_WorkshopRecipe>("World Interaction/Workshop Recipes");
    //     foreach (SO_WorkshopRecipe wr in allWorkshopRecipes)
    //     {
    //         if(!wr.AvailableInWorkshops[workshopType])continue;
    //         //Check if there is a match.
    //         if (wr.CheckMaterialMatch(workshopData.GetCurrentMaterialArray()))
    //         {
    //             workshopData.AssignRecipe(wr);
    //             return;
    //         }
    //     }
    //     workshopData.AssignRecipe(null);
    // }

    // public void StartCrafting()
    // {
    //     if (workshopCraftingController.isCrafting && workshopCraftingController.isCraftingPaused)
    //     {
    //         workshopCraftingController.ResumeCrafting();
    //     }
    //     else
    //     {
    //         workshopCraftingController.StartAndSetUpCrafting();
    //     }
    //     ExitUI(); //put this after workshopCraftingController.StartAndSetUpCrafting to avoid currentRecipe reset
    // }
    
    // /// <summary>
    // /// 
    // /// </summary>
    // /// <param name="amount"> +1 or -1 </param>
    // public void AdjustProductAmountClicked(int amount)
    // {
    //     if (amount > 0) // put iso from inventory into workshop, return if not sufficient
    //     {
    //         foreach (ItemScriptableObject iso in workshopData.currentWorkshopRecipe.materials)
    //         {
    //             if (Inventory.i.GetISOInstockAmount(iso) <= 0) return;
    //         }
    //         foreach (ItemScriptableObject iso in workshopData.currentWorkshopRecipe.materials)
    //         {
    //             Inventory.i.InWorkshopItem(iso, true);
    //         }
    //     }
    //     else // putting iso into inventory
    //     {
    //         if(workshopData.currentWorkshopRecipeAmount <= 0) return;
    //         foreach (ItemScriptableObject iso in workshopData.currentWorkshopRecipe.materials)
    //         {
    //             Inventory.i.InWorkshopItem(iso, false);
    //         }
    //     }
    //     workshopData.AdjustRecipeAmount(amount);
    //     // if(workshopData.currentWorkshopRecipeAmount == 0) workshopCraftingController.CancelCrafting();
    // }

    public void SetInteractionActionToOpen()
    {
        SetCurrentInteraction(new InteractionType(InteractionType.TypeName.Click, ClickEvent));
    }
    
    // public void SetInteractionActionToHarvest()
    // {
    //     SetCurrentInteraction(new InteractionType(InteractionType.TypeName.LongPress, workshopCraftingController.HarvestProduct, harvestLongPressTime));
    // }

}
