using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

/* NOTES Oct 1 2023 Peng Guo

Overview：
PlayerState 这个class用于统一管理玩家正在交互的state。Specifically，进行一下两件事情：
1. 提供统一的function进入这些state
2. 统一管理在进入和退出state的时候游戏内的变化。（比如说在进入哪些state的时候自动打开inventory panel，这样程序加入新的state的时候更加轻松）

State Explanation：
Browsing - 玩家在家园界面的默认状态
BlueprintAndResearch - 玩家在Research Panel使用Tetris研究物品 OR - 玩家在Blueprint界面看Blueprint/制作东西
Building - 玩家在家园界面的建筑状态
AllocatingBackpack - 玩家在家园界面，但是在一种特殊的交互当中。在这个状态下玩家要使用背包里的东西并且assign他们。例如在crafting station制作的时候或者是未来给小动物味东西的时候。
ExploreMap - 玩家在Explore Map界面
*/


public static class PlayerState
{
    public enum State { Browsing, BlueprintAndResearch, Building, AllocatingBackpack, ExploreMap }
    static public State state = State.Browsing;
    static private bool inventoryPanelOpen = false;
    static private bool craftingTableOpen = false;
    private enum RecipeViewerState {Open, Hide, Close } //Open: open   Close: hide   Hide: open a little bit
    static private RecipeViewerState recipeViewerPanelState = RecipeViewerState.Close;


    public static void ExitState()
    {
        if(state == State.Browsing)
        {
            
        }
        else if (state == State.BlueprintAndResearch)
        {
            CraftingManager.i.ClosePanel();
            BlueprintAndResearchManager.i.ClosePanel();
        }
        else if (state == State.Building)
        {
            BuildingManager.i.CloseModifyMode();
            BuildingManager.i.CloseBuildingMode();
            BuildingManager.i.gridOperationManager.GetComponent<GridOperationManager>().EndPaintMode();
        }
        else if (state == State.AllocatingBackpack)
        {
            
        }else if (state == State.ExploreMap)
        {
            ExploreMapPanel.i.ClosePanel();
        }
    }

    public static void EnterState(State enterState)
    {
        if (enterState == State.Browsing)
        {
            ChangeInventoryPanel(false);
            ChangeRecipeViewerPanel(RecipeViewerState.Close);
            PanelButtonIndicator.i.Exit();
        }
        else if (enterState == State.BlueprintAndResearch)
        {
            ChangeInventoryPanel(true);
            CraftingManager.i.OpenPanel();
            BlueprintAndResearchManager.i.OpenPanel();
            PanelButtonIndicator.i.EnterCrafting();
            
            /*Old Tech Tree Code
            ChangeInventoryPanel(false);
            RecipeMapManager.i.OpenPanel();
            ChangeRecipeViewerPanel(RecipeViewerState.Open);
            PanelButtonIndicator.i.EnterResearch();*/
        }
        else if (enterState == State.Building)
        {
            ChangeInventoryPanel(true);
            BuildingManager.i.OpenBuildingMode  ();
        }
        else if (enterState == State.AllocatingBackpack)
        {
            ChangeInventoryPanel(true);
        }else if (enterState == State.ExploreMap)
        {
            ExploreMapPanel.i.OpenPanel();
            ChangeRecipeViewerPanel(RecipeViewerState.Close);
        }

        state = enterState;
    }


    public static void ChangeInventoryPanel(bool changeTo)
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

    
    public static void OpenCloseAllocatingBackpack(bool open)
    {
        if (open) //open
        {
            if (state != State.AllocatingBackpack)
            {
                ExitState();
                EnterState(State.AllocatingBackpack);
            }
        }
        else 
        {
            if (state == State.AllocatingBackpack)
            {
                ExitState();
                EnterState(State.Browsing);
            }
        }
    }

    public static bool isInventoryPanelOpen()
    {
        return inventoryPanelOpen;
    }

    public static void OpenCloseChangeInventoryPanel()
    {
        Debug.Log("Is browsing:" + IsBrowsing() + ",  inventory panel open:" + inventoryPanelOpen);
        if (!IsBrowsing() && !IsBuilding()) return;
        ChangeInventoryPanel(!inventoryPanelOpen);
    }

    /*
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
    }*/

    public static void OpenCloseCraftingPanel()
    {
        if(state == State.BlueprintAndResearch) //close
        {
            ExitState();
            EnterState(State.Browsing);
        }
        else //open
        {
            ExitState();
            EnterState(State.BlueprintAndResearch);
        }
    }

    /*
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
    }*/

    public static void OpenCloseBuildingPanel()
    {
        if (state == State.Building)
        {
            ExitState();
            EnterState(State.Browsing);
            //ChangeInventoryPanel(true);
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

    public static void ExploreMapButton()
    {
        if (state == State.ExploreMap)
        {
            ExitState();
            EnterState(State.Browsing);
        }else if (state != State.AllocatingBackpack)
        {
            ExitState();
            EnterState(State.ExploreMap);
        }
    }

    #region Determines Current State

    public static bool IsBrowsing()
    {
        return state == State.Browsing;
    }

    public static bool IsBlueprintAndResearch()
    {
        return state == State.BlueprintAndResearch;
    }

    public static bool IsBlueprint()
    {
        return BlueprintAndResearchManager.i.IsBlueprintPanelOpen();
    }

    public static bool IsResearch()
    {
        return BlueprintAndResearchManager.i.IsResearchPanelOpen();
    }
    
    public static bool IsBuilding()
    {
        return state == State.Building;
    }

    public static bool IsAllocatingBackpack()
    {
        return state == State.AllocatingBackpack;
    }

    public static bool IsExploreMap()
    {
        return state == State.ExploreMap;
    }

    #endregion


}
