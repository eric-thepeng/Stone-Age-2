using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiscoveryGatherSpot : GatherSpot
{
    [Header("------ EDIT THIS ------")]
    [SerializeField] private float discoveryTarget = 10;
    private float discoveryCurrent = 0;

    [Header("------ DO NOT EDIT BELOW ------")]
    [SerializeField] private GameObject discoveryUIGameObject;

    public void DiscoverySuccess()
    {

    }

    public bool Discover(float discoverAmount)
    {
        if(Discovered()) return true;

        discoveryCurrent += discoverAmount;

        if (Discovered())
        {
            DiscoverySuccess();
            return true;
        }

        return false;
    }

    public float GetDiscoveryPercentage()
    {
        return Mathf.Clamp((int)(discoveryCurrent / discoveryTarget * 100)/100, 0, 1);
    }

    public bool Discovered()
    {
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
        base.OnMouseEnter();
    }
}
