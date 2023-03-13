using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            SpiritPoint.i.Add(100);
        }
    }
}
