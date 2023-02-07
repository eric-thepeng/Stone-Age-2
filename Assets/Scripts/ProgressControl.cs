using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressControl : MonoBehaviour
{
    List<TaskInfo> completedPCTasks = new List<TaskInfo>();
    static ProgressControl instance;

    //task 1 related
    [SerializeField] TaskInfo task1;
    bool task1_freed = false, task1_viewedShop = false, task1_viewedCraft = false;

    [SerializeField] TaskInfo task2;
    bool pc_task2 = false;
    [SerializeField] GameObject task2_ExitButton;

    [SerializeField] TaskInfo task10;
    bool pc_task10= false;
    [SerializeField] GameObject task10_Corpse;

    public static ProgressControl i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ProgressControl>();
            }
            return instance;
        }
    }

    public bool CheckCompletion(TaskInfo ti)
    {
        if (completedPCTasks.Contains(ti)) return true;
        return false;
    }

    public void Free_FirstTask(int panelsTraveled)
    {
        if (task1_freed) return;

        if(panelsTraveled == 2)
        {
            task1_viewedCraft = true;
        }
        else if (panelsTraveled == 4)
        {
            task1_viewedShop = true;
        }

        if(task1_viewedCraft && task1_viewedShop)
        {
            task1_freed = true;
            completedPCTasks.Add(task1);
            TaskManager.i.CheckTasksComplete();
        }
        
    }

    public void TaskOpened(TaskInfo ti)
    {
        if(ti == task2)
        {
            if (pc_task2) return;
            pc_task2 = true;
            task2_ExitButton.SetActive(true);
        }
        if(ti == task10)
        {
            if (pc_task10) return;
            pc_task10 = true;
            task10_Corpse.SetActive(true);
        }
    }

}
