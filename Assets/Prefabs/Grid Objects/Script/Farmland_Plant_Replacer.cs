using MK.Toon.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmland_Plant_Replacer : MonoBehaviour
{
    [SerializeField]
    float scaleOffset = 0;

    [SerializeField]
    GameObject prefab;
    void Start()
    {
        foreach (Transform child in transform)
        {
            foreach (Transform childsChild in child.transform)
            {
                Destroy(childsChild.gameObject);
            }

            GameObject plant = Instantiate(prefab, child.transform);
            float scale = Random.Range(plant.transform.localScale.x - scaleOffset, plant.transform.localScale.x + scaleOffset);

            //plant.transform.localScale = new Vector3(scale, scale, scale);
            plant.transform.localRotation = Quaternion.Euler(new Vector3(-90, Random.Range(0f, 360f), 0));
        }   
    }
}
