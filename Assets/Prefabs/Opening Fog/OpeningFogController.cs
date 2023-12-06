using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class OpeningFogController : MonoBehaviour
{
    VisualEffect fogVFX;

    void Start()
    {
        fogVFX = GetComponent<VisualEffect>();
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Stop Fog");

            fogVFX.Stop();
            fogVFX.playRate = 2.0f;

            Destroy(gameObject, 4.0f);
        }
    }
}
