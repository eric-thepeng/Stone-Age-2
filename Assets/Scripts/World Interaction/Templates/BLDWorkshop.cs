using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLDWorkshop : WorldInteractable
{
    private UI_BLDWorkshop ui;

    enum State
    {Idle, Assigning
    }

    private State state = State.Idle;

    private bool isWorking = false;

    private ItemScriptableObject[] currentMaterialsArray = new ItemScriptableObject[3]{null, null, null};

    private ItemScriptableObject currentProduct = null;

    private void Start()
    {
        ui = GetComponent<UI_BLDWorkshop>();
    }

    #region interaction logic
    
    protected override void BeginMousePress()
    {
        if (state == State.Idle)
        {
            ui.TurnOnUI();
            state = State.Assigning;
            PlayerState.OpenCloseAllocatingBackpack(true);
            CameraManager.i.MoveToDisplayLocation(transform.position + new Vector3(0,0,15), 100f);
        }
        base.BeginMousePress();
    }

    protected override void TurnOnHighlight()
    {
        if(state == State.Assigning) return;
        base.TurnOnHighlight();
    }


    /*
    protected override void EndMouseHover()
    {
        if (state == State.Assigning)
        {
            ui.TurnOffUI();
            state = State.Idle;
        }
        base.EndMouseHover();
    }*/

    public void ExitUI()
    {
        if (state == State.Assigning)
        {
            ui.TurnOffUI();
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
        currentProduct = null;
        ui.ClearAllMaterialAndProductIcon();
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
                UpdateProduct(wr.product);
                return;
            }
        }
        UpdateProduct(null);
    }

    private void UpdateProduct(ItemScriptableObject iso = null)
    {
        ui.UpdateProductIcon(iso);
        currentProduct = iso;
    }

    public void StartCrafting()
    {
        ExitUI();
    }
    
}
