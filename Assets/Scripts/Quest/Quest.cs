using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest:MonoBehaviour
{
    
    public string questName = "default quest name";
    public string questDescription = "default quest description";
    public string questID = "default quest ID";
    public string narrativeSequenceIDToQue = "No Narrative Sequence";
    public float startDelay = 0f;
    public float endDelay = 1f;


    public bool completed = false;
    public bool onGoing = false;

    public string GetName() { return questName; }
    public string GetDescription() { return questDescription; }
    public string GetID() { return questID; }
    public bool IsCompleted() { return completed; }
    public bool IsGoing() { return onGoing; }
    public string GetSystemDisplayName() { return questID + " "+ GetName();}

    private void Start()
    {
        gameObject.name = GetSystemDisplayName();
    }

    public void QueQuest()
    {
        StartCoroutine(CorQueQuest());
    }

    private IEnumerator CorQueQuest()
    {
        yield return new WaitForSeconds(startDelay);
        StartQuest();
    }

    private void QueEndingNS()
    {
        if (!narrativeSequenceIDToQue.Equals("No Narrative Sequence")) { StartCoroutine(CorQueEndingNS()); }
    }

    private IEnumerator CorQueEndingNS()
    {
        yield return new WaitForSeconds(endDelay);
        //DialogueManager.i.QueueNarrativeSequence(narrativeSequenceIDToQue);
    }

    protected virtual void StartQuest() {
        UI_Quest.i.SetUpDisplay(this);
        onGoing = true; 
    }
    protected virtual void CompleteQuest() { 
        completed = true; 
        onGoing = false;
        UI_Quest.i.CloseDisplay(this);
        QueEndingNS(); 
    }


}
