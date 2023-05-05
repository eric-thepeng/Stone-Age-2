using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_0001_LookAround : Quest
{

    public GameObject leftGO, rightGO, topGO, downGO;
    public float pressW = 0; 
    public float pressA = 0; 
    public float pressS = 0; 
    public float pressD = 0;
    private float pressTime = 0.3f;

    protected override void StartQuest()
    {
        base.StartQuest();
        leftGO.SetActive(true);rightGO.SetActive(true);topGO.SetActive(true);downGO.SetActive(true);
    }

    private void Update()
    {
        if (!onGoing || completed) return;
        if (Input.GetKey(KeyCode.W)) pressW += Time.deltaTime;
        if (Input.GetKey(KeyCode.A)) pressA += Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) pressS += Time.deltaTime;
        if (Input.GetKey(KeyCode.D)) pressD += Time.deltaTime;
        if (pressA >= pressTime)
        {
            leftGO.SetActive(false);
        }

        if (pressD >= pressTime)
        {
            rightGO.SetActive(false);
        }

        if (pressW >= pressTime)
        {
            topGO.SetActive(false);
        }

        if (pressS >= pressTime)
        {
            downGO.SetActive(false);
        }
        if (pressW >= pressTime && pressA >= pressTime && pressS >= pressTime && pressD >= pressTime) {
            CompleteQuest();
        }
    }
}
