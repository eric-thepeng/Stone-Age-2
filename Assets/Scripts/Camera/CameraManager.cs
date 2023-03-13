using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.PlayerLoop;

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

    private void Update()
    {
        //ZOOMING
        float targetCamHeight = transform.position.y + Input.mouseScrollDelta.y * 2;
        targetCamHeight = Mathf.Clamp(targetCamHeight, 40, 90);
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
        transform.position += new Vector3(direction.x, 0f, direction.y) * Time.deltaTime * 15 * momentum;
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

}
