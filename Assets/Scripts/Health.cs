using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    float maxHealth = 100;
    float health;
    static Health instance;
    Health_UI ui;
    public static Health i
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<Health>();
            }
            return instance;
        }
    }

    private void Start()
    {
        health = maxHealth;
        ui = FindObjectOfType<Health_UI>();
    }
    void UpdateUI()
    {
        ui.SetToRatio(Mathf.Clamp(health/maxHealth, 0, 1));
    }

    public void ChangeHealth(float amount)
    {
        health += amount;
        UpdateUI();
    }
}
