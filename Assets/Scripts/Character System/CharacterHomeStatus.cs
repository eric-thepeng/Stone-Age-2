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

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Character>();

        l2dCharacter = GameObject.Find(character.GetCharacterName());

        if (character.EnergyLessThanRestingPercentage())
        {
            currentState = HomeState.Resting;
        } else
        {
            currentState = HomeState.Gatherable;
        }
    }

    public void EnterState(HomeState state)
    {
        if (currentState == HomeState.Resting)
        {
            if (state == HomeState.Gatherable)
            {
                currentState = HomeState.Gatherable;

            }
            else if (state == HomeState.Gathering)
            {
                currentState = HomeState.Gathering;
                setL2dCharacterActive(false);
            }
        }
        else if (currentState == HomeState.Gatherable)
        {
            if (state == HomeState.Gathering)
            {
                currentState = HomeState.Gathering;
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

            }
            else if (state == HomeState.Gatherable)
            {
                currentState = HomeState.Gatherable;
                setL2dCharacterActive(true);
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
