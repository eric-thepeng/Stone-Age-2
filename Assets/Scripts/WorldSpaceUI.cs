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

    private void Update()
    {
        if (Input.GetKey(KeyCode.Backspace))
        {
            print(GetMouseWorldPositionAtWorldSpaceUI());
        }
    }

    public Vector3 GetMouseWorldPositionAtWorldSpaceUI()
    {
        UI_FullScreenUIDragCollider.i.Open(this);

        Vector3 positionToReturn = new Vector3(0, 0, 0);
        try
        {
            positionToReturn = WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.UI_BACKGROUND, true);
            UI_FullScreenUIDragCollider.i.Close();
            return positionToReturn;
        }
        catch
        {
            throw new Exception();
        }
    }
}
