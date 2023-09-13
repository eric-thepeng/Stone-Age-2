using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    NarrativeSequence currentNS = null;
    int currentLine = 0;

    GameObject dialogueGO = null;
    GameObject nextLineIndicationGO = null;
    TextMeshPro lineDispaly = null;

    public bool performing { get { return currentNS != null; } }
    bool loggingLine = false;

    [SerializeField] NarrativeSequenceList NSList;
    Dictionary<string, NarrativeSequence> allNSDictionary = new Dictionary<string, NarrativeSequence>();

    [SerializeField] private bool debugMode = false;
    [SerializeField] private string debugQueNS = "";

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

    private void Awake()
    {
        foreach(NarrativeSequence ns in NSList.data)
        {
            allNSDictionary.Add(ns.narrativeSequenceID, ns);
        }
    }

    private void Start()
    {
        dialogueGO = transform.Find("Dialogue").gameObject;
        lineDispaly = dialogueGO.transform.Find("Dialogue Text").GetComponent<TextMeshPro>();
        nextLineIndicationGO = dialogueGO.transform.Find("Next Line Indication").gameObject;
        
        if(!debugMode)QueueNarrativeSequence(GetNarrativeSequenceByID("NS_0001"));
        else
        {
            if(!debugQueNS.Equals("none")) QueueNarrativeSequence(GetNarrativeSequenceByID(debugQueNS));
        }
    }

    private void Update()
    {
        if (!performing) return;
        if (Input.GetMouseButtonDown(0))
        {
            if (loggingLine)
            {
                LogLineImmediately(currentNS.GetLine(currentLine));
            }
            else
            {
                PerformNextLine();
            }
        }
    }
    
    public NarrativeSequence GetNarrativeSequenceByID(string id)
    {
        if (!allNSDictionary.ContainsKey(id)) Debug.Log("Does not contain Narrative Sequence of ID: " + id);
        return allNSDictionary[id];
    }

    public bool QueueNarrativeSequence(NarrativeSequence sequence)
    {
        if (performing) return false;
        currentNS = sequence;
        currentLine = 0;
        StartPerforming();
        return true;
    }

    public bool QueueNarrativeSequence(string NSID)
    {
        return QueueNarrativeSequence(GetNarrativeSequenceByID(NSID));
    }

    void StartPerforming()
    {
        print("start perform");
        dialogueGO.SetActive(true);
        UI_FullScreenShading.i.ShowDialogueShading();
        PerformLine(currentLine);
    }

    void PerformLine(int line)
    {
        StartCoroutine(LogLine(currentNS.GetLine(line)));
    }

    void PerformNextLine()
    {
        if(currentNS.HasLine(currentLine+1))
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
        if (currentNS.HasQuest()) { QuestManager.i.StartQuestByID(currentNS.questIDToQue); }
        dialogueGO.SetActive(false);
        UI_FullScreenShading.i.HideShading();
        currentNS = null; 
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
