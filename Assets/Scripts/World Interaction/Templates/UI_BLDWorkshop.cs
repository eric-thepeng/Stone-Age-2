using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class UI_BLDWorkshop : MonoBehaviour, IISOReceiver
{
    [SerializeField] private GameObject uiGameObject = null;

    [SerializeField] private UI_ISOIconDisplayBox productISODisplayBox = null;
    [SerializeField] private UI_ISOIconDisplayBox material1ISODisplayBox = null;
    [SerializeField] private UI_ISOIconDisplayBox material2ISODisplayBox = null;
    [SerializeField] private UI_ISOIconDisplayBox material3ISODisplayBox = null;

    [SerializeField] private GameObject startCraftingButton = null;
    
    [SerializeField] private float workshopRecipeDisplayDisplacement;
    [SerializeField] private UI_WorkshopRecipeDisplay workshopRecipeDisplayTemplate;
    [SerializeField] private Transform workshopRecipeDisplayContainer;
    
    private BLDWorkshop workshop;

    static UI_BLDWorkshop instance;
    public static UI_BLDWorkshop i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_BLDWorkshop>();
            }
            return instance;
        }
    }

    public void TurnOnUI(BLDWorkshop orgWorkshop)
    {
        // Turn On UI
        uiGameObject.SetActive(true);
        workshop = orgWorkshop;
        
        // Gather Unlocked Workshop Recipes
        SO_WorkshopRecipe[] allWorkshopRecipes = Resources.LoadAll<SO_WorkshopRecipe>("World Interaction/Workshop Recipes");
        foreach (SO_WorkshopRecipe wr in allWorkshopRecipes)
        {
            print("wr product is: " + wr.product);
        }
        
        // Delete Past Workshop Recipes
        foreach (Transform child in workshopRecipeDisplayContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Display All Workshop Recipes
        workshopRecipeDisplayTemplate.gameObject.SetActive(true);
        for (int i = 0; i < allWorkshopRecipes.Length; i++)
        {
            if(!allWorkshopRecipes[i].AvailableInWorkshops[orgWorkshop.workshopType]) continue;
            UI_WorkshopRecipeDisplay wrd = Instantiate(workshopRecipeDisplayTemplate.gameObject, workshopRecipeDisplayContainer).GetComponent<UI_WorkshopRecipeDisplay>();
            wrd.Display(allWorkshopRecipes[i]);
            wrd.gameObject.transform.localPosition += new Vector3(0,i * workshopRecipeDisplayDisplacement,0);
        }
        workshopRecipeDisplayTemplate.gameObject.SetActive(false);
    }

    public void TurnOffUI()
    {
        UI_FullScreenShading.i.HideShading();
        uiGameObject.SetActive(false);
    }

    public void Button_ExitButtonClicked()
    {
        workshop.ExitUI();
        //TurnOffUIDisplay();
    }

    public void Button_StartCraftingButtonClicked()
    {
        workshop.StartCrafting();
    }

    public void Button_AdjustProductAmountButtonClicked(int amount)
    {
        workshop.AdjustProductAmountClicked(amount);
    }

    #region Interface ISOReceiver

    public void ReceiveISOWithIndex(ItemScriptableObject iso, int index)
    {
        workshop.workshopData.AssignMaterial(index,iso);
    }

    public void CancelISO(int index)
    {
        workshop.workshopData.AssignMaterial(index,null);
    }

    #endregion

    /* The set of data provided to UI_BLDWorkshop to display
 
        - display combination of materials
        - if workshop recipe exist: 
            - display product
            - display amount
 
    */
    public void RefreshUI()
    {
        BLDWorkshop.WorkshopData targetWorkshopData = workshop.workshopData;
        bool recipeExists = workshop.workshopData.currentWorkshopRecipe != null;
        int count = 0;
        foreach (BLDWorkshop.WorkshopData.ISOAndAmount isoAA in workshop.workshopData.materialStat)
        {
            if(count == 0) material1ISODisplayBox.Display(isoAA.iso, false, isoAA.amount, false);
            else if(count == 1) material2ISODisplayBox.Display(isoAA.iso, false, isoAA.amount, false);
            else if(count == 2) material3ISODisplayBox.Display(isoAA.iso, false, isoAA.amount, false);
            count++;
        }

        if (recipeExists)
        {
            productISODisplayBox.Display(targetWorkshopData.productStat.iso, false, targetWorkshopData.productStat.amount, false);
        }
        else
        {
            productISODisplayBox.Display(null, false, default, false);
        }
    }
    public void UpdateProductIcon(ItemScriptableObject iso = null)
    {
        if (iso == null)
        {
            productISODisplayBox.Display(iso,false);
            startCraftingButton.SetActive(false);
        }
        else
        {
            productISODisplayBox.Display(iso,false,0);
            startCraftingButton.SetActive(true);
        }
    }

    /*
    public void UpdateProductAndRecipeAmount(bool material1, bool material2, bool material3, int amount)
    {
        material1ISODisplayBox.DisplayAmount(material1, amount);
        material2ISODisplayBox.DisplayAmount(material2, amount);
        material3ISODisplayBox.DisplayAmount(material3, amount);
        productISODisplayBox.DisplayAmount(true, amount);
    }

    public void ClearProductAndRecipeAmount()
    {
        material1ISODisplayBox.DisplayAmount(false, 0);
        material2ISODisplayBox.DisplayAmount(false, 0);
        material3ISODisplayBox.DisplayAmount(false, 0);
        productISODisplayBox.DisplayAmount(false, 0);
    }

    public void ClearAllMaterialAndProductIcon()
    {
        material1ISODisplayBox.Clear();
        material2ISODisplayBox.Clear();
        material3ISODisplayBox.Clear();
        productISODisplayBox.Clear();
    }*/
}
