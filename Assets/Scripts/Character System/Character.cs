using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uniland.Characters;
using UnityEngine.Events;
using static CharacterHomeStatus;

namespace Uniland.Characters
{
    class Energy
    {
        private int currentEnergy;
        private int maxEnergy;

        private float restingEnergyPercentage;

        //public Energy(int maxEnergy)
        //{
        //    this.maxEnergy = maxEnergy;
        //    currentEnergy = this.maxEnergy;
        //    restingEnergyPercentage = 0.4f;
        //}

        public Energy(int maxEnergy, float restingEnergyPercentage)
        {
            this.maxEnergy = maxEnergy;
            currentEnergy = this.maxEnergy;
            this.restingEnergyPercentage = restingEnergyPercentage;
        }

        public Energy(int currentEnergy, int maxEnergy)
        {
            this.currentEnergy = currentEnergy;
            this.maxEnergy = maxEnergy;
        }

        public int GetMaxEnergy()
        {
            return maxEnergy;
        }

        public int GetCurrentEnergy()
        {
            return currentEnergy;
        }

        public void RestoreAllEnergy()
        {
            currentEnergy = maxEnergy;
        }

        public bool CostEnergy()
        {
            if(NoEnergy()) return false;
            currentEnergy -= 1;
            return true;
        }

        public bool AddEnergy()
        {
            if (maximizeEnergy()) return false;
            currentEnergy += 1;
            return true;
        }

        public bool NoEnergy()
        {
            return currentEnergy <= 0;
        }

        public bool maximizeEnergy()
        {
            return currentEnergy == maxEnergy;
        }

        public bool EnergyLessThanRestingPercentage()
        {
            return currentEnergy <= maxEnergy * restingEnergyPercentage;
        }

        public float RemainEnergyPercentage()
        {
            return ((float)currentEnergy) / ((float)maxEnergy);
        }
    }

    class GatherSpeed
    {
        private float currentGatherSpeed;

        public GatherSpeed(float currentGatherSpeed)
        {
            this.currentGatherSpeed = currentGatherSpeed;
        }

        public float GetCurrentGatherSpeed()
        {
            return currentGatherSpeed;
        }

    }

    class ExploreSpeed
    {
        private float exploreSpeed;

        public ExploreSpeed(float exploreSpeed)
        {
            this.exploreSpeed = exploreSpeed;
        }

        public float GetExploreSpeed()
        {
            return exploreSpeed;
        }
    }

    class RestingSpeed
    {
        private float restingSpeed;

        public RestingSpeed(float restingSpeed)
        {
            this.restingSpeed = restingSpeed;
        }

        public float GetRestingSpeed()
        {
            return restingSpeed;
        }
    }

    class HerdSize
    {
        private int herdSize;

        public HerdSize(int herdSize)
        {
            this.herdSize = this.herdSize;
        }

        public int GetHerdSize()
        {
            return herdSize;
        }

        public void AddHerdSize(int addAmount)
        {
            herdSize += addAmount;
        }

    }

    class CharacterStats
    {
        public Energy energy;
        public GatherSpeed gatherSpeed;
        public ExploreSpeed exploreSpeed;
        public RestingSpeed restingSpeed;
        public HerdSize herdSize;
        public CharacterStats(CharacterBasicStats basicStats)
        {
            energy = new Energy(basicStats.energy, basicStats.restingEnergyPercentage);
            gatherSpeed = new GatherSpeed(basicStats.gatherSpeed);
            exploreSpeed = new ExploreSpeed(basicStats.exploreSpeed);
            restingSpeed = new RestingSpeed(basicStats.restingSpeed);
            herdSize = new HerdSize(basicStats.herdSize);
        }
    }
}

public class Character : MonoBehaviour
{
    [SerializeField] private CharacterBasicStats initialStats;
    private CharacterStats characterStats;

    private CharacterHomeStatus homeStatus; 
    
    enum CharacterState {Idle, Gather}
    CharacterState state = CharacterState.Idle;

    GatherSpot gatheringSpot;
    CharacterIcon characterIcon;
    float gatherTimeLeft;
    float restTimeLeft;
    CharacterIcon myCI;
    
    // Unity Event

    /// <summary>
    /// Integer Meaning: 0: Start Gather 1: End Gather
    /// </summary>
    public static UnityEvent<SO_ExploreSpotSetUpInfo, CharacterBasicStats, int> CharacterGatherUnityEvent = null;

    void Start()
    {
        if (CharacterGatherUnityEvent == null)
            CharacterGatherUnityEvent = new UnityEvent<SO_ExploreSpotSetUpInfo, CharacterBasicStats, int>();
        characterStats = new CharacterStats(initialStats);
        homeStatus = (GetComponent<CharacterHomeStatus>()==null)?gameObject.AddComponent<CharacterHomeStatus>():GetComponent<CharacterHomeStatus>();
    }

    void Update()
    {
        //Debug.Log(characterStats.energy.GetCurrentEnergy() + "/" + characterStats.energy.GetMaxEnergy() + " (" + characterStats.energy.RemainEnergyPercentage() + ")");
        if (state == CharacterState.Gather)
        {
            if(characterStats.energy.NoEnergy())
            {
                EndGather();
                gatheringSpot.EndGathering();
                //state = CharacterState.Idle;
                //currentEnergy = maxEnergy;
                //myCI.ResetHome();   
            }
            if(gatherTimeLeft <= 0)
            {
                YieldResource();
                DiscoverSpot();
                characterStats.energy.CostEnergy();
                gatherTimeLeft = gatheringSpot.gatherTime;
            }
            //update ui
            gatherTimeLeft -= characterStats.gatherSpeed.GetCurrentGatherSpeed() * Time.deltaTime;

            characterIcon.SetGatheringProgress(100 * (1 - (gatherTimeLeft / gatheringSpot.gatherTime)), 100 * characterStats.energy.RemainEnergyPercentage(), true);
            gatheringSpot.SetGatheringProgress(100 * (1 - (gatherTimeLeft / gatheringSpot.gatherTime)), 100 * characterStats.energy.RemainEnergyPercentage(), true);
        }
        else if(state == CharacterState.Idle)
        {
            if (!EnergyLessThanRestingPercentage())
            {
                characterIcon.ChangeIconColorToHome();

                homeStatus.EnterState(HomeState.Gatherable);
            } else
            {
                characterIcon.ChangeIconColorToGather();
                homeStatus.EnterState(HomeState.Resting);
            }

            if (!characterStats.energy.maximizeEnergy())
            {
                characterIcon.SetCircularUIState(CircularUI.CircularUIState.Display);
            } else
            {
                characterIcon.SetCircularUIState(CircularUI.CircularUIState.NonDisplay);
            }

            if (restTimeLeft <= 0)
            {
                characterIcon.SetGatheringProgress(0, 100 * characterStats.energy.RemainEnergyPercentage(), false);
                characterStats.energy.AddEnergy();
                restTimeLeft = characterStats.restingSpeed.GetRestingSpeed();
            }
            //update ui
            restTimeLeft -= characterStats.restingSpeed.GetRestingSpeed() * Time.deltaTime;

            //characterIcon.SetGatheringProgress(100 * (1 - (gatherTimeLeft / gatheringSpot.gatherTime)), 100 * characterStats.energy.RemainEnergyPercentage(), true);
        }
    }

    public void SetUp(CharacterIcon ci)
    {
        characterIcon = ci;
    }

    public void Initialize(CharacterBasicStats initialStats)
    {
        this.initialStats = initialStats;
    }

    public void StartGather(GatherSpot es, CharacterIcon ci)
    {

        if (!characterStats.energy.EnergyLessThanRestingPercentage())
        {
            gatheringSpot = es;
            gatherTimeLeft = es.gatherTime;

            state = CharacterState.Gather;
            myCI = ci;

            characterIcon.SetGatheringProgress(100 * (1 - (gatherTimeLeft / gatheringSpot.gatherTime)), 100 * characterStats.energy.RemainEnergyPercentage(), false);
            gatheringSpot.SetGatheringProgress(100 * (1 - (gatherTimeLeft / gatheringSpot.gatherTime)), 100 * characterStats.energy.RemainEnergyPercentage(), false);
            SetCircularUIState(CircularUI.CircularUIState.Display);

            CharacterGatherUnityEvent.Invoke(gatheringSpot.transform.parent.GetComponentInParent<BLDExploreSpot>().GetSetUpInfo(),initialStats,1);
            homeStatus.EnterState(HomeState.Gathering);
        }


    }

    public void EndGather()
    {
        SetCircularUIState(CircularUI.CircularUIState.NonDisplay);

        gatheringSpot.EndGathering();
        state = CharacterState.Idle;
        //characterStats.energy.RestoreAllEnergy();

        myCI.ResetHome();
        
        CharacterGatherUnityEvent.Invoke(gatheringSpot.transform.parent.GetComponentInParent<BLDExploreSpot>().GetSetUpInfo(),initialStats,0);


        if (characterStats.energy.EnergyLessThanRestingPercentage())
        {
            homeStatus.EnterState(HomeState.Resting);
        } else
        {
            homeStatus.EnterState(HomeState.Gatherable);
        }
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

    public void DiscoverSpot()
    {
        if (gatheringSpot is DiscoveryGatherSpot)
        {
            ((DiscoveryGatherSpot)gatheringSpot).Discover(characterStats.exploreSpeed.GetExploreSpeed());
        }
    }

    public string GetCharacterName()
    {
        return initialStats.name;
    }

    public GameObject GetL2dGameObject()
    {
        return initialStats.l2dGameObject;
    }

    public int GetMoveSpeed()
    {
        return initialStats.moveSpeed;
    }

    public float GetHangOutWaitTime()
    {
        return initialStats.hangOutWaitTime;
    }


    public CharacterHomeStatus GetHomeStatus()
    {
        return homeStatus;
    }

    public bool EnergyLessThanRestingPercentage()
    {
        return characterStats.energy.EnergyLessThanRestingPercentage();
    }
}
