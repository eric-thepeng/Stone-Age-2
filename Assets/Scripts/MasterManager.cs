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

    private void Update()
    {
        if(playerState == PlayerState.World) //Player at exploration
        {
            /*
            if (Input.GetKeyDown(KeyCode.Return))
            {
                GoHome();
            }*/
        }
        else //Player at Home
        {
            /*
            if (Input.GetKeyDown(KeyCode.Return))
            {
                LeaveHome();
            }*/

            if(playerState == PlayerState.Crafting && Input.GetKeyDown(KeyCode.A)) //Crafting -> Shop
            {
                GoToShop();
            }

            if (playerState == PlayerState.Crafting && Input.GetKeyDown(KeyCode.D)) //Crafting -> Task
            {
                GoToTask();
            }

            if (playerState == PlayerState.Task && Input.GetKeyDown(KeyCode.A)) //Task -> Crafting
            {
                GoToCrafting();
            }

            if (playerState == PlayerState.Shop && Input.GetKeyDown(KeyCode.D)) //Shop -> Crafting
            {
                GoToCrafting();
            }
        }
    }

    public bool inExploration()
    {
        if (playerState == PlayerState.World) return true;
            return false;
    }
    public void GoHome()
    {
        CameraManager.i.switchCameraTo(2);
        playerState = PlayerState.Crafting;
        TaskManager.i.CheckTasksComplete();
    }

    public void LeaveHome()
    {
        CameraManager.i.switchCameraTo(1);
        playerState = PlayerState.World;
    }

    public void GoToCrafting()
    {
        CameraManager.i.switchCameraTo(2);
        playerState = PlayerState.Crafting;
    }

    public void GoToTask()
    {
        CameraManager.i.switchCameraTo(3);
        playerState = PlayerState.Task;
    }

    public void GoToShop()
    {
        CameraManager.i.switchCameraTo(4);
        playerState = PlayerState.Shop;
    }

    public void ExitDoorButtonClick(Button btn)
    {
        print("exit door button");
        if(playerState == PlayerState.World)
        {
            AudioManager.i.PlaySoundEffectByName("Door Close");
            GoHome();
        }
        else
        {
            AudioManager.i.PlaySoundEffectByName("Door Open");
            LeaveHome();
        }
    }

}
