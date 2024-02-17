using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uniland.Characters;
using UnityEngine.Events;
using static CharacterBehaviors;

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
    
    
    class Saturation
    {
        private int currentSaturation;
        private int maxSaturation;

        private float restingSaturationPercentage;

        //public Energy(int maxEnergy)
        //{
        //    this.maxEnergy = maxEnergy;
        //    currentEnergy = this.maxEnergy;
        //    restingEnergyPercentage = 0.4f;
        //}

        public Saturation(int maxSaturation, float restingSaturationPercentage)
        {
            this.maxSaturation = maxSaturation;
            currentSaturation = this.maxSaturation;
            this.restingSaturationPercentage = restingSaturationPercentage;
        }

        public Saturation(int currentSaturation, int maxSaturation)
        {
            this.currentSaturation = currentSaturation;
            this.maxSaturation = maxSaturation;
        }

        public int GetMaxSaturation()
        {
            return maxSaturation;
        }

        public int GetCurrentSaturation()
        {
            return currentSaturation;
        }

        public void RestoreAllSaturation()
        {
            currentSaturation = maxSaturation;
        }

        public bool CostSaturation()
        {
            if(NoSaturation()) return false;
            currentSaturation -= 1;
            return true;
        }

        public bool AddSaturation()
        {
            if (maximizeSaturation()) return false;
            currentSaturation += 1;
            return true;
        }

        public bool NoSaturation()
        {
            return currentSaturation <= 0;
        }

        public bool maximizeSaturation()
        {
            return currentSaturation == maxSaturation;
        }

        public bool SaturationLessThanRestingPercentage()
        {
            return currentSaturation <= maxSaturation * restingSaturationPercentage;
        }

        public float RemainSaturationPercentage()
        {
            return ((float)currentSaturation) / ((float)maxSaturation);
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
        public Saturation saturation;
        public GatherSpeed gatherSpeed;
        public ExploreSpeed exploreSpeed;
        public RestingSpeed restingSpeed;
        public HerdSize herdSize;
        public CharacterStats(CharacterBasicStats basicStats)
        {
            energy = new Energy(basicStats.energy, basicStats.restingEnergyPercentage);
            saturation = new Saturation(basicStats.saturation, basicStats.restingSaturationPercentage);
            
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

    public CharacterBasicStats InitialStats
    {
        get => initialStats;
        set => initialStats = value;
    }

    // [SerializeField]
    private CharacterBehaviors _behaviors; 
    
    [SerializeField]
    private GameObject l2dCharacter;
    
    // [SerializeField]
    public CharacterInteraction charInteractions;
    
    enum CharacterState {Idle, Gather}
    CharacterState state = CharacterState.Idle;

    GatherSpot gatheringSpot;
    CharacterIcon characterIcon;
    float gatherTimeLeft;
    float restTimeLeft;
    CharacterIcon myCI;

    [SerializeField] private int charExperience;
    
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
        _behaviors = (GetComponent<CharacterBehaviors>()==null)?gameObject.AddComponent<CharacterBehaviors>():GetComponent<CharacterBehaviors>();
        
        l2dCharacter = Instantiate(GetL2dGameObject(), transform);
        _behaviors.L2dCharacter = l2dCharacter;
        charInteractions = l2dCharacter.GetComponent<CharacterInteraction>();
        
        charInteractions.Initialize(initialStats);

        charExperience = 0;
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

                _behaviors.EnterState(HomeState.Gatherable);
            } else
            {
                characterIcon.ChangeIconColorToGather();
                _behaviors.EnterState(HomeState.Resting);
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
            _behaviors.EnterState(HomeState.Gathering);
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
            _behaviors.EnterState(HomeState.Resting);
        } else
        {
            _behaviors.EnterState(HomeState.Gatherable);
        }
    }

    void SetCircularUIState(CircularUI.CircularUIState circularUIState)
    {
        gatheringSpot.SetCircularUIState(circularUIState);
        gatheringSpot.SetCircularUIState(circularUIState);

        /*
        characterIcon.SetCircularUIState(circularUIState);
        characterIcon.SetCircularUIState(circularUIState);*/
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


    public CharacterBehaviors GetHomeStatus()
    {
        return _behaviors;
    }

    public bool EnergyLessThanRestingPercentage()
    {
        return characterStats.energy.EnergyLessThanRestingPercentage();
    }
    
    
}
