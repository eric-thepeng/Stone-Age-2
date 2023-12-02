using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterHomeStatus : MonoBehaviour
{
    public enum HomeState { Resting, Gatherable, Gathering }

    private Character character;

    public HomeState currentState;
    public GameObject l2dCharacter;
    public CharacterMovement characterMovement;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Character>();

        l2dCharacter = character.GetL2dGameObject();
        if (l2dCharacter == null) Debug.LogError("Character " + character.GetCharacterName() + "'s L2dCharacter is missing!");

        characterMovement = l2dCharacter.GetComponent<CharacterMovement>();

        if (character.EnergyLessThanRestingPercentage())
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

    public void EnterState(HomeState state)
    {
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
        l2dCharacter.SetActive(active);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
