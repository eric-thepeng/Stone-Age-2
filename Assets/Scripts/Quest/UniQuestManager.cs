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

    public List<UniQuest> currentUniQuests;

    private void Awake()
    {
        currentUniQuests = new List<UniQuest>();
    }
    
    public void QueUniQuest(UniQuest newUniQuest)
    {
        currentUniQuests.Add(newUniQuest);
        LoadUniQuest(newUniQuest);
    }

    public void CompleteUniQuest(UniQuest uniQuest)
    {
        if (currentUniQuests.Contains(uniQuest))
        {
            currentUniQuests.Remove(uniQuest);
        }
        else
        {
            Debug.LogWarning("A complete uniQuest not in uniQuestManager, game object: " + uniQuest.gameObject.name);
        }
    }

    private void LoadUniQuest(UniQuest uniQuest)
    {
        currentUniQuests.Add(uniQuest);
        uniQuest.uniActionSequence.SetSelfUniQuest(uniQuest);
        uniQuest.uniActionSequence.PerformAction();
        //uniQuest.uniActionSequence.onActionCompletes.AddListener();
    }
}
