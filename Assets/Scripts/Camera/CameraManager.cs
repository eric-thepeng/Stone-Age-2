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

    private void Update()
    {
        transform.position += new Vector3(GetKeyboardInput().x, 0f, GetKeyboardInput().y) * Time.deltaTime * 10;
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
}
