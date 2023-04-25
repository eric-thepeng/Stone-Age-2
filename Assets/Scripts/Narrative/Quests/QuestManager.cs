using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    static QuestManager instance = null;
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

    List<Quest> onGoingQuests = new List<Quest>();

    private void Update()
    {
        for(int i = onGoingQuests.Count-1; i>=0; i--)
        {
            if (onGoingQuests.Count == 0) return;
            if (onGoingQuests[i].CheckCompletion())
            {
                Quest temp = onGoingQuests[i];
                onGoingQuests.RemoveAt(i);
                temp.CompleteQuest();
            }
        }
    }

    public void AddNewQuest(Quest newQuest)
    {
        if (IsQuestOngoing(newQuest)) return;
        onGoingQuests.Add(newQuest);
    }

    public bool IsQuestOngoing(Quest q)
    {
        if(onGoingQuests.Contains(q)) return true;
        return false;
    }
}
