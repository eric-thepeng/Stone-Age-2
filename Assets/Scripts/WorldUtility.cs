using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUtility : MonoBehaviour
{
    //Attach a physical raycast to camera to use this.
    public static Vector3 GetMouseHitPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a RaycastHit object to store the information about what the ray hit
        RaycastHit hitInfo;

        // Perform the raycast and check if it hit something
        if (Physics.Raycast(ray, out hitInfo))
        {
            // Get the position of the collision point in the world
            return hitInfo.point;
        }
        Debug.LogError("failure to detect collision");
        return Vector3.zero;
    }
}
