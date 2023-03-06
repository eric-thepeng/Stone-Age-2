using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerState
{
    public enum State { Browsing, Crafting, Recipe, Building }
    static private State state;
    static private bool inventoryPanelOpen;

    public static void ExitState()
    {
        if(state == State.Browsing)
        {
            
        }
        else if (state == State.Crafting)
        {

        }
        else if (state == State.Recipe)
        {

        }
        else if (state == State.Building)
        {

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
        }
        else if (enterState == State.Recipe)
        {
            ChangeInventoryPanel(false);
        }
        else if (enterState == State.Building)
        {
            ChangeInventoryPanel(true);
        }

        state = enterState;
    }



    static void ChangeInventoryPanel(bool changeTo)
    {
        //TODO: change inventory panel
        inventoryPanelOpen = changeTo;
    }

    public static void OpenCloseChangeInventoryPanel()
    {
        if (state != State.Browsing) return;
        ChangeInventoryPanel(!inventoryPanelOpen);
    }

    public static void OpenCloseCraftingPanel()
    {
        if(state == State.Crafting)
        {
            ExitState();
            EnterState(State.Browsing);
        }
        else
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
