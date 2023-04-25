using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public virtual void AssignQuest()
    {

    }

    public virtual bool CheckCompletion()
    {
        return false;
    }

    public virtual void CompleteQuest()
    {

    }
}
