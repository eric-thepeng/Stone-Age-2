using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ClearingFinishedParticleRemover : MonoBehaviour
{
    VisualEffect vfx;
    float lifetimeCount = 0;

    void Start()
    {
        vfx = GetComponent<VisualEffect>();
    }

    void Update()
    {
        lifetimeCount += Time.deltaTime;

        if (vfx.aliveParticleCount == 0 && lifetimeCount > 0.5f)
        {
            Destroy(gameObject);
        }
    }
}
