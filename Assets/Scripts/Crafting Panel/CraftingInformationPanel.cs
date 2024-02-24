using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CraftingInformationPanel : MonoBehaviour
{
    class ShadowBoxManager
    {
        private Transform parentGO;
        private Transform boxTemplate;
        private List<Transform> allBoxes;
        private float distance = 0.21f;
        public ShadowBoxManager(Transform parent)
        {
            parentGO = parent;
            boxTemplate = parent.transform.GetChild(0);
            allBoxes = new List<Transform>();
        }
        public void GenerateBoxes(List<Vector2> coords)
        {
            HideBoxes();
            boxTemplate.gameObject.SetActive(true);
            foreach (Vector2 c in coords)
            {
                GameObject newGO = Instantiate(boxTemplate.gameObject, parentGO);
                newGO.transform.localPosition += new Vector3(c.x * distance, -c.y * distance, 0f);
                allBoxes.Add(newGO.transform);
            }
            boxTemplate.gameObject.SetActive(false);
        }

        public void HideBoxes()
        {
            for (int i = allBoxes.Count - 1; i >= 0; i--)
            {
                Destroy(allBoxes[i].gameObject);
            }
            allBoxes.Clear();
        }
        
    }
    
    // Serialized Private Variables
    [Header("Dependencies"),SerializeField] private GameObject shadowBoxManagerGameObject;
    [SerializeField] private TextMeshPro isoNameTMP;
    [SerializeField] private Transform researchedTetrisParent;
    
    [Header("Material Display"),SerializeField] private float materialYDelta;
    [SerializeField] private ResourceSetDisplayer researchedMaterialResourceSetDisplayer;
    [SerializeField] private ResourceSetDisplayer unresearchedMaterialResourceSetDisplayer;

    [Header("Researched / Not"), SerializeField] private Transform researchedDisplay;
    [SerializeField]private Transform notResearchedDisplay;
    
    // Private Variables
    private ShadowBoxManager shadowBoxManager;
    private List<GameObject> currentDisplayingMaterial = new List<GameObject>();
    private GameObject researchedTetrisGO;
    private CraftingInformationPanelProduction cipProduction;

    private void Awake()
    {
        cipProduction = GetComponent<CraftingInformationPanelProduction>();
    }

    private void Start()
    {
        shadowBoxManager = new ShadowBoxManager(shadowBoxManagerGameObject.transform);
    }

    public void DisplayBlueprintCard(BlueprintCard blueprintCard)
    {
        // Display according to researched
        if (blueprintCard.GetICSO().IsResearched())
        {
            researchedDisplay.localPosition = new Vector3(0, 0, 0);
            notResearchedDisplay.localPosition = new Vector3(-30, 0, 0);
            
            //Set Up cipProduction
            cipProduction.SetUpToActive(blueprintCard);
        }
        else
        {
            researchedDisplay.localPosition = new Vector3(-30, 0, 0);
            notResearchedDisplay.localPosition = new Vector3(0, 0, 0);
        }
        
        // Update ISO Name
        isoNameTMP.text = blueprintCard.GetICSO().ItemCrafted.tetrisHoverName;
        
        // Display researched tetris
        Destroy(researchedTetrisGO);
        researchedTetrisGO = CraftingManager.i.CreateTetris(blueprintCard.GetICSO().ItemCrafted, new Vector3(0, 0, 0),
            CraftingManager.CreateFrom.VISUAL_ONLY);
        researchedTetrisGO.transform.SetParent(researchedTetrisParent);
        researchedTetrisGO.transform.localPosition = new Vector3(0, 0, 0);
        researchedTetrisGO.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
        
        // Display shadow box for recipe
        shadowBoxManager.HideBoxes();
        shadowBoxManager.GenerateBoxes(blueprintCard.GetICSO().GetDefaultRecipeCoords());
        
        // Display material list
        unresearchedMaterialResourceSetDisplayer.Display(blueprintCard.GetICSO().GetResourceSet());
        researchedMaterialResourceSetDisplayer.Display(blueprintCard.GetICSO().GetResourceSet());
            
        /* Old Material Display
            for (int i = currentDisplayingMaterial.Count-1; i >= 0 ; i--)
            {
                Destroy(currentDisplayingMaterial[i]);
            }
            currentDisplayingMaterial = new List<GameObject>();

            int count = 0;
            foreach (var VARIABLE in blueprintCard.GetICSO().GetRecipeComposition())
            {
                //Create new material display game object
                GameObject materialDisplay = Instantiate(materialDisplayTemplate.gameObject, materialDisplayContainer);
                currentDisplayingMaterial.Add(materialDisplay);
                materialDisplay.SetActive(true);
                materialDisplay.transform.localPosition = materialDisplayTemplate.transform.localPosition +
                                                          count * new Vector3(0, materialYDelta, 0);
            
                //Set up visual
                materialDisplay.transform.Find("Material Sprite").GetComponent<SpriteRenderer>().sprite =
                    VARIABLE.Key.iconSprite;
                materialDisplay.transform.Find("Material Amount").GetComponent<TextMeshPro>().text =
                    "" + VARIABLE.Value;

                count++;
            }*/
    }
}

