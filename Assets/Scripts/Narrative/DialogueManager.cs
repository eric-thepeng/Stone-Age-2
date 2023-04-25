using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    NarrativeSequence currentNS = null;
    int currentLine = 0;

    public NarrativeSequence testNS = null;

    GameObject backgroundShadingGO = null;
    GameObject dialogueGO = null;
    GameObject nextLineIndicationGO = null;
    TextMeshPro lineDispaly = null;

    public bool performing { get { return currentNS != null; } }
    bool loggingLine = false;
    bool onHoldForProgressEvent;

    private void Start()
    {
        backgroundShadingGO = transform.Find("Background Shading").gameObject;
        dialogueGO = transform.Find("Dialogue").gameObject;
        lineDispaly = dialogueGO.transform.Find("Dialogue Text").GetComponent<TextMeshPro>();
        nextLineIndicationGO = dialogueGO.transform.Find("Next Line Indication").gameObject;

        QueueNarrativeSequence(testNS);
    }

    private void Update()
    {
        if (!performing) return;
        if(!loggingLine && Input.GetMouseButtonDown(0)) PerformNextLine();
    }

    public bool QueueNarrativeSequence(NarrativeSequence sequence)
    {
        print("que narrative sequence");
        if (performing) return false;
        print("here");
        currentNS = sequence;
        currentLine = 0;
        StartPerforming();
        return true;
    }

    void StartPerforming()
    {
        print("start perform");
        dialogueGO.SetActive(true);
        backgroundShadingGO.SetActive(true);
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
        dialogueGO.SetActive(false);
        backgroundShadingGO.SetActive(false);
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
}
