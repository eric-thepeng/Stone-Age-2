using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
//using UnityEditor.Experimental.GraphView;
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
        public static LayerID HOME_GRID = new LayerID(LayerMask.GetMask("Home Grid"));
        public static LayerID WORLD_INTERACTABLE = new LayerID(LayerMask.GetMask("World Interactable"));
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
        Debug.LogWarning("failure to detect collision");
        throw new Exception();
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

    public static TextMeshPro CreateWorldText(string text, Transform parent = null, Vector3 globalPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignmentOptions textAlignment = TextAlignmentOptions.Left, int sortingOrder = 0)
    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, globalPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }

    // Create Text in the World
    public static TextMeshPro CreateWorldText(Transform parent, string text, Vector3 globalPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignmentOptions textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMeshPro));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.position = globalPosition;
        TextMeshPro textMesh = gameObject.GetComponent<TextMeshPro>();
        //textMesh.textan//anchor = textAnchor;
       textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
}
