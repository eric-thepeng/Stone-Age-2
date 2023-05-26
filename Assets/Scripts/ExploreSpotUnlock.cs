using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExploreSpotUnlock : MonoBehaviour
{ 
    GameObject resourceSet;
    Transform spawnTransform;
    GameObject spiritPointText;

    int unlockSpiritPoint = 0;
    ItemScriptableObject[] unlockResource;
    int[] unlockResrouceAmount;

    float unit = 0.75f;

    private void Awake()
    {
        resourceSet = transform.Find("Resource Set").gameObject;
        spawnTransform = transform.Find("Spawn Transform");
    }

    private void Start()
    {
    }

    public void PassInResourceInfo(ItemScriptableObject[] unlockResource, int[] unlockResrouceAmount, int unlockSpiritPoint)
    {
        this.unlockResource = unlockResource;
        this.unlockResrouceAmount = unlockResrouceAmount;
        this.unlockSpiritPoint = unlockSpiritPoint;
    }

    public void CreatResourceIndicator()
    {
        gameObject.SetActive(true);

        for (int count = 0; count < unlockResource.Length; count ++)
        {
            CreateResourceSet(unlockResource[count], unlockResrouceAmount[count], count, unlockResource.Length);
        }

        transform.Find("Spirit Point Amount").GetComponent<TextMeshPro>().text = unlockSpiritPoint + "";
    }

    void CreateResourceSet(ItemScriptableObject resource, int weight, int offset, int length)
    {
        GameObject thisResourceSet = Instantiate(resourceSet, spawnTransform.position, spawnTransform.rotation, spawnTransform);

        thisResourceSet.SetActive(true);

        thisResourceSet.transform.position += new Vector3((length - 1) * -unit / 2 + offset * unit, 0, 0);

        thisResourceSet.transform.Find("Resource Icon").GetComponent<SpriteRenderer>().sprite = resource.iconSprite;
        thisResourceSet.transform.Find("Resource Amount").GetComponent<TextMeshPro>().text = weight + "";
    }
}
