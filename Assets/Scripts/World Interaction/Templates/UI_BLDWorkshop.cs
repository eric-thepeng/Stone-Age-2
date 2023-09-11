using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class UI_BLDWorkshop : MonoBehaviour
{
    [SerializeField] private GameObject uiGameObject = null;
    [SerializeField] private string recipeLocation = "";
    private BLDWorkshop workshop;

    private void Start()
    {
        workshop = GetComponent<BLDWorkshop>();
    }

    public void TurnOnUI()
    {
        uiGameObject.SetActive(true);
        
        //Gather Unlocked Workshop Recipes
        SO_WorkshopRecipe[] allWorkshopRecipes = Resources.LoadAll<SO_WorkshopRecipe>("World Interaction/Workshop Recipes");
        foreach (SO_WorkshopRecipe wr in allWorkshopRecipes)
        {
            print("wr product is: " + wr.product);
        }
    }

    public void TurnOffUI()
    {
        uiGameObject.SetActive(false);
    }

    public void ExitButtonClicked()
    {
        TurnOffUI();
        workshop.ExitUI();
    }
}
