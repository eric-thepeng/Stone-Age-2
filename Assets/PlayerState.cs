using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerState
{
    public enum State { Browsing, Crafting, Recipe, Building }
    static private State state = State.Browsing;
    static private bool inventoryPanelOpen = false;

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
        }
        else if (enterState == State.Crafting)
        {
            ChangeInventoryPanel(true);
            CraftingManager.i.OpenPanel();
        }
        else if (enterState == State.Recipe)
        {
            ChangeInventoryPanel(false);
            RecipeMapManager.i.OpenPanel();
        }
        else if (enterState == State.Building)
        {
            ChangeInventoryPanel(true);
            BuildingManager.i.OpenBuilding();
        }

        state = enterState;
    }



    static void ChangeInventoryPanel(bool changeTo)
    {
        //TODO: change inventory panel
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

    public static void OpenCloseChangeInventoryPanel()
    {
        if (state != State.Browsing) return;
        ChangeInventoryPanel(!inventoryPanelOpen);
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
