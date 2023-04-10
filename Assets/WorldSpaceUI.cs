using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Vector3 positionToReturn = WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.UI_BACKGROUND, true);
        backgroundCollider.gameObject.SetActive(false);

        return positionToReturn;
    }
}
