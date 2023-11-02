using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Hypertonic.GridPlacement;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class CropGrowth : BuildingInteractable, IResourceSetProvider
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


        //[Header("Objects & Actions")]
        public DoAction InitAction = DoAction.ENABLE;
        public DoAction FinishAction = DoAction.DISABLE;

        public GameObject dryObj = null;
        public GameObject wetObj = null;

        public ResourceSet waterCost = null;
        public float GrowthTime;

        //[HideInInspector]
        public Action OnGrowthComplete;

        // 新增的倒计时协程
        // counting related
        private Coroutine growthCoroutine;

        public bool IsCountingDown { get; private set; } = false;
        public float TotalGrowthTime => GrowthTime;
        private float countdownStartTime;

        public float timeToClear;

        public float ElapsedTime
        {
            get
            {
                if (!IsCountingDown) return 0;
                return (Time.time - countdownStartTime);
            }
        }

        public GameObject growParticle;
        public GameObject waterParticle;

        public bool Water(MonoBehaviour runner)
        {
            if (IsCountingDown) return false; // 如果正在倒计时，返回false

            if (waterCost != null)
            {
                if (!waterCost.SpendResource())
                {
                    return false;
                }
            }
            AdjustObjToAction(dryObj, FinishAction);
            AdjustObjToAction(wetObj, InitAction);

            IsCountingDown = true;
            countdownStartTime = Time.time;
            // 开始倒计时
            if (growthCoroutine != null)
            {
                Debug.Log("Still in counting!");
                runner.StopCoroutine(growthCoroutine);
                //return false;
            }
            growthCoroutine = runner.StartCoroutine(GrowthCountdown(runner));
            return true;
        }

        public void Initialize()
        {


            AdjustObjToAction(dryObj, InitAction); // enable
            AdjustObjToAction(wetObj, FinishAction); //disable
        }

        public void Finish()
        {
            AdjustObjToAction(dryObj, FinishAction); // enable
            AdjustObjToAction(wetObj, FinishAction); //disable
        }

        private IEnumerator GrowthCountdown(MonoBehaviour runner)
        {
            yield return new WaitForSeconds(GrowthTime); // 等待设定的生长时间

            IsCountingDown = false;
            AdjustObjToAction(wetObj, FinishAction); // 应用结束动作

            OnGrowthComplete?.Invoke(); // 触发成长完成事件
        }


        private void AdjustObjToAction(GameObject go, DoAction da)
        {
            if (go == null) return;
            if (da == DoAction.NONE)
            {
                return;
            }
            else if (da == DoAction.ENABLE)
            {
                go.SetActive(true);
            }
            else if (da == DoAction.DISABLE)
            {
                go.SetActive(false);
            }
            else if (da == DoAction.DESTROY)
            {
                Destroy(go);
            }
        }

        public void SetUpUnlockCost(ResourceSet resourceSet)
        {
            waterCost = resourceSet;
        }
    }

    [SerializeField] List<UnlockState> allUnlockStates;

    private int currentState = 0;

    public UnityEvent matureRewardEvent;
    public UnityEvent stateChangeEvent;

    public FinishedAction finishedAction = FinishedAction.RESET;

    [Header("Rewards")]
    public ItemScriptableObject rewardObjects;
    public int rewardAmount;

    public enum FinishedAction
    {
        //NONE,
        RESET,
        RESET_WATER,
        DISABLE,
        DESTROY
    }

    public int GetCurrentState()
    {
        return currentState;
    }

    protected virtual void Start()
    {
        for (int i = 0; i < allUnlockStates.Count; i++)
        {
            int stateIndex = i; // 为了在闭包中捕获
            allUnlockStates[i].Finish();
            allUnlockStates[i].OnGrowthComplete += () =>
            {
                HandleGrowthCompletion(stateIndex);
            };
        }
        InitializeCrop(0);
        //allUnlockStates[0].Initialize();
    }

    private void InitializeCrop(int num)
    {
        allUnlockStates[num].Initialize();

        if (allUnlockStates[num].timeToClear > 0)
        {
            currentInteraction = new InteractionType(InteractionType.TypeName.LongPress, () => Water());
        } else
        {
            currentInteraction = new InteractionType(InteractionType.TypeName.Click, () => Water());
        }
        currentInteraction.pressDuration = allUnlockStates[num].timeToClear;

        //Debug.LogWarning(num);

        if (num == allUnlockStates.Count - 1 )
        {
            onCropMatured();
        }
        else if (num < allUnlockStates.Count - 1)
        {
            onStateChange();
        }
        else
        {
            Debug.LogWarning("Out of the state!");
        }
    }

    protected virtual void onStateChange()
    {
        if (stateChangeEvent != null) stateChangeEvent.Invoke();
    }

    protected virtual void onCropMatured()
    {

    }

    protected virtual void HandleGrowthCompletion(int stateIndex)
    {
        PlayGrowthParticle(GetCurrentUnlockState().growParticle);
        if (stateIndex == allUnlockStates.Count - 1)
        {
            //Debug.Log("HandleGrowthCompletion to 0");

            if (rewardObjects != null) Inventory.i.AddInventoryItem(rewardObjects,rewardAmount);
            //if (matureRewardEvent != null) matureRewardEvent.Invoke();

            //if (finishedAction == FinishedAction.NONE)
            //{
            //    allUnlockStates[stateIndex].Finish();
            //    allUnlockStates[stateIndex].Finish();

            //}
            //else
            if (finishedAction == FinishedAction.RESET)
            {
                allUnlockStates[stateIndex].Finish(); // Disable / Destroy / 重置到第一个状态

                currentState = 0;
                InitializeCrop(0);
                //allUnlockStates[0].Initialize();
            }
            else if (finishedAction == FinishedAction.RESET_WATER)
            {
                allUnlockStates[stateIndex].Finish();

                currentState = 0;
                InitializeCrop(0);
                //allUnlockStates[0].Initialize();

                Water();
            }
            else if (finishedAction == FinishedAction.DISABLE)
            {
                gameObject.SetActive(false);
            } else if (finishedAction == FinishedAction.DESTROY)
            {
                GridManagerAccessor.GridManager.DeleteObject(gameObject);
            }

        }
        else
        {
            //Debug.Log("HandleGrowthCompletion");

            UnlockToNextState(); // 否则，转到下一个状态
        }

    }

    public void PlayGrowthParticle(GameObject particlePrefab)
    {
        Vector3 position = transform.position;
        position.y += 0.5f;
        Instantiate(particlePrefab, position, Quaternion.identity);
    }


    public bool Water()
    {
        //Debug.Log("Watering state: " + currentState);
        if (allUnlockStates[currentState].Water(this) == false)
        {
            NotEnoughResource();
            return false;
        }

        return true;
    }

    public bool UnlockToNextState()
    {
        if (currentState == allUnlockStates.Count)
        {
            Debug.LogWarning("Try to exceed max amount of states with object " + gameObject.name);
            return false;
        }
        //if (allUnlockStates[currentState].IsCountingDown) return false; // 检查是否正在倒计时

        currentState++;



        //allUnlockStates[currentState].Initialize();
        InitializeCrop(currentState);
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

    protected UnlockState GetUnlockState(int stateNum)
    {
        return allUnlockStates[stateNum];
    }

    protected UnlockState GetCurrentUnlockState() { return allUnlockStates[currentState]; }

    protected virtual void NotEnoughResource()
    {
        transform.DOShakePosition(0.5f, new Vector3(0.3f, 0, 0), 10, 0);
    }


    public bool IsCurrentlyCountingDown()
    {
        return GetCurrentUnlockState().IsCountingDown;
    }

    public float CurrentTotalGrowthTime()
    {
        return GetCurrentUnlockState().TotalGrowthTime;
    }

    public float CurrentElapsedTime()
    {
        return GetCurrentUnlockState().ElapsedTime;
    }


    #region IResourceSetProvider

    public virtual ResourceSet ProvideResourceSet(int index = 0)
    {
        if (index >= allUnlockStates.Count)
        {
            Debug.LogError("LevelUp's IResourceSetProvider being requested a out of range index.");
            return null;
        }
        return allUnlockStates[index].waterCost;
    }


    #endregion
}
