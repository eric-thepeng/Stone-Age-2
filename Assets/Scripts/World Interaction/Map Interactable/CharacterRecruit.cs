using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRecruit : MonoBehaviour
{
    [SerializeField] private CharacterBasicStats characterToRecruit;
    [SerializeField] private int recruitAtLevelState = 2;

    private void Awake()
    {
        GetComponent<LevelUp>().GetCurrentStatePlayerStat().SubscribeStatChange(CheckToRecruit);
    }

    private void CheckToRecruit(int level)
    {
        if (level == recruitAtLevelState)
        {
            CharacterManager.i.AddCharacter(characterToRecruit);
        }
    }
}
