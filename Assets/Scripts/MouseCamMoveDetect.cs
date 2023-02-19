using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCamMoveDetect : MonoBehaviour
{
    [SerializeField] Vector2Int boarder;
    private void OnMouseEnter()
    {
        print(gameObject.name);
        CameraManager.i.SetMoveByMouse(boarder);
    }
    private void OnMouseExit()
    {
        CameraManager.i.CancelMoveByMouse(boarder);
    }
}
