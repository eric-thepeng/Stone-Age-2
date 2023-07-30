using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelUp : WorldInteractable, IResourceSetProvider
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

        public void SetUpUnlockCost(ResourceSet resourceSet)
        {
            unlockCost = resourceSet;
        }
    }

    [SerializeField] List<UnlockState> allUnlockStates;
    private int currentState = 0;

    public int GetCurrentState()
    {
        return currentState;
    }

    protected UnlockState GetUnlockState(int stateNum)
    {
        return allUnlockStates[stateNum];
    }

    public bool UnlockToNextState()
    {
        if (currentState == allUnlockStates.Count)
        {
            Debug.LogWarning("Try to exceed max amount of states with object " + gameObject.name);
            return false;
        }
        if (allUnlockStates[currentState].Unlock() == false)
        {
            NotEnoughResource();
            return false;
        }
        currentState++;
        if(currentState == allUnlockStates.Count) ReachFinalState();
        return true;
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

    protected UnlockState GetCurrentUnlockState() { return allUnlockStates[currentState]; }

    protected virtual void NotEnoughResource()
    {
        transform.DOShakePosition(0.5f, new Vector3(0.3f,0,0),10,0);
    }

    protected virtual void ReachFinalState()
    {
        
    }
    
    #region IResourceSetProvider

    public virtual ResourceSet ProvideResourceSet(int index = 0)
    {
        if (index >= allUnlockStates.Count)
        {
            Debug.LogError("LevelUp's IResourceSetProvider being requested a out of range index.");
            return null;
        }
        return allUnlockStates[index].unlockCost;
    }


    #endregion
}
