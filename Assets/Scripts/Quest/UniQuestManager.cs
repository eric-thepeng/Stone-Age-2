using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniQuestManager : MonoBehaviour
{
    static UniQuestManager instance=null;
    public static UniQuestManager i
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<UniQuestManager>();
            }
            return instance;
        }
    }

    private Queue<UniQuest> uniQuests;
    private UniQuest currentUniQuest;

    private void Awake()
    {
        uniQuests = new Queue<UniQuest>();
        currentUniQuest = null;
    }
    
    public void QueUniQuest(UniQuest newUniQuest)
    {
        uniQuests.Enqueue(newUniQuest);
        if (uniQuests.Count == 1 && currentUniQuest == null)
        {
            currentUniQuest = uniQuests.Dequeue();
            currentUniQuest.uniActionSequence.onActionCompletes.AddListener(CurrentUniQuestCompletes);
            currentUniQuest.uniActionSequence.PerformAction();
        }
    }

    public void CurrentUniQuestCompletes()
    {
        if (uniQuests.Count > 0)
        {
            currentUniQuest = uniQuests.Dequeue();
            currentUniQuest.uniActionSequence.onActionCompletes.AddListener(CurrentUniQuestCompletes);
            currentUniQuest.uniActionSequence.PerformAction();
        }
        else
        {
            currentUniQuest = null;
        }
    }
}
