using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestIndicationWorldspace : MonoBehaviour
{
    public static HarvestIndicationWorldspace i;

    //[SerializeField]
    GameObject FloatingTextPrefab;

    void Awake()
    {
        if (i == null)
        {
            i = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        FloatingTextPrefab = transform.Find("Harvest Floating Text Template").gameObject;
    }

    public void CreateText(Vector3 FloatingTextPosition, ItemScriptableObject item, int itemNumber)
    {
        GameObject thisFloatingTextPrefab = Instantiate(FloatingTextPrefab, FloatingTextPosition + new Vector3(0, 2.5f, 0), FloatingTextPrefab.transform.rotation, this.gameObject.transform);

        thisFloatingTextPrefab.GetComponent<HarvestFloatingText>().Setup(item, itemNumber);
    }

    public void TestCreate()
    {
        print("asd");
    }
}
