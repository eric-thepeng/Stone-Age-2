using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WorkshopRecipeDisplay : MonoBehaviour
{
    [Header("DO NOT EDIT ANYTHING - Internal Dependencies")]
    [SerializeField] private GameObject lockedComp;
    [SerializeField] private GameObject unlockedComp;
    [SerializeField] private GameObject bothComp;
    [Header("UnlockedComp Dependencies")]
    [SerializeField] private GameObject m1;
    [SerializeField] private GameObject m2;
    [SerializeField] private GameObject m3;
    [SerializeField] private GameObject product;
    [SerializeField] private GameObject p1;
    [SerializeField] private GameObject p2;
    
    //This ui is hard-coded, bite me ~_~
    
    public void Display(SO_WorkshopRecipe workshopRecipe)
    {
        bothComp.SetActive(true);
        if (workshopRecipe.unlocked)
        {
            unlockedComp.SetActive(true);
            
            m2.SetActive(false);
            m3.SetActive(false);
            p1.SetActive(false);
            p2.SetActive(false);
            
            //display m length = 1
            m1.GetComponent<SpriteRenderer>().sprite = workshopRecipe.materials[0].iconSprite;
            product.GetComponent<SpriteRenderer>().sprite = workshopRecipe.product.iconSprite;
            if(workshopRecipe.materials.Count == 1) return;
            //display m length = 2
            m2.GetComponent<SpriteRenderer>().sprite = workshopRecipe.materials[1].iconSprite;
            m2.SetActive(true);
            p1.SetActive(true);
            if(workshopRecipe.materials.Count == 2) return;
            //display m length = 3
            m3.GetComponent<SpriteRenderer>().sprite = workshopRecipe.materials[2].iconSprite;
            m3.SetActive(true);
            p2.SetActive(true);
        }
        else
        {
            lockedComp.SetActive(true);
        }
    }

    public void Hide()
    {
        bothComp.SetActive(false);
        unlockedComp.SetActive(false);
        lockedComp.SetActive(false);
    }
}
