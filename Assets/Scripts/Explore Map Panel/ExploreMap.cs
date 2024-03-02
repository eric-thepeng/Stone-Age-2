using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Sirenix.Utilities;
using Unity.Mathematics;
using UnityEngine;

public class ExploreMap : MonoBehaviour
{
    float momentum = 0f;
    [SerializeField]private float moveSpeed = 1f;
    Vector2 direction = new Vector2(0,0);

    private float mapXMax = -12.5f;
    private float mapXMin = -31f;

    private void Update()
    {
        if(!PlayerState.IsExploreMap())return;
        
        //MOVEMENT BY KEYBOARD
        if (PlayerInputChannel.i.GetKeyBoardInputDirection() == new Vector2Int(0,0)) 
        {
            momentum -= Time.deltaTime * 6f;
            momentum = Mathf.Clamp(momentum, 0, 1f);
        }
        else
        {
            {
                direction = -PlayerInputChannel.i.GetKeyBoardInputDirection();
            }
            direction.y = 0;
            direction = direction.normalized;
            momentum = 1;
        }
        transform.localPosition += new Vector3(direction.x, direction.y, 0) * Time.deltaTime * moveSpeed * momentum;
        transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, mapXMin, mapXMax),
                transform.localPosition.y,
                transform.localPosition.z);
        //if(restrainedCamera) transform.position = new Vector3(Mathf.Clamp(transform.position.x, cameraXMin, cameraXMax), transform.position.y, Mathf.Clamp(transform.position.z, cameraZMin, cameraZMax));
    }
}
