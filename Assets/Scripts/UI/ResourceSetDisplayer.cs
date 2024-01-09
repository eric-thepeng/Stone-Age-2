using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Sirenix.Serialization;
using UnityEngine;
using TMPro;

/// <summary>
/// Create and display ResourceSet in worldspace
/// </summary>
public class ResourceSetDisplayer : MonoBehaviour
{
    [Header("------EDIT WHAT TO DISPLAY------")]
    [Header("Display a preset ResourceSet")]
    [SerializeField,  Tooltip("Resource set to display")] ResourceSet resourceSetToDisplay = null;

    [Header("OR Display a ResourceSet from IResourceSetProvider")] [SerializeField]
    private bool displayFromResourceSetProvider = false;
    [SerializeField] private MonoBehaviour resourceSetProvider = null;
    [SerializeField] private int resourceSetProviderIndex = 0;

    
    [Header("------EDIT VARIABLES------")]
    [SerializeField, Tooltip("Does it display the spirit points and amount section?")] private bool displaySpiritPoints = true;
    [SerializeField, Tooltip("Does it display the resource and amount section?")] private bool displayResource = true;
    [SerializeField, Tooltip("Does it display the resource name?")] private bool displayResourceName = false;
    [SerializeField] private string displaySign = "+";
    [SerializeField, Tooltip("Does it display shadow under sprites and texts for better readability")] private bool displayShadow = true;
    [SerializeField,  Tooltip("Displacement between each set of resource+text")] Vector3 displacement = new Vector3(2,0,0);
    
    enum Alighment {Front,Back, Center}
    [SerializeField] Alighment alighment = Alighment.Front;

    [Header("------DO NOT EDIT BELOW------")]
    [SerializeField,  Tooltip("Do not change. Indicate the GameObejct template that displays the set of sprite+text for each resource.")] GameObject spriteAmountSetTemplate;
    [SerializeField,  Tooltip("Do not change. Indicate the container for each set of sprite+text for each resource.")] Transform container;
    [SerializeField,  Tooltip("Do not change. Indicate the GameObject that displays the spirit points and amount.")] GameObject spiritPointDisplay;

    [SerializeField] private Transform shadowsContainer;
    
    //local vairables:
    private ResourceSet displayingResourceSet;
    Vector3 shadowDisplacement = new Vector3(0.04f, -0.04f, -0.04f);
    
    private void OnEnable()
    {
        Generate();
    }

    public void Generate()
    {
        if (displayFromResourceSetProvider)
        {
            if (resourceSetProvider is IResourceSetProvider)
            {
                resourceSetToDisplay = ((IResourceSetProvider)resourceSetProvider).ProvideResourceSet(resourceSetProviderIndex);
            }
            else
            {
                Debug.LogError("ResourceSetDisplay has nothing to display");
            }
        }
        //print(resourceSetToDisplay);
        Display(resourceSetToDisplay);;
    }

    public void Display(ResourceSet rs)
    {
        if (rs == null)
        {
            Debug.LogError("No resource set to be displayed.");
            return;
        }

        //Set up what will be displayed
        if (displaySpiritPoints)
        {
            spiritPointDisplay.gameObject.SetActive(true);
        }
        else
        {
            spiritPointDisplay.gameObject.SetActive(false);
        }
        if (displayResource)
        {
            container.gameObject.SetActive(true);
        }
        else
        {
            container.gameObject.SetActive(false);
        }
        
        if (displayingResourceSet == rs)
        {
            //print("no need to recalculate");
            //return;
        } 
        ClearDisplay();
        displayingResourceSet = rs;
        spriteAmountSetTemplate.SetActive(true);
        GenerateIcons();
        GenerateSpiritPoints();
        spriteAmountSetTemplate.SetActive(false);
    }
    
    private void GenerateIcons()
    {
        if(!displayResource) return;
        for (int i = 0; i < displayingResourceSet.resources.Count; i++) //Generate each resource and amount
        {
            //generate resource and amount
            ResourceSet.ResourceAmount ra = displayingResourceSet.resources[i];
            GameObject go = Instantiate(spriteAmountSetTemplate, container);
            go.SetActive(true);
            SpriteRenderer sr = go.transform.Find("Sprite").GetComponent<SpriteRenderer>();
            TextMeshPro tmp = go.transform.Find("Amount").GetComponent<TextMeshPro>();
            TextMeshPro sign = go.transform.Find("Sign").GetComponent<TextMeshPro>();
            TextMeshPro name = go.transform.Find("Name").GetComponent<TextMeshPro>();
            sr.sprite = ra.iso.iconSprite;
            tmp.text = "" + ra.amount;
            sign.text = displaySign;
            name.text = ra.iso.tetrisHoverName;
            
            
            //generate shadow
            /*
            if (displayShadow)
            {
                SpriteRenderer srShadow = Instantiate(sr.gameObject, shadowsContainer).GetComponent<SpriteRenderer>();
                srShadow.color = Color.black;
                srShadow.transform.localPosition += shadowDisplacement;
                
                TextMeshPro tmpShadow = Instantiate(tmp.gameObject, shadowsContainer).GetComponent<TextMeshPro>();
                tmpShadow.color = Color.black;
                tmpShadow.transform.localPosition += shadowDisplacement;

            }*/
            
            //set position according to alignment
            if (alighment == Alighment.Front)
            {
                go.transform.localPosition += displacement * i;
            }
            else if (alighment == Alighment.Back)
            {
                go.transform.localPosition -= displacement * i;
            }
            else if (alighment == Alighment.Center)
            {
                if (displayingResourceSet.resources.Count % 2 == 1) //amount display is odd
                {
                    int positivity = (int)Mathf.Pow(-1, i % 2);
                    go.transform.position += positivity * displacement * ((i + 1) / 2);
                }
                else //amount display is even
                {
                    int positivity = (int)Mathf.Pow(-1, i % 2);
                    go.transform.localPosition += positivity * displacement * (0.5f + (i / 2));
                }
            }
        }
    }

    private void GenerateSpiritPoints()
    {
        if(!displaySpiritPoints)return;
        SpriteRenderer sr = spiritPointDisplay.transform.Find("Sprite").GetComponentInChildren<SpriteRenderer>();
        TextMeshPro tmp = spiritPointDisplay.transform.Find("Amount").GetComponentInChildren<TextMeshPro>();
        tmp.text = ""+displayingResourceSet.spiritPoint;
        TextMeshPro sign = spiritPointDisplay.transform.Find("Sign").GetComponentInChildren<TextMeshPro>();
        sign.text = displaySign;
            
        /*
        if (displayShadow) //generate shadow
        {
            SpriteRenderer srShadow = Instantiate(sr.gameObject, shadowsContainer).GetComponent<SpriteRenderer>();
            srShadow.color = Color.black;
            srShadow.transform.localPosition += shadowDisplacement;
            
            TextMeshPro tmpShadow = Instantiate(tmp.gameObject, shadowsContainer).GetComponent<TextMeshPro>();
            tmpShadow.color = Color.black;
            tmpShadow.transform.localPosition += shadowDisplacement;
        }*/
    }
    
    private void ClearDisplay()
    {
        for(int i = container.childCount-1; i>= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(container.GetChild(i).gameObject);
#else
        Destroy(container.GetChild(i).gameObject);
#endif
        }
    }



}
