using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiscoveryGatherSpot : MonoBehaviour
{
    [SerializeField] private float discoveryTarget = 10;
    [SerializeField] private float discoveryCurrent = 10;

    private void Start()
    {
        
    }

    public void DiscoverySuccess()
    {

    }

    public bool Discover(float discoverAmount)
    {
        discoveryCurrent += discoverAmount;
        if (discoveryCurrent >= discoveryTarget)
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


}
