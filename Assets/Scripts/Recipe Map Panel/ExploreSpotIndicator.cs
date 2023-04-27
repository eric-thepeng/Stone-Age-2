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
    List<ExploreSpot.SpotResourceInfo> resourceList = null;

    float unit = 3f;

    private void Start()
    {
        resourceSet = transform.Find("Resource Set").gameObject;
        spawnTransform = transform.Find("Spawn Transform");

        gameObject.SetActive(false);
    }

    public void PassInResourceInfo(int spiritPointAmount, List<ExploreSpot.SpotResourceInfo> resourceList)
    {
        this.resourceList = resourceList;
        this.spiritPointAmount = spiritPointAmount;
    }

    public void CreatResourceIndicator()
    {
        gameObject.SetActive(true);

        for (int count = 0; count < resourceList.Count; count ++)
        {
            CreateResourceSet(resourceList[count].item, resourceList[count].amount, count, resourceList.Count);
        }

        transform.Find("Spirit Point Amount").GetComponent<TextMeshPro>().text = spiritPointAmount + "" ;
    }

    void CreateResourceSet(ItemScriptableObject resource, int amount, int offset, int length)
    {
        GameObject thisResourceSet = Instantiate(resourceSet, spawnTransform.position, spawnTransform.rotation, spawnTransform);

        thisResourceSet.SetActive(true);

        thisResourceSet.transform.position += new Vector3((length - 1) * -unit / 2 + offset * unit, 0, 0);
        thisResourceSet.transform.localPosition += new Vector3(0, 0.3f, 0);

        thisResourceSet.transform.Find("Resource Icon").GetComponent<SpriteRenderer>().sprite = resource.iconSprite;
        thisResourceSet.transform.Find("Resource Amount").GetComponent<TextMeshPro>().text = "x " + amount;
    }
}
