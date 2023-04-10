using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldSpaceUI : MonoBehaviour
{
    static WorldSpaceUI instance;
    public static WorldSpaceUI i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<WorldSpaceUI>();
            }
            return instance;
        }
    }

    Transform backgroundCollider;

    private void Start()
    {
        backgroundCollider = transform.Find("Background").GetComponent<Transform>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Backspace))
        {
            print(GetMouseWorldPositionAtWorldSpaceUI());
        }
    }

    public Vector3 GetMouseWorldPositionAtWorldSpaceUI()
    {
        backgroundCollider.gameObject.SetActive(true);
        Vector3 positionToReturn = new Vector3(0, 0, 0);
        try
        {
            positionToReturn = WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.UI_BACKGROUND, true);
            backgroundCollider.gameObject.SetActive(false);
            return positionToReturn;
        }
        catch
        {
            throw new Exception();
        }
    }
}
