using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class OpeningFogController : MonoBehaviour
{
    [SerializeField]private int index;
    
    VisualEffect fogVFX;

    void Start()
    {
        fogVFX = GetComponent<VisualEffect>();
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Alpha1) && index == 1) || (Input.GetKeyDown(KeyCode.Alpha2) && index == 2))
        {
            Debug.Log("Stop Fog");

            fogVFX.Stop();
            fogVFX.playRate = 2.0f;

            Destroy(gameObject, 4.0f);
        }
    }
}
