using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    //Current NS Info
    NarrativeSequenceAction currentNSA = null;
    int currentLine = 0;
    
    //Current State
    public bool performing { get { return currentNSA != null; } }
    bool loggingLine = false;

    //Game Objects and UIs
    [Header("DO NOT EDIT BELOW")][SerializeField]GameObject dialogueGO;
    [SerializeField]GameObject nextLineIndicationGO;
    [SerializeField]TextMeshPro lineDispaly;

    /* Debug
    [SerializeField] private bool debugMode = false;
    [SerializeField] private string debugQueNS = "";
    */
    
    static DialogueManager instance;
    public static DialogueManager i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DialogueManager>();
            }
            return instance;
        }
    }
    
    

    private void Start()
    {
        /*
        dialogueGO = transform.Find("Dialogue").gameObject;
        lineDispaly = dialogueGO.transform.Find("Dialogue Text").GetComponent<TextMeshPro>();
        nextLineIndicationGO = dialogueGO.transform.Find("Next Line Indication").gameObject;*/
        /*
        if(!debugMode)QueueNarrativeSequence(GetNarrativeSequenceByID("NS_0001"));
        else
        {
            if(!debugQueNS.Equals("none")) QueueNarrativeSequence(GetNarrativeSequenceByID(debugQueNS));
        }*/
    }
    
    private void OnMouseUpAsButton()
    {
        if (!performing) return;

        if (loggingLine)
        {
            LogLineImmediately(currentNSA.narrativeSequenceToPlay.GetLine(currentLine));
        }
        else
        {
            PerformNextLine();
        }
        
    }


    /// <summary>
    /// Trigger a QuestDialogue
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    public bool QueueNarrativeSequence(NarrativeSequenceAction narrativeSequenceAction)
    {
        if (performing) return false;
        currentNSA = narrativeSequenceAction;
        currentLine = 0;
        StartPerforming();
        return true;
    }
    

    void StartPerforming()
    {
        print("start perform");
        dialogueGO.transform.DOLocalMove(new Vector3(0, 0, 0), 1f);
        //UI_FullScreenShading.i.ShowDialogueShading();
        PerformLine(currentLine);
    }

    void PerformLine(int line)
    {
        StartCoroutine(LogLine(currentNSA.narrativeSequenceToPlay.GetLine(line)));
    }

    void PerformNextLine()
    {
        if(currentNSA.narrativeSequenceToPlay.HasLine(currentLine+1))
        {
            currentLine++;
            PerformLine(currentLine);
        }
        else
        {
            EndPerforming();
        }
    }

    void EndPerforming()
    {
        dialogueGO.transform.DOLocalMove(new Vector3(-10, 0, 0), 1f).onComplete = currentNSA.onActionCompletes.Invoke;
        currentNSA = null; 
    }

    IEnumerator LogLine(string lineText)
    {
        loggingLine = true;
        nextLineIndicationGO.SetActive(false);
        int byteNow = 0;
        while (byteNow <= lineText.Length)
        {
            lineDispaly.text = lineText.Substring(0, byteNow);
            byteNow++;
            yield return new WaitForSeconds(0.01f);
        }
        loggingLine = false;
        nextLineIndicationGO.SetActive(true);
        
    }

    void LogLineImmediately(string lineText)
    {
        StopAllCoroutines();
        loggingLine = false;
        nextLineIndicationGO.SetActive(true);
        lineDispaly.text = lineText;
    }
}
