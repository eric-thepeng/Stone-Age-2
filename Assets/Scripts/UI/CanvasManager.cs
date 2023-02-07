using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    static CanvasManager instance;
    public static CanvasManager i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CanvasManager>();
            }
            return instance;
        }
    }
}
