using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) CraftingPanelOpenButton();
        if (Input.GetKeyDown(KeyCode.Space)) HomeReturnButton();
        if (Input.GetKeyDown(KeyCode.B)) InventoryPanelOpenButton();
        if (Input.GetKeyDown(KeyCode.R)) RecipeMapOpenButton();
    }

    public static void InventoryPanelOpenButton()
    {
        PlayerState.OpenCloseChangeInventoryPanel();
    }

    public static void CraftingPanelOpenButton()
    {
        PlayerState.OpenCloseCraftingPanel();
    }

    public static void RecipeMapOpenButton()
    {
        PlayerState.OpenCloseRecipePanel();
    }

    public static void BuildingSystemOpenButton()
    {
        PlayerState.OpenCloseBuildingPanel();
    }

    public static void RecipeViewerPanelOpenButton()
    {
        PlayerState.OpenCloseChangeRecipeViewerPanel();
    }

    public static void HomeReturnButton()
    {
        PlayerState.HomeReturnButton();
    }

    public static void CameraBackHomeButton()
    {
        CameraManager.i.MoveBackToHome();
    }
}
