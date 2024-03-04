using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiscoveryGatherSpot : GatherSpot, IResourceSetProvider
{
    [Header("------ EDIT THIS ------")]
    [SerializeField] private float discoveryTarget = 10;
    private float discoveryCurrent = 0;

    [Header("------ DO NOT EDIT BELOW ------")]
    [SerializeField] private GameObject discoveryUIGameObject;

    public ResourceSet ProvideResourceSet(int index = 0)
    {
        return gatherResource;
    }

    public void DiscoverySuccess()
    {

    }

    /// <summary>
    /// called by character after each gather
    /// </summary>
    /// <param name="discoverAmount">discover level to increase</param>
    /// <returns></returns>
    public void Discover(float discoverAmount)
    {
        if(Discovered()) return;

        discoveryCurrent += discoverAmount;

        if (Discovered())
        {
            DiscoverySuccess();
            return;
        }

    }

    public float GetDiscoveryPercentage()
    {
        return Mathf.Clamp((int)(discoveryCurrent / discoveryTarget * 100)/100, 0, 1);
    }

    public bool Discovered()
    {
        return false;
        return GetDiscoveryPercentage() == 1;
    }

    protected override void OnMouseEnter()
    {
        if (Discovered())
        {
            discoveryUIGameObject.SetActive(true);
        }
        base.OnMouseEnter();
    }

    protected override void OnMouseExit()
    {
        if (Discovered())
        {
            discoveryUIGameObject.SetActive(false);
        }
        base.OnMouseExit();
    }
}
