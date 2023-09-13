using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class UI_BLDWorkshop : MonoBehaviour
{
    [SerializeField] private GameObject uiGameObject = null;
    [SerializeField] private float workshopRecipeDisplayDisplacement;
    [SerializeField] private UI_WorkshopRecipeDisplay workshopRecipeDisplayTemplate;
    [SerializeField] private Transform workshopRecipeDisplayContainer;
    private BLDWorkshop workshop;

    private void Start()
    {
        workshop = GetComponent<BLDWorkshop>();
    }

    public void TurnOnUI()
    {
        // Turn On UI
        uiGameObject.SetActive(true);
        
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

    public void ExitButtonClicked()
    {
        TurnOffUI();
        workshop.ExitUI();
    }
}
