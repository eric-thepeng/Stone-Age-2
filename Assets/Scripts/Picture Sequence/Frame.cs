using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frame : MonoBehaviour
{
    Vector3 origionalPos;
    public float playTime = 1;
    private void Awake()
    {
        origionalPos = transform.position;
    }

    public void BackToOrigionalPosition()
    {
        transform.position = origionalPos;
    }

    public void MoveToPosition(Vector3 destination)
    {
        transform.position = new Vector3(destination.x, destination.y, transform.position.z);
    }
}
