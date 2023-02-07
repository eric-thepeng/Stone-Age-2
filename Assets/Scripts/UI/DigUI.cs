using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigUI : MonoBehaviour
{
    static DigUI instance;
    SpriteRenderer mySR, fillerSR, bgSR;
    public static DigUI i
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<DigUI>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        mySR = GetComponent<SpriteRenderer>();
        fillerSR = transform.GetChild(0).GetComponent<SpriteRenderer>();
        bgSR = transform.GetChild(1).GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        Clear();
    }
    public void SetRatio(float setTo)
    {
        if (mySR.enabled == false) mySR.enabled = true;
        if (fillerSR.enabled == false) fillerSR.enabled = true;
        if (bgSR.enabled == false) bgSR.enabled = true;
        transform.GetChild(0).transform.localScale = new Vector3(setTo, setTo, 1);
    }

    public void Clear()
    {
        mySR.enabled = false;
        fillerSR.enabled = false;
        bgSR.enabled = false;
    }
}
