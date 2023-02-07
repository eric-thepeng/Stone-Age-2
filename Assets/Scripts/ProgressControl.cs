using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressControl : MonoBehaviour
{
    static ProgressControl instance;

    public static ProgressControl i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ProgressControl>();
            }
            return instance;
        }
    }




}
