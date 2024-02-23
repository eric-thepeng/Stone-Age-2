using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * 一个统一的地方接收button input并执行
 */
public class PlayerInputChannel : MonoBehaviour
{
    static PlayerInputChannel instance = null;
    public static PlayerInputChannel i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerInputChannel>();
            }
            return instance;
        }
    }
    public enum WorldButtons { Crafting,HomeReturn, Building, Inventory, RecipeViewer, CameraBackHome, ExploreMap }

    public delegate void OnPlayerPressWorldButton(WorldButtons wb);
    public static event OnPlayerPressWorldButton onPlayerPressWorldButton;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) CraftingPanelOpenButton();
        if (Input.GetKeyDown(KeyCode.Space)) HomeReturnButton();
        if (Input.GetKeyDown(KeyCode.B)) InventoryPanelOpenButton();
        //if (Input.GetKeyDown(KeyCode.R)) RecipeMapOpenButton();
        if (Input.GetKeyDown(KeyCode.M)) ExploreMapButton();
        if (Input.GetKeyDown(KeyCode.Q)) RotateLeftButton();
        if (Input.GetKeyDown(KeyCode.Q)) RotateRightButton();

    }

    public static void GoToPanel(PlayerState.GamePanel targetGamePanel)
    {
        switch (targetGamePanel)
        {
            case PlayerState.GamePanel.Home:
                HomeReturnButton();
                break;
            case PlayerState.GamePanel.Crafting:
                CraftingPanelOpenButton();
                break;
            case PlayerState.GamePanel.ExploreMap:
                ExploreMapButton();
                break;
            case PlayerState.GamePanel.Inventory:
                InventoryPanelOpenButton();
                break;
        }
    }

    public static void InventoryPanelOpenButton()
    {
        if(onPlayerPressWorldButton != null)onPlayerPressWorldButton(WorldButtons.Inventory);
        PlayerState.OpenCloseChangeInventoryPanel();
    }

    public static void CraftingPanelOpenButton()
    {
        if (onPlayerPressWorldButton != null) onPlayerPressWorldButton(WorldButtons.Crafting);
        PlayerState.OpenCloseCraftingPanel();
    }

    /*
    public static void RecipeMapOpenButton()
    {
        if (onPlayerPressWorldButton != null) onPlayerPressWorldButton(WorldButtons.Research);
        PlayerState.OpenCloseRecipePanel();
    }*/

    public static void BuildingSystemOpenButton()
    {
        if (onPlayerPressWorldButton != null) onPlayerPressWorldButton(WorldButtons.Building);
        PlayerState.OpenCloseBuildingPanel();
    }

    /*
    public static void RecipeViewerPanelOpenButton()
    {
        if (onPlayerPressWorldButton != null) onPlayerPressWorldButton(WorldButtons.RecipeViewer);
        PlayerState.OpenCloseChangeRecipeViewerPanel();
    }*/

    public static void HomeReturnButton()
    {
        if (onPlayerPressWorldButton != null) onPlayerPressWorldButton(WorldButtons.HomeReturn);
        PlayerState.HomeReturnButton();
    }

    public static void CameraBackHomeButton()
    {
        if (onPlayerPressWorldButton != null) onPlayerPressWorldButton(WorldButtons.CameraBackHome);
        CameraManager.i.MoveBackToHome();
    }

    public static void ExploreMapButton()
    {
        if (onPlayerPressWorldButton != null) onPlayerPressWorldButton(WorldButtons.ExploreMap);
        PlayerState.ExploreMapButton();
    }

    public static void RotateLeftButton()
    {
        
    }

    public static void RotateRightButton()
    {
        
    }
    
    public Vector2Int GetKeyBoardInputDirection()
    {
        Vector2Int ip = new Vector2Int(0,0);
        if (Input.GetKey(KeyCode.W)) ip.y += 1;
        if (Input.GetKey(KeyCode.S)) ip.y -= 1;
        if (Input.GetKey(KeyCode.A)) ip.x -= 1;
        if (Input.GetKey(KeyCode.D)) ip.x += 1;
        return ip;
    }
}
