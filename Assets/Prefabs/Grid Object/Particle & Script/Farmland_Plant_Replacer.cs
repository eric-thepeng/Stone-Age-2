using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmland_Plant_Replacer : MonoBehaviour
{
    [SerializeField]
    float scaleOffset = 0;

    [SerializeField]
    Sprite sprite;
    void Start()
    {
        foreach (Transform child in transform)
        {
            float scale = Random.Range(child.localScale.x - scaleOffset, child.localScale.x + scaleOffset);
            child.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
            child.localScale = new Vector3(scale, scale, scale);
        }   
    }
}
