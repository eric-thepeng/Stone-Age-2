using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        foreach(Transform child in transform)
        {
            print(child.name);
            child.GetComponent<Tetris>().itemSO.tetrisSprite = child.GetComponent<SpriteRenderer>().sprite;
            child.GetComponent<Tetris>().itemSO.iconSprite = child.GetComponent<SpriteRenderer>().sprite;
            //child.GetComponent<Tetris>().itemSO;
        }
    }
}
