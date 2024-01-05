using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ExploreMapBillboard : MonoBehaviour
{
    public GameObject spriteGO;
    public bool revealed = false;

    private void Start()
    {
        spriteGO.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(revealed) return;
        if (other.gameObject.name == "Billboard Enable")
        {
            revealBillboard();
        }
    }

    private void revealBillboard()
    {
        revealed = true;
        spriteGO.SetActive(true);
        spriteGO.transform.localEulerAngles = new Vector3(0, 0, 0);
        spriteGO.transform.DOLocalRotate(new Vector3(-40,0,0),2f);
    }
}
