using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpiritPoint : MonoBehaviour
{
    static SpiritPoint instance;
    public int startingAmount;
    TextMeshPro displayText;
    [ReadOnly,SerializeField]private long amount;
    [ReadOnly, SerializeField] private long historyAmount;

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
        historyAmount = startingAmount;
        displayText = transform.Find("Spirit Point UI").Find("Spirit Point Amount").GetComponent<TextMeshPro>();
        UpdateUI();
    }

    public void Add(int addAmount)
    {
        amount += addAmount;
        historyAmount += addAmount;
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

    public long GetAmount()
    {
        return amount;
    }

    void UpdateUI()
    {
        displayText.text = "" + amount;
    }
}
