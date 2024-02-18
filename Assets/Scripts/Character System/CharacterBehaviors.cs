using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterBehaviors : MonoBehaviour
{
    public enum HomeState { 
        Resting, 
        Gatherable, 
        Gathering
        
    }

    private Character character;

    private HomeState currentState;
    

    private CharacterMovement characterMovement;

    private float moveSpeed = 1f;
    public BoxCollider hangOutArea;
    private float hangOutWaitTime = 2f; // 停顿时间


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
    
    void Update()
    {
        //Debug.Log(characterStats.energy.GetCurrentEnergy() + "/" + characterStats.energy.GetMaxEnergy() + " (" + characterStats.energy.RemainEnergyPercentage() + ")");
        if (state == CharacterState.Gather)
        {
            if(character.CharacterStats.energy.NoEnergy())
            {
                character.EndGatherUI();
                character.GatheringSpot.EndGathering();
                //state = CharacterState.Idle;
                //currentEnergy = maxEnergy;
                //myCI.ResetHome();   
            }
            if(gatherTimeLeft <= 0)
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
        else if(state == CharacterState.Idle)
        {
            if (!character.CharacterStats.energy.EnergyLessThanRestingPercentage())
            {
                character.CharacterIcon.ChangeIconColorToHome();

                EnterState(HomeState.Gatherable);
            } else
            {
                character.CharacterIcon.ChangeIconColorToGather();
                EnterState(HomeState.Resting);
            }
            
            /* Energy is always displayed
             
            if (!characterStats.energy.maximizeEnergy())
            {
                characterIcon.setga(CircularUI.CircularUIState.Display);
            } else
            {
                characterIcon.SetCircularUIState(CircularUI.CircularUIState.NonDisplay);
            }*/

            if (restTimeLeft <= 0)
            {
                character.CharacterIcon.SetGatheringProgress(0, 100 * character.CharacterStats.energy.RemainEnergyPercentage(), false);
                character.CharacterStats.energy.AddEnergy();
                restTimeLeft = character.CharacterStats.restingSpeed.GetRestingSpeed();
            }
            //update ui
            restTimeLeft -= character.CharacterStats.restingSpeed.GetRestingSpeed() * Time.deltaTime;

            //characterIcon.SetGatheringProgress(100 * (1 - (gatherTimeLeft / gatheringSpot.gatherTime)), 100 * characterStats.energy.RemainEnergyPercentage(), true);
        }
    }
    public void EnterState(HomeState state)
    {
        if (characterMovement == null) return;

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
