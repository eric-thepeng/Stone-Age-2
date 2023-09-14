using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FogOfWar : MonoBehaviour
{
    VisualEffect fogVFX; // this VFX

    [HideInInspector]
    public bool removed = false; // becomes true after collide with a remover

    void Start()
    {
        fogVFX = GetComponent<VisualEffect>();
    }

    void Update()
    {
        //Debug.Log(fogVFX.aliveParticleCount);

        CheckSelfDestory();
    }

    void CheckSelfDestory()
    {
        if (removed)
        {
            if (fogVFX.aliveParticleCount == 0)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("triggered / tag :" + collider.gameObject.tag);
        if (collider.gameObject.CompareTag("Fog Remover"))
        {
            removed = true;
            if (fogVFX != null)
            {
                //fogVFX.SetBool("isDestroyed", true);
                fogVFX.playRate = 4;
                fogVFX.Stop();
            }
        }     
    }
}
