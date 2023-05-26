using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : WorldInteractable
{
    [Serializable]
    protected class UnlockState
    {
        public enum DoAction
        {
            NONE,
            DESTROY,
            ENABLE,
            DISABLE
        }

        public GameObject orgObj = null;
        public DoAction orgObjAction = DoAction.DISABLE;
        public GameObject newObj = null;
        public DoAction newObjAction = DoAction.ENABLE;
        public ResourceSet unlockCost = null;

        public bool Unlock()
        {
            if (unlockCost != null)
            {
                if (!unlockCost.SpendResource())
                {
                    return false;
                }
            }
            AdjustObjToAction(orgObj, orgObjAction);
            AdjustObjToAction(newObj, newObjAction);
            return true;
        }

        private void AdjustObjToAction(GameObject go, DoAction da)
        {
            if(go == null) return;
            if (da == DoAction.NONE)
            {
                return;
            }else if (da == DoAction.ENABLE)
            {
                go.SetActive(true);
            }else if (da == DoAction.DISABLE)
            {
                go.SetActive(false);
            }else if (da == DoAction.DESTROY)
            {
                Destroy(go);
            }
        }
    }

    [SerializeField] List<UnlockState> allUnlockStates;
    private int currentState = 0;

    public int GetCurrentState()
    {
        return currentState;
    }

    public void UnlockToNextState()
    {
        if (currentState == allUnlockStates.Count)
        {
            Debug.LogWarning("Try to exceed max amount of states with object " + gameObject.name);
            return;
        }
        if (allUnlockStates[currentState].Unlock() == false)
        {
            NotEnoughResource();
            return;
        }
        currentState++;
        if(currentState == allUnlockStates.Count) ReachFinalState();
    }

    public void UnlockToState(int targetState)
    {
        if (targetState >= allUnlockStates.Count)
        {
            Debug.LogWarning("Try to exceed max amount of states with object " + gameObject.name);
            return;
        }

        for (int i = currentState; i < targetState; i++)
        {
            UnlockToNextState();
        }
    }

    protected virtual void NotEnoughResource()
    {
        
    }

    protected virtual void ReachFinalState()
    {
        
    }
}
