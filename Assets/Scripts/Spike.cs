using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    bool coolDown = false;
    private void OnTriggerStay(Collider other)
    {
        if (coolDown) return;
        coolDown = true;
        Health.i.ChangeHealth(-10);
        StartCoroutine(GoCoolDown());
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("Enter");
        if (coolDown) return;
        coolDown = true;
        Health.i.ChangeHealth(-10);
        StartCoroutine(GoCoolDown());
    }

    IEnumerator GoCoolDown()
    {
        yield return new WaitForSeconds(1);
        coolDown = false;
    }
}
