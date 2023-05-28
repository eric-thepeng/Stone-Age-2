using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSetDisplayer
{

    ResourceSet rs;
    Vector3 centralPosition;
    Vector3 displacement;
    Transform parentTransform;
    Transform container;

    public ResourceSetDisplayer(ResourceSet rs, Vector3 centralPosition, Vector3 displacement, Transform parentTransform = null)
    {
        this.rs = rs;
        this.centralPosition = centralPosition;
        this.displacement = displacement;
        this.parentTransform = parentTransform;
        if (parentTransform == null)
        {
            container = MonoBehaviour.Instantiate(new GameObject("Resource Set Display")).transform;
        }
        else
        {
            container = MonoBehaviour.Instantiate(new GameObject("Resource Set Display"), parentTransform).transform;
        }
    }

    public void ResetResourceSet(ResourceSet rs)
    {
        this.rs = rs;
        Clear();
        Generate();
    }

    private void Generate()
    {

    }

    private void Clear()
    {

    }
    
}
