using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.PlayerLoop;
using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

    // Camera related stats
    private float cameraZoomSpeedOnFloor = 4, cameraMoveSpeedOnFloor = 22;
    private float cameraHeightMin = 60, cameraHeightMax = 180;
    Vector3 homeCameraPosition = new Vector3(4f,70,-70);
    Vector3 firstExploreSpotCameraPosition = new Vector3(1f,79f,-55f);
    
    // Internal variable
    float momentum = 0f;
    Vector2 direction = new Vector2(0,0);
    Vector2Int moveByMouseDirection = new Vector2Int(0,0);

    // For camera movement space restriction
    bool restrainedCamera = false;
    float cameraXMin=-60, cameraXMax=100, cameraZMin = -100, cameraZMax = 50;

    // Animation Curves
    [SerializeField] private AnimationCurve moveSpeedAgainstHeight;
    [SerializeField] private AnimationCurve zoomSpeedAgainstHeight;
    [SerializeField] private AnimationCurve stickyHeightAgainstHeight;

    [SerializeField] private float urpShadowDistanceMin = 1;
    [SerializeField] private float urpShadowDistanceMax = 10;
    
    // URP Related
    private float urpShadowDistance;
    
    private void Update()
    {
        if (!(PlayerState.IsBrowsing() || PlayerState.IsBuilding())) return;
        //ZOOMING
        float targetCamHeight = transform.position.y + Input.mouseScrollDelta.y * GetWeightedZoomSpeed();
        targetCamHeight = Mathf.Clamp(targetCamHeight, cameraHeightMin, cameraHeightMax);
        float yTrueDelta = targetCamHeight - transform.position.y;
        transform.position = new Vector3(transform.position.x, targetCamHeight ,transform.position.z-yTrueDelta);

        float ratio = (targetCamHeight - cameraHeightMin) / (cameraHeightMax - cameraHeightMin);
        urpShadowDistance = (urpShadowDistanceMax - urpShadowDistanceMin) * ratio + urpShadowDistanceMin;
        
        QualitySettings.shadowDistance = urpShadowDistance;
        UniversalRenderPipelineAsset urp = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        urp.shadowDistance = urpShadowDistance;
        

        //MOVEMENT
        if (PlayerInputChannel.i.GetKeyBoardInputDirection() == new Vector2Int(0,0) && moveByMouseDirection == new Vector2Int(0, 0)) 
        {
            momentum -= Time.deltaTime * 6f;
            momentum = Mathf.Clamp(momentum, 0, 1f);
        }
        else
        {
            /* MOVEMENT BY MOUSE IS DISABLED BECAUSE SHIT IS USELESS
            if(moveByMouseDirection != new Vector2Int(0, 0)) //move by mouse
            {
                direction = moveByMouseDirection;
            }
            else */ 
            //move by keyboard
            {
                direction = PlayerInputChannel.i.GetKeyBoardInputDirection();
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
        return cameraZoomSpeedOnFloor;
        // ZOOM SPEED WEIGHT IS DISABLE, WILL REVISE IN THE FUTURE 漂泊牧歌 still sticky movement clamp
        return cameraZoomSpeedOnFloor * zoomSpeedAgainstHeight.Evaluate((transform.position.y - cameraHeightMin) / (cameraHeightMax - cameraHeightMin));
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
    
    public void MoveToDisplayLocation(Vector3 locationToDisplay, bool moveToHeight = false)
    {
        Vector3 targetCameraPosition = new Vector3(0,0,0);
        targetCameraPosition.x = locationToDisplay.x;
        float targetHeight = moveToHeight ? locationToDisplay.y : transform.position.y;
        targetCameraPosition.y = targetHeight;
        targetCameraPosition.z = locationToDisplay.z - targetHeight;
        
        transform.DOMove(targetCameraPosition, 0.5f);
    }
    
    public void MoveToDisplayLocation(Vector3 locationToDisplay, float targetHeight)
    {
        Vector3 targetCameraPosition = new Vector3(0,0,0);
        targetCameraPosition.x = locationToDisplay.x;
        targetCameraPosition.y = targetHeight;
        targetCameraPosition.z = locationToDisplay.z - targetHeight;
        
        transform.DOMove(targetCameraPosition, 0.5f);
    }

}
