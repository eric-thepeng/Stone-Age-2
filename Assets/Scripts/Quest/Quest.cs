using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest:MonoBehaviour
{
    public string questName = "default quest name";
    public string questDescription = "default quest description";
    public string questID = "default quest ID";
    public bool completed = false;
    public bool onGoing = false;
    public string GetName() { return questName; }
    public string GetDescription() { return questDescription; }
    public string GetID() { return questID; }
    public bool IsCompleted() { return completed; }
    public bool IsGoing() { return onGoing; }
    public virtual void StartQuest() { print("starting quest " + questName); onGoing = true; }
    public virtual void CompleteQuest() { completed = true; onGoing = false; } 
    private void Start()
    {
        gameObject.name= questName;
    }
}
