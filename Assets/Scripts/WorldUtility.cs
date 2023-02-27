using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUtility : MonoBehaviour //Attach a physical raycast to camera to use this.
{

    public static class LAYER
    {
        public class LayerID
        {
            public int id;
            public LayerID(int inID)
            {
                id = inID;
            }
        }
        public static LayerID UI_BACKGROUND = new LayerID(LayerMask.GetMask("UI Background"));
        public static LayerID UI_DRAG = new LayerID(LayerMask.GetMask("UI Drag"));
        public static LayerID GROUND = new LayerID(LayerMask.GetMask("Ground"));
        public static LayerID EXPLORATION_SPOT = new LayerID(LayerMask.GetMask("Exploration Spot"));
        public static LayerID EXPLORATION_SPOT_VIEWER = new LayerID(LayerMask.GetMask("Exploration Spot Viewer"));
        public static LayerID RECIPE_BLOCK_VIEWER = new LayerID(LayerMask.GetMask("Recipe Block Viewer"));
    }

    private void Start()
    {
        print("START PRINTING");
        print(LAYER.UI_BACKGROUND.id);
        print(LAYER.UI_DRAG.id);
        print(LAYER.GROUND.id);
        print(LAYER.EXPLORATION_SPOT.id);
        print(LAYER.EXPLORATION_SPOT_VIEWER.id);
        print(LAYER.RECIPE_BLOCK_VIEWER.id);
        print("END PRINTING");
    }

    

    public static Vector3 GetMouseHitPoint(LAYER.LayerID LayerID, bool collideWithLayer)
    {
        int layerID = LayerID.id;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a RaycastHit object to store the information about what the ray hit
        RaycastHit hitInfo;

        // Perform the raycast and check if it hit something
        if (collideWithLayer)
        {
            if (Physics.Raycast(ray, out hitInfo, 1000, layerID)) //1 << layerID))
            {
                // Get the position of the collision point in the world
                return hitInfo.point;
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hitInfo, 1000, ~(layerID))) //
            {
                // Get the position of the collision point in the world
                return hitInfo.point;
            }
        }
        Debug.LogError("failure to detect collision");
        return Vector3.zero;
    }

    public static bool TryMouseHitPoint(LAYER.LayerID LayerID, bool collideWithLayer)
    {
        int layerID = LayerID.id;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a RaycastHit object to store the information about what the ray hit
        RaycastHit hitInfo;

        // Perform the raycast and check if it hit something
        if (collideWithLayer)
        {
            if (Physics.Raycast(ray, out hitInfo, 1000, layerID)) //
            {
                // Get the position of the collision point in the world
                return true;
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hitInfo, 1000, ~(layerID))) //
            {
                // Get the position of the collision point in the world
                return true;
            }
        }
        return false;
    }

    public static GameObject GetMouseHitObject(LAYER.LayerID LayerID, bool collideWithLayer)
    {
        int layerID = LayerID.id;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a RaycastHit object to store the information about what the ray hit
        RaycastHit hitInfo;

        // Perform the raycast and check if it hit something
        if (collideWithLayer)
        {
            if (Physics.Raycast(ray, out hitInfo, 1000, layerID)) //
            {
                // Get the position of the collision point in the world
                return hitInfo.collider.gameObject;
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hitInfo, 1000, ~(layerID))) //
            {
                // Get the position of the collision point in the world
                return hitInfo.collider.gameObject;
            }
        }
        Debug.LogError("failure to detect collision");
        return null;
    }
}
