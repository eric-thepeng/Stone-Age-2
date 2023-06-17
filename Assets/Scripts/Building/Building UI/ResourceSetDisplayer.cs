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

    [SerializeField] private bool displaySpiritPoints = true;
    [SerializeField] private bool displayResource = true;
    [SerializeField] ResourceSet resourceSet = null; 
    [SerializeField] Vector3 displacement = new Vector3(2,0,0);
    [SerializeField] GameObject spriteAmountSetTemplate;
    [SerializeField] Transform container;
    [SerializeField] GameObject spiritPointDisplay;
    
    enum Alighment
    {Left, Center, Right
    }

    [SerializeField] Alighment alighment = Alighment.Left;


    public void Start()
    {
        spriteAmountSetTemplate.SetActive(false);
        spiritPointDisplay.SetActive(false);
    }

    private void GenerateDisplay()
    {
        //Generate Spirit Point
        if (displaySpiritPoints)
        {
            spiritPointDisplay.GetComponentInChildren<TextMeshPro>().text = ""+resourceSet.spiritPoint;
            spiritPointDisplay.SetActive(true);
        }
    

        
        // Generate Resource
        if(!displayResource) return;
        for (int i = 0; i < resourceSet.resources.Count; i++)
        {
            ResourceSet.ResourceAmount ra = resourceSet.resources[i];
            GameObject go = Instantiate(spriteAmountSetTemplate, transform);
            go.SetActive(true);
            go.GetComponentInChildren<SpriteRenderer>().sprite = ra.iso.iconSprite;
            go.GetComponentInChildren<TextMeshPro>().text = "" + ra.amount;
            if (alighment == Alighment.Left)
            {
                go.transform.position += displacement * i;
            }
            else if (alighment == Alighment.Right)
            {
                go.transform.position -= displacement * i;
            }
            else if (alighment == Alighment.Center)
            {
                if (resourceSet.resources.Count % 2 == 1) //amount display is odd
                {
                    int sign = (int)Mathf.Pow(-1, i % 2);
                    go.transform.position += sign * displacement * ((i + 1) / 2);
                }
                else //amount display is even
                {
                    int sign = (int)Mathf.Pow(-1, i % 2);
                    go.transform.position += sign * displacement * (0.5f + (i / 2));
                }
            }
        }
    }

    private void ClearDisplay()
    {
        for(int i = container.childCount-1; i>= 0; i--)
        {
            Destroy(container.GetChild(i));
        }
        spiritPointDisplay.SetActive(false);
    }

    public void Hide()
    {
        container.gameObject.SetActive(false);
        spiritPointDisplay.SetActive(false);
    }

    public void Display(ResourceSet rs)
    {
        container.gameObject.SetActive(true);
        if (resourceSet == rs)
        {
            print("no need to recalculate");
            return;
        }
        ClearDisplay();
        resourceSet = rs;
        GenerateDisplay();
    }

}
