using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uniland.Characters;
using UnityEngine.Events;
using static CharacterBehaviors;

namespace Uniland.Characters
{
    public class Energy
    {
        private int currentEnergy;
        private int maxEnergy;

        private float restingEnergyPercentage;

        public UnityEvent<float> RemainEnergyPercentageBroadcast;

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
            RemainEnergyPercentageBroadcast = new UnityEvent<float>();
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
            RemainEnergyPercentageBroadcast.Invoke(RemainEnergyPercentage());
        }

        public bool CostEnergy()
        {
            if(NoEnergy()) return false;
            currentEnergy -= 1;
            RemainEnergyPercentageBroadcast.Invoke(RemainEnergyPercentage());
            return true;
        }

        public bool AddEnergy()
        {
            if (maximizeEnergy()) return false;
            currentEnergy += 1;
            RemainEnergyPercentageBroadcast.Invoke(RemainEnergyPercentage());
            return true;
        }

        public bool SetEnergy(int energy)
        {
            if (energy > maxEnergy) return false;
            currentEnergy = energy;
            RemainEnergyPercentageBroadcast.Invoke(RemainEnergyPercentage());
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
        
        public bool EnergyLessThanPercentage(float percentage)
        {
            return currentEnergy < maxEnergy * percentage;
        }

        public float RemainEnergyPercentage()
        {
            return ((float)currentEnergy) / ((float)maxEnergy);
        }
    }
    
    
    public class Saturation
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
        //
        // public Saturation(int currentSaturation, int maxSaturation)
        // {
        //     this.currentSaturation = currentSaturation;
        //     this.maxSaturation = maxSaturation;
        // }

        
        public bool SetSaturation(int saturation)
        {
            if (saturation > maxSaturation) return false;
            currentSaturation = saturation;
            return true;
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

        public bool SaturationLessThanFullPercentage()
        {
            return currentSaturation < maxSaturation * restingSaturationPercentage;
        }

        public float RemainSaturationPercentage()
        {
            return ((float)currentSaturation) / ((float)maxSaturation);
        }
    }

    public class GatherSpeed
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

    public class ExploreSpeed
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

    public class RestingSpeed
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

    public class HerdSize
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

    public class CharacterStats
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

    public CharacterStats CharacterStats
    {
        get => characterStats;
        set => characterStats = value;
    }

    public CharacterBasicStats InitialStats
    {
        get => initialStats;
        set => initialStats = value;
    }

    // [SerializeField]
    private CharacterBehaviors _behaviors; 
    
    [SerializeField]
    public GameObject l2dCharacter;


    // [SerializeField]
    public CharacterInteraction charInteractions;
    

    GatherSpot gatheringSpot;

    public GatherSpot GatheringSpot1
    {
        get => gatheringSpot;
        set => gatheringSpot = value;
    }

    public CharacterIcon CharacterIcon
    {
        get => characterIcon;
        set => characterIcon = value;
    }

    public GatherSpot GatheringSpot
    {
        get => gatheringSpot;
        set => gatheringSpot = value;
    }

    CharacterIcon characterIcon;
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
        // characterStats = new CharacterStats(initialStats);
        _behaviors = (GetComponent<CharacterBehaviors>()==null)?gameObject.AddComponent<CharacterBehaviors>():GetComponent<CharacterBehaviors>();
        //
        // l2dCharacter = Instantiate(GetL2dGameObject(), transform);
        // Debug.Log(l2dCharacter + "generated");
        // _behaviors.L2dCharacter = l2dCharacter;
        charInteractions = l2dCharacter.GetComponent<CharacterInteraction>();
        
        charInteractions.Initialize(initialStats);

        charExperience = 0;

        characterStats.energy.SetEnergy(0);
        characterStats.saturation.SetSaturation(0);
    }


    public void SetUp(CharacterIcon ci)
    {
        characterIcon = ci;
    }

    public void Initialize(CharacterBasicStats initialStats)
    {
        this.initialStats = initialStats;
    }

    public void StartGatherUI(GatherSpot es, CharacterIcon ci)
    {

        if (!characterStats.energy.EnergyLessThanRestingPercentage())
        {
            gatheringSpot = es;
            _behaviors.GatherTimeLeft = es.gatherTime;

            _behaviors.state = CharacterState.Gather;
            myCI = ci;

            characterIcon.SetGatheringProgress(100 * (1 - (_behaviors.GatherTimeLeft / gatheringSpot.gatherTime)), 100 * characterStats.energy.RemainEnergyPercentage(), false);
            gatheringSpot.SetGatheringProgress(100 * (1 - (_behaviors.GatherTimeLeft / gatheringSpot.gatherTime)), 100 * characterStats.energy.RemainEnergyPercentage(), false);
            SetCircularUIState(CircularUI.CircularUIState.Display);

            CharacterGatherUnityEvent.Invoke(gatheringSpot.transform.parent.GetComponentInParent<BLDExploreSpot>().GetSetUpInfo(),initialStats,1);
            _behaviors.EnterState(HomeState.Exploring);
        }


    }

    public void EndGatherUI()
    {
        SetCircularUIState(CircularUI.CircularUIState.NonDisplay);

        gatheringSpot.EndGathering();
        //characterStats.energy.RestoreAllEnergy();

        myCI.ResetHome();
        
        CharacterGatherUnityEvent.Invoke(gatheringSpot.transform.parent.GetComponentInParent<BLDExploreSpot>().GetSetUpInfo(),initialStats,0);


        // if (characterStats.energy.EnergyLessThanRestingPercentage())
        // {
        //     _behaviors.EnterState(HomeState.Resting);
        // } else
        // {
        //     _behaviors.EnterState(HomeState.Gatherable);
        // }
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

    
    
    
}
