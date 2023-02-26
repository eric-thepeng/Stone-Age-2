using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpiritPoint : MonoBehaviour
{
    static SpiritPoint instance;

    public int startingAmount;

    int amount;

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

    private void Start()
    {
        amount = startingAmount;

        UpdateUI();
    }

    public void Add(int addAmount)
    {
        amount += addAmount;
        UpdateUI();
    }

    public bool Use(int useAmount)
    {
        if(useAmount > amount)
        {
            return false;
        }
        amount -= useAmount;
        UpdateUI();
        return true;
    }

    void UpdateUI()
    {
        GetComponentInChildren<TextMeshPro>().text = "" + amount;
    }
}
