using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpiritPoint : MonoBehaviour
{
    static SpiritPoint instance;
    public int startingAmount;
    [SerializeField]TextMeshPro displayText;
    private PlayerStat spiritPointAmount;

    public static SpiritPoint i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SpiritPoint>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        spiritPointAmount = new PlayerStat(startingAmount);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void Add(int addAmount)
    {
        spiritPointAmount.ChangeAmount(addAmount);
        UpdateUI();
    }

    public bool Use(int useAmount)
    {
        if(useAmount > spiritPointAmount.GetAmount())
        {
            return false;
        }
        spiritPointAmount.ChangeAmount(useAmount);
        UpdateUI();
        return true;
    }

    public long GetAmount()
    {
        return spiritPointAmount.GetAmount();
    }

    public PlayerStat GetPlayerStat()
    {
        return spiritPointAmount;
    }

    void UpdateUI()
    {
        displayText.text = "" + spiritPointAmount.GetAmount();
        FindObjectOfType<SpiritPointWarning>().TextHighlight();

    }
}
