using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FogWithSDFController : MonoBehaviour
{
    VisualEffect fogVFX; // this VFX

    void Start()
    {
        fogVFX = GetComponent<VisualEffect>();
    }

    void Update()
    {
        Debug.Log(fogVFX.aliveParticleCount);
    }
}
