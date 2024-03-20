using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ExploreMapBillboard : MonoBehaviour
{
    public GameObject spriteGO;
    public bool revealed = false;
    LevelUp closestLevelUp = null;
    
    private void Start()
    {
        spriteGO.SetActive(false);
        BindToLevelUp();
    }

    private void BindToLevelUp()
    {
        float smallestDistance = 10000;
        foreach (var element in ExploreMap.i.GetAllExploreSpotsLevelUp())
        {
            float thisDistance = (element.transform.position - transform.position).magnitude;
            if ( thisDistance <= smallestDistance)
            {
                closestLevelUp = element;
                smallestDistance = thisDistance;
            }
        }
        closestLevelUp.OnUnlockFinalState.AddListener(RevealBillboard);
    }

    private void RevealBillboard()
    {
        revealed = true;
        spriteGO.SetActive(true);
        spriteGO.transform.localEulerAngles = new Vector3(0, 0, 0);
        SpriteRenderer sr = spriteGO.GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
        sr.DOFade(1, .9f);
        spriteGO.transform.DOLocalRotate(new Vector3(-55,0,0),1f).onComplete = BillboardGoToPosition;
        closestLevelUp.OnUnlockFinalState.RemoveListener(RevealBillboard);
    }

    private void BillboardGoToPosition()
    {
        spriteGO.transform.DOLocalRotate(new Vector3(-40, 0, 0), .5f);
    }
    
    
}
