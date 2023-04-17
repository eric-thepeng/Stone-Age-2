using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.PlayerLoop;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    static CameraManager instance;
    public static CameraManager i
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<CameraManager>();
            }
            return instance;
        }
    }

    float momentum = 0f;
    Vector2 direction = new Vector2(0,0);
    Vector2Int moveByMouseDirection = new Vector2Int(0,0);

    float cameraZoomSpeed = 4, cameraMoveSpeed = 22, cameraMinHeight = 40, cameraMaxHeight = 90;

    float cameraXMin=-80, cameraXMax=100, cameraYMin, cameraYMax;

    Vector3 homeCameraPosition = new Vector3(4f,70,-70);

    private void Update()
    {
        if (!(PlayerState.IsBrowsing() || PlayerState.IsBuilding())) return;
        //ZOOMING
        float targetCamHeight = transform.position.y + Input.mouseScrollDelta.y * cameraZoomSpeed;
        targetCamHeight = Mathf.Clamp(targetCamHeight, cameraMinHeight, cameraMaxHeight);
        float yTrueDelta = targetCamHeight - transform.position.y;
        transform.position = new Vector3(transform.position.x, targetCamHeight ,transform.position.z-yTrueDelta);

        //MOVEMENT
        if (GetKeyboardInput() == new Vector2Int(0,0) && moveByMouseDirection == new Vector2Int(0, 0)) 
        {
            momentum -= Time.deltaTime * 6f;
            momentum = Mathf.Clamp(momentum, 0, 1f);
        }
        else
        {
            if(moveByMouseDirection != new Vector2Int(0, 0)) //move by mouse
            {
                direction = moveByMouseDirection;
            }
            else //move by keyboard
            {
                direction = GetKeyboardInput();
            }
            direction = direction.normalized;
            momentum = 1;
        }
        transform.position += new Vector3(direction.x, 0f, direction.y) * Time.deltaTime * cameraMoveSpeed * momentum;
    }

    Vector2Int GetKeyboardInput()
    {
        Vector2Int ip = new Vector2Int(0,0);
        if (Input.GetKey(KeyCode.W)) ip.y += 1;
        if (Input.GetKey(KeyCode.S)) ip.y -= 1;
        if (Input.GetKey(KeyCode.A)) ip.x -= 1;
        if (Input.GetKey(KeyCode.D)) ip.x += 1;
        return ip;
    }

    public void SetMoveByMouse(Vector2Int toWhich)
    {
        moveByMouseDirection = toWhich;
    }

    public void CancelMoveByMouse(Vector2Int fromWhich)
    {
        if(moveByMouseDirection == fromWhich) moveByMouseDirection = new Vector2Int(0, 0);
    }

    public void MoveBackToHome()
    {
        transform.DOMove(homeCameraPosition, 0.5f);
    }

}
