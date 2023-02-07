using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_UI : MonoBehaviour
{
    RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }
    public void SetToRatio(float ratio)
    {
        rect.anchorMax = new Vector2(0.5f,ratio);
    }
}
