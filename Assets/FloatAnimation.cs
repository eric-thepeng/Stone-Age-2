using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAnimation : MonoBehaviour
{
    [SerializeField]float yDelta = 1;
    [SerializeField]float speed = 1;
    [SerializeField]AnimationCurve curve;
    float time = 0;
    private void Update()
    {
        time+= Time.deltaTime * speed;
        transform.localPosition = new Vector3(0, yDelta * curve.Evaluate(time), 0);
    }
}
