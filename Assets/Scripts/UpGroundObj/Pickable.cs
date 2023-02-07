using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : UpGroundObj
{
    [SerializeField] GameObject objProduce;
    [SerializeField] int produceAmount = 1;
    [SerializeField] ItemScriptableObject isoToGet;
    [SerializeField] Sprite staticPop = null;
    // Start is called before the first frame update
    bool staticCanPickUp = true;

    public void PickUp()
    {
        if(staticPop != null)
        {
            if (!staticCanPickUp) return;
            staticCanPickUp = false;
            StartCoroutine(StaticPickUpIE());
        }
        else if (objProduce != null)
        {
            StartCoroutine(PickUpIE());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator PickUpIE()
    {
        AudioManager.i.PlaySoundEffectByName("Pick Up Resource", true);
        int amount = 0;
        while(amount < produceAmount)
        {
            GameObject ngo = Instantiate(objProduce, transform.parent);
            ngo.transform.position = transform.position;
            ngo.GetComponent<mushroom>().SetUp(GetComponent<SpriteRenderer>().sprite, isoToGet, 1.5f);
            amount += 1;
            if(amount == produceAmount)
            {
                Destroy(gameObject);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
    IEnumerator StaticPickUpIE()
    {
        AudioManager.i.PlaySoundEffectByName("Pick Up Resource", true);
        GameObject ngo = Instantiate(objProduce, transform.parent);
        ngo.transform.position = transform.position;
        ngo.GetComponent<mushroom>().SetUp(staticPop, isoToGet, 1f);
        yield return new WaitForSeconds(0.5f);
        staticCanPickUp = true;
    }
}
