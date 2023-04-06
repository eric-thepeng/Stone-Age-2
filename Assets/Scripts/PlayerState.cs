using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PlayerState
{
    public enum State { Browsing, Crafting, Recipe, Building }
    static private State state = State.Browsing;
    static private bool inventoryPanelOpen = false;
    private enum RecipeViewerState {Open, Hide, Close } //Open: open   Close: hide   Hide: open a little bit
    static private RecipeViewerState recipeViewerPanelState = RecipeViewerState.Close;


    public static void ExitState()
    {
        if(state == State.Browsing)
        {
            
        }
        else if (state == State.Crafting)
        {
            CraftingManager.i.ClosePanel();
        }
        else if (state == State.Recipe)
        {
            RecipeMapManager.i.ClosePanel();
        }
        else if (state == State.Building)
        {
            BuildingManager.i.CloseBuilding();
        }
    }

    public static void EnterState(State enterState)
    {
        if (enterState == State.Browsing)
        {
            ChangeInventoryPanel(false);
            ChangeRecipeViewerPanel(RecipeViewerState.Close);
            PanelButtonIndicator.i.MoveToHome();
        }
        else if (enterState == State.Crafting)
        {
            if(state == State.Browsing)
            {
                ChangeRecipeViewerPanel(RecipeViewerState.Hide);
            }
            ChangeInventoryPanel(true);
            CraftingManager.i.OpenPanel();
            PanelButtonIndicator.i.MoveToCrafting();
        }
        else if (enterState == State.Recipe)
        {
            ChangeInventoryPanel(false);
            RecipeMapManager.i.OpenPanel();
            ChangeRecipeViewerPanel(RecipeViewerState.Open);
            PanelButtonIndicator.i.MoveToBlueprintMap();

        }
        else if (enterState == State.Building)
        {
            ChangeInventoryPanel(true);
            BuildingManager.i.OpenBuilding();
            PanelButtonIndicator.i.MoveToHomeBuilding();

        }

        state = enterState;
    }





    static void ChangeInventoryPanel(bool changeTo)
    {
        if (changeTo == inventoryPanelOpen) return;
        if (changeTo)
        {
            UI_InventoryPanel.i.OpenPanel();
        }
        else
        {
            UI_InventoryPanel.i.ClosePanel();
        }
        inventoryPanelOpen = changeTo;
    }

    static void ChangeRecipeViewerPanel(RecipeViewerState changeTo)
    {
        if (changeTo == recipeViewerPanelState) return;
        if (changeTo == RecipeViewerState.Open)
        {
            RecipeViewerManager.i.OpenPanel();
        }
        else if (changeTo == RecipeViewerState.Close)
        {
            RecipeViewerManager.i.ClosePanel();
        }
        else if (changeTo == RecipeViewerState.Hide)
        {
            RecipeViewerManager.i.HidePanel();
        }

        recipeViewerPanelState= changeTo;
    }




    public static void OpenCloseChangeInventoryPanel()
    {
        if (!IsBrowsing()) return;
        ChangeInventoryPanel(!inventoryPanelOpen);
    }

    public static void OpenCloseChangeRecipeViewerPanel()
    {
        if (IsCrafting() || IsRecipe())
        {
            if(recipeViewerPanelState == RecipeViewerState.Open)
            {
                ChangeRecipeViewerPanel(RecipeViewerState.Hide);
            }
            else if (recipeViewerPanelState == RecipeViewerState.Hide)
            {
                ChangeRecipeViewerPanel(RecipeViewerState.Open);
            }
        }
    }

    public static void OpenCloseCraftingPanel()
    {
        if(state == State.Crafting) //close
        {
            ExitState();
            EnterState(State.Browsing);
        }
        else //open
        {
            ExitState();
            EnterState(State.Crafting);
        }
    }

    public static void OpenCloseRecipePanel()
    {
        if (state == State.Recipe)
        {
            ExitState();
            EnterState(State.Browsing);
        }
        else
        {
            ExitState();
            EnterState(State.Recipe);
        }
    }

    public static void OpenCloseBuildingPanel()
    {
        if (state == State.Building)
        {
            ExitState();
            EnterState(State.Browsing);
        }
        else
        {
            ExitState();
            EnterState(State.Building);
        }
    }

    public static void HomeReturnButton()
    {
        if(state != State.Browsing)
        {
            ExitState();
            EnterState(State.Browsing);
        }
    }




    public static bool IsBrowsing()
    {
        return state == State.Browsing;
    }

    public static bool IsCrafting()
    {
        return state == State.Crafting;
    }

    public static bool IsRecipe()
    {
        return state == State.Recipe;
    }

    public static bool IsBuilding()
    {
        return state == State.Building;
    }


}
