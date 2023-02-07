using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterManager : MonoBehaviour
{
    static MasterManager instance=null;
    public static MasterManager i
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<MasterManager>();
            }
            return instance;
        }
    }

    public enum PlayerState {World, Crafting, Task, Shop}
    [SerializeField] public PlayerState playerState;

}
