using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Cinemachine;
using UnityEngine;

public class UniQuest : MonoBehaviour
{
    [Header("---DO NOT EDIT---")]
    public bool hasStarted = false;
    public bool hasCompleted = false;
    [Tooltip("Index starts with 1.")]public int currentUniAction = 0;
    [Tooltip("Index starts with 1.")]public int totalUniActions = 0;

    [Header("---Edit Conditions---")]
    public bool uponGameStart = false;

    [Header("---Edit UniAction Sequence---")]
    [SerializeField]public UniActionSequence uniActionSequence;
    
    private void Start()
    {
        if(uponGameStart) QueQuest(); //uniActionSequence.PerformAction();
    }

    public void QueQuest()
    {
        totalUniActions = uniActionSequence.allUniActions.Count;
        hasStarted = true;
        UniQuestManager.i.QueUniQuest(this);
    }
}
