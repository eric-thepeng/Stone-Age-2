using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUtility : MonoBehaviour //Attach a physical raycast to camera to use this.
{

    public static class LAYER
    {
        //public static LayerName;
        static int UI = 9;

    }

    //public AllPhysicsLayerID LAYER = { }

    public static Vector3 GetMouseHitPoint(int layerID, bool collideWithLayer)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a RaycastHit object to store the information about what the ray hit
        RaycastHit hitInfo;

        // Perform the raycast and check if it hit something
        if (collideWithLayer)
        {
            if (Physics.Raycast(ray, out hitInfo, 1000, 1 << layerID))
            {
                // Get the position of the collision point in the world
                return hitInfo.point;
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hitInfo, 1000, ~(1 << layerID)))
            {
                // Get the position of the collision point in the world
                return hitInfo.point;
            }
        }
        Debug.LogError("failure to detect collision");
        return Vector3.zero;
    }

    public static bool TryMouseHitPoint(int layerID, bool collideWithLayer)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a RaycastHit object to store the information about what the ray hit
        RaycastHit hitInfo;

        // Perform the raycast and check if it hit something
        if (collideWithLayer)
        {
            if (Physics.Raycast(ray, out hitInfo, 1000, 1 << layerID))
            {
                // Get the position of the collision point in the world
                return true;
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hitInfo, 1000, ~(1 << layerID)))
            {
                // Get the position of the collision point in the world
                return true;
            }
        }
        return false;
    }

    public static GameObject GetMouseHitObject(int layerID, bool collideWithLayer)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a RaycastHit object to store the information about what the ray hit
        RaycastHit hitInfo;

        // Perform the raycast and check if it hit something
        if (collideWithLayer)
        {
            if (Physics.Raycast(ray, out hitInfo, 1000, 1 << layerID))
            {
                // Get the position of the collision point in the world
                return hitInfo.collider.gameObject;
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hitInfo, 1000, ~(1 << layerID)))
            {
                // Get the position of the collision point in the world
                return hitInfo.collider.gameObject;
            }
        }
        Debug.LogError("failure to detect collision");
        return null;
    }
}
