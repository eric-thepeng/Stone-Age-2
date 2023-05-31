using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceSetDisplayer
{

    ResourceSet rs;
    Vector3 centralPosition;
    Vector3 displacement;
    Transform parentTransform;
    Transform container;
    GameObject template;

    public ResourceSetDisplayer(ResourceSet rs, Transform template, Vector3 displacement, Transform parentTransform = null)
    {
        this.rs = rs;
        this.centralPosition = template.position;
        this.displacement = displacement;
        this.parentTransform = parentTransform;
        this.template = template.gameObject;
        if (parentTransform == null)
        {
            container = MonoBehaviour.Instantiate(new GameObject("Resource Set Display")).transform;
        }
        else
        {
            container = MonoBehaviour.Instantiate(new GameObject("Resource Set Display"), parentTransform).transform;
        }
        Generate();
    }

    public void ResetResourceSet(ResourceSet rs)
    {
        this.rs = rs;
        Clear();
        Generate();
    }

    private void Generate()
    {
        for(int i = 0; i< rs.resources.Count; i++)
        {
            ResourceSet.ResourceAmount ra = rs.resources[i];
            GameObject go = MonoBehaviour.Instantiate(template, container);
            go.SetActive(true);
            go.GetComponentInChildren<SpriteRenderer>().sprite = ra.iso.iconSprite;
            go.GetComponentInChildren<TextMeshPro>().text = ""+ra.amount;
            go.transform.position += displacement * i;
        }
    }

    private void Clear()
    {
        for(int i = container.childCount-1; i>= 0; i--)
        {
            MonoBehaviour.Destroy(container.GetChild(i));
        }
    }
    
}
