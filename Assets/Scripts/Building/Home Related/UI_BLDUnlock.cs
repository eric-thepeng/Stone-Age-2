using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_BLDUnlock : MonoBehaviour,IPassResourceSet
{
    ResourceSetDisplayer rsd = null;
    [SerializeField] Transform rsdTemplate;
    [SerializeField] Vector3 displacement;
    ResourceSet unlockCost = null;
    TextMeshPro progressText;

    private void Awake()
    {
        progressText = transform.Find("Progress").GetComponent<TextMeshPro>();
    }

    public void PassResourceSet(ResourceSet rs)
    {
        unlockCost = rs;
        GetComponentInChildren<ResourceSetDisplayer>().Display(unlockCost);
    }

    public void SetProgress(float percent)
    {
        progressText.text = (int)(percent * 100) + "%";
    }

}
