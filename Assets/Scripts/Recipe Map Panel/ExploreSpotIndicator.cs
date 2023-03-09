using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExploreSpotIndicator : MonoBehaviour
{ 
    GameObject resourceSet;
    Transform spawnTransform;
    GameObject spiritPointText;

    int spiritPointAmount = 0;
    ItemScriptableObject[] resourceList;
    int[] weightList;
    int totalWeight;

    private void Start()
    {
        resourceSet = transform.Find("Resource Set").gameObject;
        spawnTransform = transform.Find("Spawn Transform");

        gameObject.SetActive(false);
    }

    public void PassInResourceInfo(ItemScriptableObject[] resourceList, int[] weightList, int totalWeight, int spiritPointAmount)
    {
        this.resourceList = resourceList;
        this.weightList = weightList;
        this.totalWeight = totalWeight;
        this.spiritPointAmount = spiritPointAmount;
    }

    public void CreatResourceIndicator()
    {
        gameObject.SetActive(true);

        for (int count = 0; count < resourceList.Length; count ++)
        {
            CreateResourceSet(resourceList[count], (int)((float)weightList[count] / (float)totalWeight * 100), count);
        }

        transform.Find("Spirit Point Amount").GetComponent<TextMeshPro>().text = spiritPointAmount + "";
    }

    void CreateResourceSet(ItemScriptableObject resource, int weight, int offset)
    {
        GameObject thisResourceSet = Instantiate(resourceSet, spawnTransform.position, spawnTransform.rotation, spawnTransform);

        thisResourceSet.SetActive(true);

        thisResourceSet.transform.position += new Vector3(offset, 0, 0);

        thisResourceSet.transform.Find("Resource Icon").GetComponent<SpriteRenderer>().sprite = resource.iconSprite;
        thisResourceSet.transform.Find("Resource Amount").GetComponent<TextMeshPro>().text = weight + " %";
    }
}
