using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FogWithSDFController : MonoBehaviour
{
    VisualEffect fogVFX;

    void Start()
    {
        fogVFX = GetComponent<VisualEffect>();
    }
}
