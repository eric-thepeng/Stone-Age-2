using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.PlayerLoop;
using DG.Tweening;
using Sirenix.Utilities;

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

    private float cameraZoomSpeedOnFloor = 4, cameraMoveSpeedOnFloor = 22;
    
    private float cameraHeightMin = 90, cameraHeightMax = 180;

    float cameraXMin=-60, cameraXMax=100, cameraZMin = -100, cameraZMax = 50;

    Vector3 homeCameraPosition = new Vector3(4f,70,-70);
    Vector3 firstExploreSpotCameraPosition = new Vector3(1f,79f,-55f);

    bool restrainedCamera = false;

    [SerializeField]private AnimationCurve moveSpeedAgainstHeight;
    [SerializeField] private AnimationCurve zoomSpeedAgainstHeight;
    [SerializeField] private AnimationCurve stickyHeightAgainstHeight;

    private void Update()
    {
        if (!(PlayerState.IsBrowsing() || PlayerState.IsBuilding())) return;
        //ZOOMING
        float targetCamHeight = transform.position.y + Input.mouseScrollDelta.y * GetWeightedZoomSpeed();
        targetCamHeight = Mathf.Clamp(targetCamHeight, cameraHeightMin, cameraHeightMax);
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
        transform.position += new Vector3(direction.x, 0f, direction.y) * Time.deltaTime * GetWeightedMoveSpeed() * momentum;
        if(restrainedCamera) transform.position = new Vector3(Mathf.Clamp(transform.position.x, cameraXMin, cameraXMax), transform.position.y, Mathf.Clamp(transform.position.z, cameraZMin, cameraZMax));
    }

    float GetWeightedMoveSpeed()
    {
        return cameraMoveSpeedOnFloor * moveSpeedAgainstHeight.Evaluate((transform.position.y - cameraHeightMin) / (cameraHeightMax - cameraHeightMin));
    }

    float GetWeightedZoomSpeed()
    {
        return cameraZoomSpeedOnFloor * zoomSpeedAgainstHeight.Evaluate((transform.position.y - cameraHeightMin) / (cameraHeightMax - cameraHeightMin));
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

    public void MoveToFirstExploreSpot()
    {
        transform.DOMove(firstExploreSpotCameraPosition, 0.5f);
    }


}
