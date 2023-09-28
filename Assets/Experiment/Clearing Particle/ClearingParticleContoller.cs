using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ClearingParticleContoller : MonoBehaviour
{
    VisualEffect vfx;
    bool shouldDestory = false;

    void Start()
    {
        vfx = GetComponent<VisualEffect>();
        StartEmitting();
    }

    public void StopEmitting()
    {
        vfx.Stop();
        shouldDestory = true;
    }

    void StartEmitting()
    {
        vfx.Play();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.G))
        {
            Debug.Log("hit G");
            StopEmitting();
        }

        // Destory the emitter when there is no particle left
        if (vfx.aliveParticleCount == 0 && shouldDestory == true)
        {
            Destroy(gameObject);
        }
    }
}
