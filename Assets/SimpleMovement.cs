using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    [SerializeField]Vector3 localPositionDelta = new Vector3(0,0,0);
    [SerializeField] float locationSpeed = 1;
    [SerializeField] AnimationCurve curve = new AnimationCurve();
    float time = 0;
    Vector3 orgLocalPosition;
    private void Start()
    {
        orgLocalPosition = transform.localPosition;
    }
    private void Update()
    {
        time+= Time.deltaTime * locationSpeed;
        transform.localPosition = orgLocalPosition + localPositionDelta * curve.Evaluate(time);
    }
}
