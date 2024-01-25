using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceRespawnManager : MonoBehaviour
{
    // [Serializable]
    // protected class RespawnRule
    // {
    //     public List<GameObject> respawnPrefabList;
    //     public PlaceableObject.BiomeType availableTileType;
    //     private int maxiumAmount = 3;
    //     private Vector2 interval = new Vector2(10f,10f);
    //
    //     private List<GameObject> spawnedObjects = new List<GameObject>();
    //     private BoxCollider obstacleCollider;
    //     
    //     // 新增的倒计时协程
    //     // counting related
    //     private Coroutine growthCoroutine;
    //
    //     public bool IsCountingDown { get; private set; } = false;
    //     public float TotalGrowthTime => GrowthTime;
    //     private float countdownStartTime;
    //
    //     public float timeToClear;
    //
    //     public float ElapsedTime
    //     {
    //         get
    //         {
    //             if (!IsCountingDown) return 0;
    //             return (Time.time - countdownStartTime);
    //         }
    //     }
    //
    //     [Header("Particles")]
    //     public GameObject finishedParticle;
    //     public GameObject overtimeParticle;
    //     public GameObject finishedWaterParticle;
    //
    //
    //     [Header("Icon")]
    //     public Sprite interactableIcon;
    //
    //     public bool Water(MonoBehaviour runner)
    //     {
    //         if (IsCountingDown) return false; // 如果正在倒计时，返回false
    //
    //         if (waterCost != null)
    //         {
    //             if (!waterCost.SpendResource())
    //             {
    //                 return false;
    //             }
    //         }
    //
    //
    //         AdjustObjToAction(dryObj, FinishAction);
    //         AdjustObjToAction(wetObj, InitAction);
    //
    //         IsCountingDown = true;
    //         countdownStartTime = Time.time;
    //         // 开始倒计时
    //         if (growthCoroutine != null)
    //         {
    //             Debug.Log("Still in counting!");
    //             runner.StopCoroutine(growthCoroutine);
    //             //return false;
    //         }
    //         growthCoroutine = runner.StartCoroutine(GrowthCountdown(runner));
    //         return true;
    //     }
    //
    //     public void Initialize()
    //     {
    //
    //
    //         AdjustObjToAction(dryObj, InitAction); // enable
    //         AdjustObjToAction(wetObj, FinishAction); //disable
    //     }
    //
    //     public void Finish()
    //     {
    //         AdjustObjToAction(dryObj, FinishAction); // enable
    //         AdjustObjToAction(wetObj, FinishAction); //disable
    //     }
    //
    //     private IEnumerator GrowthCountdown(MonoBehaviour runner)
    //     {
    //         yield return new WaitForSeconds(GrowthTime); // 等待设定的生长时间
    //
    //         IsCountingDown = false;
    //         AdjustObjToAction(wetObj, FinishAction); // 应用结束动作
    //
    //         OnGrowthComplete?.Invoke(); // 触发成长完成事件
    //     }
    //
    //
    //     private void AdjustObjToAction(GameObject go, DoAction da)
    //     {
    //         if (go == null) return;
    //         if (da == DoAction.NONE)
    //         {
    //             return;
    //         }
    //         else if (da == DoAction.ENABLE)
    //         {
    //             go.SetActive(true);
    //         }
    //         else if (da == DoAction.DISABLE)
    //         {
    //             go.SetActive(false);
    //         }
    //         else if (da == DoAction.DESTROY)
    //         {
    //             Destroy(go);
    //         }
    //     }
    //
    //     public void SetUpUnlockCost(ResourceSet resourceSet)
    //     {
    //         waterCost = resourceSet;
    //     }
    // }
    //
    // [SerializeField] List<UnlockState> allUnlockStates;
    //
    // private int currentState = 0;
    //
    // void Start()
    // {
    //     
    // }
    //
    // // Update is called once per frame
    // void Update()
    // {
    //     
    // }
}
