using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera worldCMVC, craftingCMVC, taskCMVC, shopCMVC;
    /// <summary>
    /// 1: world 2: craft 3: task 4: shop
    /// </summary>
    [SerializeField] int CamNow;
    [SerializeField] GameObject panelBoarder;

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
    

    /// <summary>
    /// Switch camera accroding to int.
    /// </summary>
    /// <param name="which">1: world 2: craft 3: task 4: shop</param>
    public void switchCameraTo(int which)
    {
        if(which == 1) //switch to world
        {
            panelBoarder.SetActive(false);
            CanvasManager.i.TutorialUIDisplay(true);
        }
        else //switch to anyother panels
        {
            //CanvasManager.i.CloseTargetInfo();
            panelBoarder.SetActive(true);
            CanvasManager.i.TutorialUIDisplay(false);
        }

        if(which == 1) //switch to world
        {
            Camera.main.orthographic = false;
            worldCMVC.Priority = 11;
            craftingCMVC.Priority = 10;
            taskCMVC.Priority = 10;
            shopCMVC.Priority = 10;

        }
        else if (which == 2) //switch to craft
        {
            Camera.main.orthographic = true;
            worldCMVC.Priority = 10;
            craftingCMVC.Priority = 11;
            taskCMVC.Priority = 10;
            shopCMVC.Priority = 10;

            ProgressControl.i.Free_FirstTask(2);
        }
        else if(which == 3) //switch to task
        {
            Camera.main.orthographic = true;
            worldCMVC.Priority = 10;
            craftingCMVC.Priority = 10;
            taskCMVC.Priority = 11;
            shopCMVC.Priority = 10;

            TaskManager.i.CheckTasksComplete();
        }
        else if (which == 4) //switch to shop
        {
            Camera.main.orthographic = true;
            worldCMVC.Priority = 10;
            craftingCMVC.Priority = 10;
            taskCMVC.Priority = 10;
            shopCMVC.Priority = 11;

            ProgressControl.i.Free_FirstTask(4);
        }
        else
        {
            print("No such camera to switch to");
        }
        CamNow = which;
    }
}
