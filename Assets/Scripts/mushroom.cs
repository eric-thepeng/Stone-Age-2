using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mushroom : MonoBehaviour
{
    ItemScriptableObject thisSO;
    //bool canGet = false; //fly for 0.5s before it could be pickedup
    IEnumerator Start()
    {
        //stage one, fly for a while
        //GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.8f, 1f), Random.Range(-0.5f, 0f)).normalized * 150f);
        float timeCount = 0;
        float p1Time = 0.5f;
        
        Vector3 p1Direction = new Vector3(Random.Range(-0.5f, 0.5f), 1f, 1f).normalized;
        Vector3 p1StartPos = transform.position;
        Vector3 p1FinalPos = transform.position + p1Direction * 3f;
        while (timeCount<p1Time)
        {
            transform.position = Vector3.Lerp(p1StartPos, p1FinalPos, Mathf.Log(20 * timeCount + 1, 10) / 1f);
            timeCount += Time.deltaTime;
            yield return new WaitForSeconds(0);
        }

       // canGet = true;
        Vector3 orgPos = transform.position;


        while((PlayerInfo.i.getPlayerLocation() - transform.position).magnitude> 1)
        {
            transform.position = Vector3.Lerp(orgPos, PlayerInfo.i.getPlayerLocation(), timeCount - p1Time);
            timeCount += Time.deltaTime;
            yield return new WaitForSeconds(0);
        }

        //Inventory.i.AddItem(thisSO);
        CraftingManager.i.AddToCrafting(thisSO.myPrefab);
        Destroy(gameObject);
    }

    public void SetUp(Sprite newSprite, ItemScriptableObject newISO, float size)
    {
        GetComponent<SpriteRenderer>().sprite = newSprite;
        thisSO = newISO;
        transform.localScale = transform.localScale * size;
    }

    public void SetUp(ItemScriptableObject newISO)
    {
        thisSO = newISO;
    }

}
