using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlueprintCard : MonoBehaviour
{
    [SerializeField] private SpriteRenderer productSpriteRenderer;
    [SerializeField] private TextMeshPro productNameTMP;
    [SerializeField] private Transform materialDisplayTemplate;
    [SerializeField] private GameObject researchFinishedIcon;
    [SerializeField] private float materialYDelta;
    
    
    private ItemCraftScriptableObject myICSO;

    public void SetUpCardInfo(ItemCraftScriptableObject icso)
    {
        myICSO = icso;
        productSpriteRenderer.sprite = myICSO.ItemCrafted.iconSprite;
        productNameTMP.text = myICSO.ItemCrafted.tetrisHoverName;

        int count = 0;
        foreach (var VARIABLE in icso.GetRecipeComposition())
        {
            //Create new material display game object
            GameObject materialDisplay = Instantiate(materialDisplayTemplate.gameObject, transform);
            materialDisplay.SetActive(true);
            materialDisplay.transform.localPosition = materialDisplayTemplate.transform.localPosition +
                                                      count * new Vector3(0, materialYDelta, 0);
            
            //Set up visual
            materialDisplay.transform.Find("Material Sprite").GetComponent<SpriteRenderer>().sprite =
                VARIABLE.Key.iconSprite;
            materialDisplay.transform.Find("Material Amount").GetComponent<TextMeshPro>().text =
                "" + VARIABLE.Value;

            count++;
        }
        
        CraftingManager.i.NewItemCrafted.AddListener(FinishResearch);
    }

    public void FinishResearch(ItemScriptableObject iso)
    {
        if (iso == myICSO.ItemCrafted)
        {
            researchFinishedIcon.SetActive(true);
            CraftingManager.i.NewItemCrafted.RemoveListener(FinishResearch);
        }
    }
}
