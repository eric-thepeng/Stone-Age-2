using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    int gatherSpeed = 3;
    int maxEnergy = 3;
    int currentEnergy;

    enum CharacterState {Idle, Gather}
    CharacterState state = CharacterState.Idle;

    ExploreSpot gatheringSpot;
    float gatherTimeLeft;
    CharacterIcon myCI;

    void Start()
    {
        currentEnergy = maxEnergy;
    }

    void Update()
    {
        if(state == CharacterState.Gather)
        {
            if(currentEnergy == 0)
            {
                gatheringSpot.EndGathering();
                state = CharacterState.Idle;
                currentEnergy = maxEnergy;
                myCI.ResetHome();
            }
            if(gatherTimeLeft <= 0)
            {
                YieldResource();
                currentEnergy--;
                gatherTimeLeft = gatheringSpot.gatherTime;
            }
            // TODO: update ui
            gatherTimeLeft -= gatherSpeed * Time.deltaTime;
        }
    }

    public void StartGather(ExploreSpot es, CharacterIcon ci)
    {
        gatheringSpot = es;
        gatherTimeLeft = es.gatherTime;

        state = CharacterState.Gather;
        myCI = ci;
    }

    public void YieldResource()
    {
        //print("Obtain " + gatheringSpot.spiritPoint + " Spirit Point.");
        SpiritPoint.i.Add(gatheringSpot.spiritPoint);
        int roll = Random.Range(0, gatheringSpot.totalWeight);
        //print("dice roll: " + roll);
        int weightCount = 0;
        for (int i = 0; i < gatheringSpot.weight.Length; i++)
        {
            weightCount += gatheringSpot.weight[i];
            if (roll < weightCount)
            {
                print("Obtain 1 " + gatheringSpot.resource[i]);
                return;
            }
        }
        print("Error when gathering resource");
    }
}
