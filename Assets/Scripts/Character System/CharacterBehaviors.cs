using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hypertonic.GridPlacement;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterBehaviors : MonoBehaviour
{
    public enum HomeState { 
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

        characterMovement.CharacterBehavior = this;
        //Debug.Log(characterMovement.transform.name);

            characterMovement.moveSpeed = moveSpeed;
            characterMovement.hangOutWaitTime = hangOutWaitTime;

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
    float periodTimeLeft; // period for execute
    
    void Update()
    {
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
                // character.CharacterIcon.SetGatheringProgress(0, 100 * character.CharacterStats.energy.RemainEnergyPercentage(), false);
                // character.CharacterStats.energy.AddEnergy();
                
                periodTimeLeft = 1;
                // periodTimeLeft = character.CharacterStats.restingSpeed.GetRestingSpeed();
                
                CheckState();
                
            }
            //update ui
            periodTimeLeft -= 1 * Time.deltaTime;


        }
    }
    

    private void CheckState()
    {
        Debug.Log("Check State: Energy - " + character.CharacterStats.energy.GetCurrentEnergy() + ", Saturation - " + character.CharacterStats.saturation.GetCurrentSaturation());
            PlaceableObject[] _nearbyObjects = FindAndSortComponents<PlaceableObject>(transform.position, 30);
            
            // sleeping <25%
            
            if (character.CharacterStats.energy.EnergyLessThanRestingPercentage()) 
            {
                if (currentState == HomeState.Sleeping1) return;
                
                PlaceableObject[] _bedObjects = _nearbyObjects.Where(obj => obj.GetBuildingISO().containTag("bed")).ToArray();
                // _behaviorTargetPosition = _bedObject.transform.position;

                PlaceableObject _bedObject = null;
                int count = 0;
                
                while (count < _bedObjects.Length)
                {
                    if (characterMovement.SetTargetPosition(_bedObjects[count].transform.position))
                    {
                        _bedObject = _bedObjects[count];
                        break;
                    }
                    else
                    {
                        count++;
                    }
                }
                
                Debug.Log("Find " + _bedObjects.Length + " beds , select " + count + " " + _bedObject + " to go");
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
                    if (characterMovement.SetTargetPosition(_foodObjects[count].transform.position))
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
                    if (characterMovement.SetTargetPosition(_bedObjects[count].transform.position))
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
            
            // find interacting object
            if (currentState == HomeState.Interacting) return;
            
            PlaceableObject[] _interactingObjects = _nearbyObjects.Where(obj => obj.GetBuildingISO().containTag("interactable")).ToArray();
            
            PlaceableObject _interactingObject = null;
            int interactableCount = 0;
            while (interactableCount < _interactingObjects.Length)
            {
                if (characterMovement.SetTargetPosition(_interactingObjects[interactableCount].transform.position))
                {
                    _interactingObject = _interactingObjects[interactableCount];
                    break;
                }
                else
                {
                    interactableCount++;
                }
            }
                
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
        
        Debug.Log("Enter state " + state + ", target " + targetObject);
        
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
            
        if (state == HomeState.HangingAround)
        {
            IsPendingTowardsTarget = false;
            characterMovement.SelectRandomTargetPosition();
            // start character moving
            // when character arrive destination, start sleeping
            
            // character.CharacterIcon.ChangeIconColorToHome();
        }
        else
        {
            IsPendingTowardsTarget = true;
            characterMovement.SetTargetPosition(targetObject.transform.position);

        }
        
    }

    public Action characterWorkingEvent; // coroutine
    private Coroutine workingCoroutine;
    private float workingDuration = 10f; 
    
    public void StartCyclicallyWorking(MonoBehaviour runner)
    {
        // MonoBehaviour runner = this;
        
        if (workingCoroutine != null)
        {
            // Debug.Log("Still in counting!");
            runner.StopCoroutine(workingCoroutine);
            //return false;
        }
        workingCoroutine = runner.StartCoroutine(WorkingCountdown(runner));
        // return true;
    }

    public void StopCyclicallyWorking(MonoBehaviour runner)
    {
        if (workingCoroutine != null)
        {
            // Debug.Log("Still in counting!");
            runner.StopCoroutine(workingCoroutine);
            //return false;
        }
    }
    
    
    private IEnumerator WorkingCountdown(MonoBehaviour runner)
    {
        yield return new WaitForSeconds(workingDuration); // 等待设定的生长时间

        characterWorkingEvent?.Invoke(); // 触发成长完成事件

        StartCyclicallyWorking(this);
    }

    public void setL2dCharacterActive(bool active)
    {
        character.l2dCharacter.SetActive(active);
    }

    
}
