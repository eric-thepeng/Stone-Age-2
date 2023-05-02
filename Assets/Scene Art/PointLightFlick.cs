using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointLightFlick : MonoBehaviour
{
    [SerializeField]
    float strengthBase = 5;
    [SerializeField]
    float flickSpeed = 10;
    [SerializeField]
    float strengthVolality = 1;

    float timeCount = 0;

    float flickStrength;

    Light pointLight;

    private void Start()
    {
        pointLight = GetComponent<Light>();
    }

    private void Update()
    {
        timeCount += Time.deltaTime;
        flickStrength = strengthBase + strengthVolality * Mathf.Cos(timeCount * flickSpeed);

        // pointLight.range = flickStrength;

        pointLight.intensity = flickStrength / 10;
    }
}
