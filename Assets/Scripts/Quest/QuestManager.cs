using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    static QuestManager instance;

    public static QuestManager i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<QuestManager>();
            }
            return instance;
        }
    }

    Dictionary<string, Quest> allQuestDictionary = new Dictionary<string, Quest>();
    private void Awake()
    {
        foreach(Quest q in GetComponentsInChildren<Quest>())
        {
            allQuestDictionary.Add(q.GetID(),q); 
        }
    }

    public void StartQuestByID(string id)
    {
        if (!allQuestDictionary.ContainsKey(id)) {
            Debug.LogError("Does not contain Quest of ID: " + id);
            return; 
        }
        StartQuest(allQuestDictionary[id]);
    }

    private void StartQuest(Quest newQuest)
    {
        if (newQuest.IsCompleted() || newQuest.IsGoing()) return;
        newQuest.StartQuest();
    }
}
