using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hypertonic.GridPlacement;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

public class CharacterBehaviors : MonoBehaviour
{
    public enum HomeState { 
        Unset,
        Sleeping1,
        Feeding,
        Sleeping2,
        Interacting,
        Working,
        HangingAround,
        Exploring
        
        // Resting,
        // Gatherable,
        // Gathering
    }

    private Character character;

    [SerializeField]
    private HomeState currentState;

    public HomeState CurrentState
    {
        get => currentState;
        set => currentState = value;
    }

    [SerializeField]
    private CharacterMovement characterMovement;

    private float moveSpeed = 1f;
    [HideInInspector]
    public BoxCollider hangOutArea;
    private float hangOutWaitTime = 2f; // 停顿时间

    public enum CharacterState {Idle, Gather}
    public CharacterState state = CharacterState.Idle;


    // public float RestTimeLeft
    // {
    //     get => restTimeLeft;
    //     set => restTimeLeft = value;
    // }



    private Vector3 _l2dCharacterOldPosition;
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Character>();
        moveSpeed = character.GetMoveSpeed();
        hangOutWaitTime = character.GetHangOutWaitTime();

        // l2dCharacter = Instantiate(character.GetL2dGameObject());
        if (character.l2dCharacter == null) Debug.LogError("Character " + character.GetCharacterName() + "'s L2dCharacter is missing!");

        characterMovement = character.l2dCharacter.GetComponent<CharacterMovement>();
        if (characterMovement == null)
        {
            characterMovement = character.l2dCharacter.AddComponent<CharacterMovement>();
        }
        
        _l2dCharacterOldPosition = characterMovement.transform.GetChild(0).localPosition;

        characterMovement.CharacterBehavior = this;
        //Debug.Log(characterMovement.transform.name);

        characterMovement.moveSpeed = moveSpeed;
            // characterMovement.hangOutWaitTime = hangOutWaitTime;

        if (hangOutArea != null) {
            hangOutArea.enabled = true;
            characterMovement.hangOutAreaMin = hangOutArea.bounds.min; 
            characterMovement.hangOutAreaMax = hangOutArea.bounds.max;
        } else
        {
            Debug.LogWarning("HangoutArea" + hangOutArea + " is null");
        }
        

        hangOutArea.enabled = false;

        
        currentState = HomeState.HangingAround;
        
        EnterState(currentState);
        character.CharacterIcon.SetGatheringProgress(0, 100 * character.CharacterStats.energy.RemainEnergyPercentage(), false);
        
        // if (character.CharacterStats.energy.EnergyLessThanRestingPercentage())
        // {
        //     currentState = HomeState.Resting;
        //     characterMovement.StartSleeping();
        // } else
        // {
        //     currentState = HomeState.Gatherable;
        //     characterMovement.StopSleeping();
        //     characterMovement.StartHangingOut();
        //
        // }
    }


    [SerializeField]
    private bool _isPendingTowardsTarget = false;

    public bool IsPendingTowardsTarget
    {
        get => _isPendingTowardsTarget;
        set => _isPendingTowardsTarget = value;
    }

    float gatherTimeLeft;

    public float GatherTimeLeft
    {
        get => gatherTimeLeft;
        set => gatherTimeLeft = value;
    }

    [SerializeField]
    float periodTimeLeft = 0; // period for execute
    
    void Update()
    {
        if (state == CharacterState.Gather)
        {
            character.l2dCharacter.SetActive(false);
        } else if (state == CharacterState.Idle) {
            character.l2dCharacter.SetActive(true);
        }
        
        if (state == CharacterState.Gather) // if character is exploring on the map
        {
            if(character.CharacterStats.energy.NoEnergy()) // end exploring when no energy
            {
                state = CharacterState.Idle;
                
                character.EndGatherUI();
                character.GatheringSpot.EndGathering();
                
                // EnterState(HomeState.Resting);
                
            }
            if(gatherTimeLeft <= 0) // else, reduce energy and continue
            {
                character.YieldResource();
                character.DiscoverSpot();
                character.CharacterStats.energy.CostEnergy();
                gatherTimeLeft = character.GatheringSpot.gatherTime;
            }
            //update ui
            gatherTimeLeft -= character.CharacterStats.gatherSpeed.GetCurrentGatherSpeed() * Time.deltaTime;

            character.CharacterIcon.SetGatheringProgress(100 * (1 - (gatherTimeLeft / character.GatheringSpot.gatherTime)), 100 * character.CharacterStats.energy.RemainEnergyPercentage(), true);
            character.GatheringSpot.SetGatheringProgress(100 * (1 - (gatherTimeLeft / character.GatheringSpot.gatherTime)), 100 * character.CharacterStats.energy.RemainEnergyPercentage(), true);
        }
        else if(state == CharacterState.Idle && !_isPendingTowardsTarget && !GridManagerAccessor.GridManager.IsPlacingGridObject) // if character is at home
        {
            
            if (periodTimeLeft <= 0)
            {
                character.CharacterIcon.SetGatheringProgress(0, 100 * character.CharacterStats.energy.RemainEnergyPercentage(), false);
                // character.CharacterStats.energy.AddEnergy();
                
                // periodTimeLeft = 1;
                periodTimeLeft = 1 / character.CharacterStats.restingSpeed.GetRestingSpeed();
                
                CheckState();
                character.CharacterIcon.UpdateUIText(UpdateStatusUIText());

            }
            //update ui
            periodTimeLeft -= 1 * Time.deltaTime;


        } else if (state == CharacterState.Idle && !_isPendingTowardsTarget &&
                   _targetObject != null && _behaviorTargetPosition != _targetObject.transform.position && GridManagerAccessor.GridManager.IsPlacingGridObject) // if character is at home and is placing grid object.position && GridManagerAccessor.GridManager.IsPlacingGridObject)
        {
            _behaviorTargetPosition = _targetObject.transform.position;
            currentState = HomeState.Unset;
        }
    }
    
    private Vector3 _behaviorTargetPosition;

    public void CheckState()
    {
        // Debug.Log("Check State: " + currentState + ", Energy - " + character.CharacterStats.energy.GetCurrentEnergy() +character.CharacterStats.energy.EnergyLessThanRestingPercentage() +  "/"+ character.CharacterStats.energy.GetMaxEnergy() + character.CharacterStats.energy.EnergyLessThanPercentage(1) + ", Saturation - " + character.CharacterStats.saturation.GetCurrentSaturation());
            PlaceableObject[] _nearbyObjects = FindAndSortComponents<PlaceableObject>(transform.position, 50);
            
            // sleeping <25%
            
            
            if (character.CharacterStats.energy.EnergyLessThanRestingPercentage()) 
            {
                characterMovement.moveSpeed = moveSpeed * 0.5f;
                
                if (currentState == HomeState.Sleeping1) return;
                
                PlaceableObject[] _bedObjects = _nearbyObjects.Where(obj => obj.GetBuildingISO().containTag("bed")).ToArray();
                // _behaviorTargetPosition = _bedObject.transform.position;

                PlaceableObject _bedObject = null;
                int count = 0;
                
                while (count < _bedObjects.Length)
                {
                    if (characterMovement.SetTargetPosition(_bedObjects[count].GetInteractionPoint()))
                    {
                        _bedObject = _bedObjects[count];
                        break;
                    }
                    else
                    {
                        count++;
                    }
                }
                
                if (currentState != HomeState.Sleeping1 && _bedObject != null) // if character find bed to go
                {
                    
                    EnterState(HomeState.Sleeping1, _bedObject);
                    return;
                }
                
            }
            
            // food <25%
            
            if (character.CharacterStats.saturation.SaturationLessThanFullPercentage()) 
            {
                // character.CharacterIcon.ChangeIconColorToGather();
                if (currentState == HomeState.Feeding) return;
                
                PlaceableObject[] _foodObjects = _nearbyObjects.Where(obj => obj.GetBuildingISO().containTag("foodContainer")).ToArray();
                // if () {}// if character find food to eat
                
                PlaceableObject _foodObject = null;
                int count = 0;
                while (count < _foodObjects.Length)
                {
                    if (characterMovement.SetTargetPosition(_foodObjects[count].GetInteractionPoint()))
                    {
                        _foodObject = _foodObjects[count];
                        break;
                    }
                    else
                    {
                        count++;
                    }
                }
                
                if (currentState != HomeState.Feeding && _foodObject != null) // if character find bed to go
                {
                    
                    EnterState(HomeState.Feeding, _foodObject);
                    return;
                }
                

            }
            
            if (character.CharacterStats.energy.EnergyLessThanPercentage(1)) // sleeping <100%
            {
                
                if (currentState == HomeState.Sleeping2) return;
                
                PlaceableObject[] _bedObjects = _nearbyObjects.Where(obj => obj.GetBuildingISO().containTag("bed")).ToArray();
                // _behaviorTargetPosition = _bedObject.transform.position;

                PlaceableObject _bedObject = null;
                int count = 0;
                
                while (count < _bedObjects.Length)
                {
                    if (characterMovement.SetTargetPosition(_bedObjects[count].GetInteractionPoint()))
                    {
                        _bedObject = _bedObjects[count];
                        break;
                    }
                    else
                    {
                        count++;
                    }
                }
                
                if (currentState != HomeState.Sleeping2 && _bedObject != null) // if character find bed to go
                {
                    
                    EnterState(HomeState.Sleeping2, _bedObject);
                    return;
                }
            }
            
            // find workingable object
            if (currentState == HomeState.Working) return;
                
            PlaceableObject[] _workingObjects = _nearbyObjects.Where(obj => obj.GetBuildingISO().containTag("workingPlace")).ToArray();
            // if () {}// if character find food to eat
                
            PlaceableObject _workObject = null;
            int workingCount = 0;
            while (workingCount < _workingObjects.Length)
            {
                if (characterMovement.SetTargetPosition(_workingObjects[workingCount].GetInteractionPoint()))
                {
                    _workObject = _workingObjects[workingCount];
                    break;
                }
                else
                {
                    workingCount++;
                }
            }
                
            if (currentState != HomeState.Working && _workObject != null) // if character find bed to go
            {
                    
                EnterState(HomeState.Working, _workObject);
                return;
            }

            
            // find interacting object
            if (currentState == HomeState.Interacting) return;
            
            PlaceableObject[] _interactingObjects = _nearbyObjects.Where(obj => obj.GetBuildingISO().containTag("interactable")).ToArray();
            
            PlaceableObject[] favoriteObjects = _interactingObjects.Where(obj => character.InitialStats.containsFavoriteBuilding(obj.GetBuildingISO())).ToArray();
            PlaceableObject[] nonFavoriteObjects = _interactingObjects.Where(obj => !character.InitialStats.containsFavoriteBuilding(obj.GetBuildingISO())).ToArray();

            PlaceableObject _interactingObject = null;
            int interactableCount = 0;

// 先尝试寻找并处理满足containsFavoriteBuilding条件的对象
            while (interactableCount < favoriteObjects.Length)
            {
                if (characterMovement.SetTargetPosition(favoriteObjects[interactableCount].GetInteractionPoint()))
                {
                    _interactingObject = favoriteObjects[interactableCount];
                    break;
                }
                else
                {
                    interactableCount++;
                }
            }

// 如果没有找到满足条件的对象，再遍历剩余的对象
            if (_interactingObject == null)
            {
                interactableCount = 0; // 重置计数器
                while (interactableCount < nonFavoriteObjects.Length)
                {
                    if (characterMovement.SetTargetPosition(nonFavoriteObjects[interactableCount].GetInteractionPoint()))
                    {
                        _interactingObject = nonFavoriteObjects[interactableCount];
                        break;
                    }
                    else
                    {
                        interactableCount++;
                    }
                }
            }
                // Debug.Log("Interacting object: " + _interactingObject + ", count: " + interactableCount + ", favorite count: " + favoriteObjects.Length + ", non-favorite count: " + nonFavoriteObjects.Length);
            if (currentState != HomeState.Interacting && _interactingObject != null) // if character find interactable object to go
            {
                    
                EnterState(HomeState.Interacting, _interactingObject);
                return;
            }
            
            if (currentState == HomeState.HangingAround) return;
            EnterState(HomeState.HangingAround);


    }
    
    
    public T[] FindAndSortComponents<T>(Vector3 center, float searchRadius) where T : Component
    {
        // 使用 Physics.OverlapSphere 查找所有碰撞体
        Collider[] hitColliders = Physics.OverlapSphere(center, searchRadius);
        // 从碰撞体中获取指定类型的组件，并过滤掉没有该组件的对象
        var components = hitColliders.Select(collider => collider.GetComponent<T>())
            .Where(component => component != null)
            .ToArray();

        // 按与中心点的距离排序
        return components.OrderBy(component => (component.transform.position - center).sqrMagnitude).ToArray();
    }

    [SerializeField]
    private PlaceableObject _targetObject;
    
    public void EnterState(HomeState state, PlaceableObject targetObject = null)
    {
        
        // Debug.Log("Enter state " + state + ", target " + targetObject);
        ExitState();
        currentState = state;
        // isInWorkingState = false;
        
        if (characterMovement == null) return;

        if (targetObject != null)
        {
            _targetObject = targetObject;
            characterWorkingEvent = targetObject.InvokeFinishedWorkEvent;
            workingDuration = targetObject.TaskDuration;

            targetObject.OccupiedCharacter = character;
            targetObject.IsOccupiedByCharacter = true;
        }
        
        // set target position
        // set player status: finding path
        // set target status (pending occupied)
        // set current event to 'calculate distance form player to target'
        // execute 'calculate player distance from target event'
            // if close to target, set occupy status to true
            // set current event to cyclically execute target working event
        
            IsPendingTowardsTarget = true;
            
        if (state == HomeState.HangingAround)
        {
            
            if (hangingoutCorountine != null)
            {
                // Debug.Log("Still in counting!");
                StopCoroutine(hangingoutCorountine);
                
                characterMovement.StopHangingOut();
                //return false;
            }
            hangingoutCorountine = StartCoroutine(HangingAroundCountdown());
            // start character moving
            // when character arrive destination, start sleeping

            // character.CharacterIcon.ChangeIconColorToHome();
        }
        else
        {
            characterMovement.StartHangingOut();
        }
        
    }

    // private bool isInWorkingState = false;

    public void EnterWorkingState()
    {
        // isInWorkingState = true;
        
        if (currentState == HomeState.Sleeping1 || currentState == HomeState.Sleeping2)
        {
            characterMovement.StopHangingOut();
            
            characterMovement.animator.ResetTrigger("Stand");
            
            characterMovement.StartSleeping();

            // _l2dCharacterOldPosition = characterMovement.transform.GetChild(1).position;
            characterMovement.transform.GetChild(0).position = _targetObject.transform.position + new Vector3(0f, 2.8f, 0);
            // Debug.Log("Set sit");
            // characterMovement.animator.SetTrigger("Sit");

            characterWorkingEvent = () =>
            {
                character.CharacterStats.energy.AddEnergy();
            };

        } else if (currentState == HomeState.Feeding) {
            characterMovement.StopHangingOut();
            // characterMovement.StartSleeping();
            
            // Debug.Log("Set crafting true");
            characterMovement.animator.SetBool("isCrafting", true);

            characterWorkingEvent = () =>
            {
                character.CharacterStats.saturation.AddSaturation();
                BuildingManager.i.ObjectMorphing(_targetObject.transform, GridManagerAccessor.GridManager.GridSettings.animationCurve,
                    GridManagerAccessor.GridManager.GridSettings.animationDuration);
            };

        } else if (currentState == HomeState.Working)
        {
            characterMovement.StopHangingOut();
            // characterMovement.StartSleeping();
            
            // Debug.Log("Set crafting true");
            characterMovement.animator.SetBool("isCrafting", true);
            
            characterWorkingEvent = () =>
            {
                _targetObject.InvokeFinishedWorkEvent();
                
                BuildingManager.i.ObjectMorphing(_targetObject.transform, GridManagerAccessor.GridManager.GridSettings.animationCurve,
                    GridManagerAccessor.GridManager.GridSettings.animationDuration);
            };
        } else if (currentState == HomeState.Interacting)
        {
            characterMovement.StopHangingOut();
            // characterMovement.StartSleeping();
            
            // Debug.Log("Set crafting true");
            characterMovement.animator.SetBool("isCrafting", true);

            characterWorkingEvent = () =>
            {
                
                if (_targetObject != null) BuildingManager.i.ObjectMorphing(_targetObject.transform, GridManagerAccessor.GridManager.GridSettings.animationCurve,
                    GridManagerAccessor.GridManager.GridSettings.animationDuration);
            };
        }

        if (currentState != HomeState.HangingAround)
        {
            if (workingCoroutine != null)
            {
                // Debug.Log("Still in counting!");
                StopCoroutine(workingCoroutine);
                //return false;
            }
            workingCoroutine = StartCoroutine(WorkingCountdown(this));
        }

        
    }

    public void ExitState()
    {
        if (workingCoroutine != null)
        {
            // Debug.Log("Still in counting!");
            StopCoroutine(workingCoroutine);
            //return false;
        }
        if (hangingoutCorountine != null)
        {
            // Debug.Log("Still in counting!");
            StopCoroutine(hangingoutCorountine);
            //return false;
        }

        if (_targetObject != null)
        {
            _targetObject.OccupiedCharacter = null;
            _targetObject.IsOccupiedByCharacter = false;
            
            characterMovement.transform.GetChild(0).localPosition = _l2dCharacterOldPosition;
            // characterMovement.transform.position = _targetObject.GetInteractionPoint();
            
            // Debug.Log("set stand");
            characterMovement.animator.SetTrigger("Stand");
        }
        
        // characterMovement.transform.GetChild(1).localPosition = _l2dCharacterOldPosition;
        
        // Debug.Log("Set crafting false");
        characterMovement.animator.SetBool("isCrafting", false);
        
        characterMovement.moveSpeed = moveSpeed;
        
        _targetObject = null;
        characterWorkingEvent = null;
        characterMovement.StopSleeping();
        characterMovement.StopHangingOut();
        
    }

    public Action characterWorkingEvent; // coroutine
    private Coroutine workingCoroutine;
    private Coroutine hangingoutCorountine;
    private float workingDuration = 10f; 
    
    
    private IEnumerator WorkingCountdown(MonoBehaviour runner)
    {
        yield return new WaitForSeconds(workingDuration); // 等待设定的生长时间
        // Debug.Log("Execute Working Event...");

        characterWorkingEvent?.Invoke(); // 触发成长完成事件

        if (workingCoroutine != null)
        {
            runner.StopCoroutine(workingCoroutine);
            //return false;
        }
        workingCoroutine = runner.StartCoroutine(WorkingCountdown(runner));
    }

    private IEnumerator HangingAroundCountdown()
    {
        // yield return new WaitForSeconds(Random.Range(0, hangOutWaitTime));
        yield return new WaitForSeconds(2);
        
        IsPendingTowardsTarget = false;
        characterMovement.SelectRandomTargetPosition();
        characterMovement.StartHangingOut();

        

    }
    
    public void setL2dCharacterActive(bool active)
    {
        character.l2dCharacter.SetActive(active);
    }

    public String UpdateStatusUIText()
    {
        String text = "Energy: " + character.CharacterStats.energy.GetCurrentEnergy() + " / "
                      + character.CharacterStats.energy.GetMaxEnergy() 
                      + "\nSaturation: " + character.CharacterStats.saturation.GetCurrentSaturation() 
                      + " / " + character.CharacterStats.saturation.GetMaxSaturation() + "\nStatus: " 
                      + currentState + (IsPendingTowardsTarget && currentState != HomeState.HangingAround ? "(Pending)" : "") ;
        
        return text;
    }
    
}
