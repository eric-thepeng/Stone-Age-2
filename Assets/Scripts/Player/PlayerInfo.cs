using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    Transform playerTransform;
    static PlayerInfo instance = null;
    public static PlayerInfo i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerInfo>();
            }
            return instance;
        }
    }

    //SETS
    public void refreshPlayerTransform(Transform inTransform)
    {
        playerTransform = inTransform;
    }

    //GETS
    public Vector3 getPlayerLocation()
    {
        return playerTransform.position;
    }


}
