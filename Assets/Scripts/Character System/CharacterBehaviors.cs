using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    

    private CharacterMovement characterMovement;

    private float moveSpeed = 1f;
    public BoxCollider hangOutArea;
    private float hangOutWaitTime = 2f; // 停顿时间

    public enum CharacterState {Idle, Gather}
    public CharacterState state = CharacterState.Idle;


    float gatherTimeLeft;

    public float GatherTimeLeft
    {
        get => gatherTimeLeft;
        set => gatherTimeLeft = value;
    }

    public float RestTimeLeft
    {
        get => restTimeLeft;
        set => restTimeLeft = value;
    }

    float restTimeLeft;


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

        if (character.CharacterStats.energy.EnergyLessThanRestingPercentage())
        {
            currentState = HomeState.Resting;
            characterMovement.StartSleeping();
        } else
        {
            currentState = HomeState.Gatherable;
            characterMovement.StopSleeping();
            characterMovement.StartHangingOut();

        }
    }

    private Vector3 _behaviorTargetPosition;
    
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
        else if(state == CharacterState.Idle) // if character is at home
        {
            
            if (restTimeLeft <= 0)
            {
                character.CharacterIcon.SetGatheringProgress(0, 100 * character.CharacterStats.energy.RemainEnergyPercentage(), false);
                character.CharacterStats.energy.AddEnergy();
                restTimeLeft = character.CharacterStats.restingSpeed.GetRestingSpeed();
            }
            //update ui
            restTimeLeft -= character.CharacterStats.restingSpeed.GetRestingSpeed() * Time.deltaTime;
            
            if (character.CharacterStats.energy.EnergyLessThanRestingPercentage())
            {
                PlaceableObject[] _bedObjects = FindAndSortComponents<PlaceableObject>(transform.position, 30);
                // _behaviorTargetPosition = _bedObject.transform.position;

                PlaceableObject _bedObject = null;
                int count = 0;
                while (count < _bedObjects.Length)
                {
                    if (characterMovement.SetTargetPosition(_bedObjects[count].transform.position))
                    {
                        _bedObject = _bedObjects[count];
                        _behaviorTargetPosition = _bedObject.transform.position;
                        break;
                    }
                    else
                    {
                        count++;
                    }
                }
                
                if (currentState != HomeState.Sleeping1 && _bedObject != null) // if character find bed to go
                {
                    
                    EnterState(HomeState.Sleeping1);
                    return;
                }
                
            }
            if (character.CharacterStats.saturation.SaturationLessThanFullPercentage())
            {
                // character.CharacterIcon.ChangeIconColorToGather();
                
                // if () {}// if character find food to eat
                    EnterState(HomeState.Sleeping1);
                return;
            }
            


        }
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
    
    public void EnterState(HomeState state)
    {
        if (characterMovement == null) return;

        if (state == HomeState.Sleeping1)
        {
            character.CharacterIcon.ChangeIconColorToHome();
        }
        

        if (currentState == HomeState.Resting)
        {
            if (state == HomeState.Gatherable)
            {
                currentState = HomeState.Gatherable;
                characterMovement.StopSleeping();
                characterMovement.StartHangingOut();


            }
            else if (state == HomeState.Gathering)
            {
                currentState = HomeState.Gathering;

                characterMovement.StopSleeping();
                //characterMovement.StopHangingOut();
                setL2dCharacterActive(false);
            }
        }
        else if (currentState == HomeState.Gatherable)
        {
            if (state == HomeState.Gathering)
            {
                currentState = HomeState.Gathering;

                characterMovement.StopHangingOut();
                setL2dCharacterActive(false);

            }
            //else if (state == HomeState.Gatherable)
            //{

            //}
            //else if (state == HomeState.Resting)
            //{

            //}
        }
        else if (currentState == HomeState.Gathering)
        {
            if (state == HomeState.Resting)
            {
                currentState = HomeState.Resting;
                setL2dCharacterActive(true);

                characterMovement.StartSleeping();
                characterMovement.StopHangingOut();

            }
            else if (state == HomeState.Gatherable)
            {
                currentState = HomeState.Gatherable;
                setL2dCharacterActive(true);

                characterMovement.StartHangingOut();
            }
        }

    }

    public HomeState getCurrentHomeState()
    {
        return currentState;
    }

    public void setL2dCharacterActive(bool active)
    {
        character.l2dCharacter.SetActive(active);
    }

    
}
