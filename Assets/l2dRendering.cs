using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class l2dRendering : MonoBehaviour
{
    private Camera _mainCam;

    void Start()
    {
        _mainCam = Camera.main;
    }

    void LateUpdate()
    {
        // make sure the character is always facing the camera
        transform.LookAt(transform.position + _mainCam.transform.rotation * Vector3.forward, _mainCam.transform.rotation * Vector3.up);

    }
}
