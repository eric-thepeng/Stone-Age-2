using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_0001_LookAround : Quest
{
    public Quest_0001_LookAround()
    {
        questName = "Look around!";
        questDescription = "Press WASD to move around, mouse scroll to zoom in and out.";
        questID = "Quest_0001";
    }

    public float pressW = 0; 
    public float pressA = 0; 
    public float pressS = 0; 
    public float pressD = 0;

    private void Update()
    {
        if (!onGoing || completed) return;
        if (Input.GetKey(KeyCode.W)) pressW += Time.deltaTime;
        if (Input.GetKey(KeyCode.A)) pressA += Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) pressS += Time.deltaTime;
        if (Input.GetKey(KeyCode.D)) pressD += Time.deltaTime;
        if (pressW >= 0.3 && pressA >= 0.3 && pressS >= 0.3 && pressD >= 0.3) {
            CompleteQuest();
            DialogueManager.i.QueueNarrativeSequence("NS_0002");
        }
    }
}
