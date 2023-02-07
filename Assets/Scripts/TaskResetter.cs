using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TaskResetter : MonoBehaviour
{
    [SerializeField] bool RUN = false;
    [SerializeField] AllTaskInfo ati = null;

    private void Update()
    {
        if (RUN)
        {
            RUN = false;

            FunctionToRun();
        }
    }

    private void Awake()
    {
        FunctionToRun();
    }
    private void FunctionToRun()
    {
        foreach(TaskInfo ti in ati.info)
        {
            ti.completion = 0;
        }
        Debug.Log("All tasks are resetted back to completion = 0.");
    }
}