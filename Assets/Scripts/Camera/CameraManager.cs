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

    private void Update()
    {
        
        if(GetKeyboardInput() == new Vector2Int(0,0))
        {
            momentum -= Time.deltaTime * 6f;
            momentum = Mathf.Clamp(momentum, 0, 1f);
        }
        else
        {
            direction = GetKeyboardInput();
            direction = direction.normalized;
            momentum = 1;
        }
        transform.position += new Vector3(direction.x, 0f, direction.y) * Time.deltaTime * 10 * momentum;
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
