using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    int gatherSpeed = 1;
    int maxEnergy = 10;
    int currentEnergy;

    enum CharacterState {Idle, Gather}
    CharacterState state = CharacterState.Idle;

    GatherSpot gatheringSpot;
    CharacterIcon characterIcon;
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
                EndGather();
                //gatheringSpot.EndGathering();
                //state = CharacterState.Idle;
                //currentEnergy = maxEnergy;
                //myCI.ResetHome();   
            }
            if(gatherTimeLeft <= 0)
            {
                YieldResource();
                currentEnergy--;
                gatherTimeLeft = gatheringSpot.gatherTime;
            }
            //update ui
            gatherTimeLeft -= gatherSpeed * Time.deltaTime;

            characterIcon.SetGatheringProgress(100 * (1 - (gatherTimeLeft / gatheringSpot.gatherTime)), 100 * ((float)currentEnergy / (float)maxEnergy), true);
            gatheringSpot.SetGatheringProgress(100 * (1 - (gatherTimeLeft / gatheringSpot.gatherTime)), 100 * ((float)currentEnergy / (float)maxEnergy), true);
        }
    }

    public void SetUp(CharacterIcon ci)
    {
        characterIcon = ci;
    }

    public void StartGather(GatherSpot es, CharacterIcon ci)
    {
        gatheringSpot = es;
        gatherTimeLeft = es.gatherTime;

        state = CharacterState.Gather;
        myCI = ci;

        characterIcon.SetGatheringProgress(100 * (1 - (gatherTimeLeft / gatheringSpot.gatherTime)), 100 * ((float)currentEnergy / (float)maxEnergy), false);
        gatheringSpot.SetGatheringProgress(100 * (1 - (gatherTimeLeft / gatheringSpot.gatherTime)), 100 * ((float)currentEnergy / (float)maxEnergy), false);
        SetCircularUIState(CircularUI.CircularUIState.Display);
    }

    public void EndGather()
    {
        SetCircularUIState(CircularUI.CircularUIState.NonDisplay);

        gatheringSpot.EndGathering();
        state = CharacterState.Idle;
        currentEnergy = maxEnergy;
        myCI.ResetHome();
    }

    void SetCircularUIState(CircularUI.CircularUIState circularUIState)
    {
        gatheringSpot.SetCircularUIState(circularUIState);
        gatheringSpot.SetCircularUIState(circularUIState);

        characterIcon.SetCircularUIState(circularUIState);
        characterIcon.SetCircularUIState(circularUIState);
    }

    public void YieldResource()
    {
        gatheringSpot.gatherResource.GainResource();
    }
}
