using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopDisplayObj : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(PopsUp());
        GetComponent<Animator>().SetBool("Pop", true);
    }

    IEnumerator PopsUp()
    {
        yield return new WaitForSeconds(0);
    }
}
