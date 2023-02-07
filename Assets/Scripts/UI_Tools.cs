using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Tools : MonoBehaviour
{
    static UI_Tools instance;
    public static UI_Tools i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_Tools>();
            }
            return instance;
        }
    }

    [SerializeField] GameObject Hand, Axe, Shovel, HLHand, HLAxe, HLShovel;
    bool alreadyHasAxe = false, alreadyHasShovel = false;

    public bool hasAxe() { return alreadyHasAxe; }
    public bool hasShovel() { return alreadyHasShovel; }

    public void addAxe()
    {
        if (alreadyHasAxe) return;
        alreadyHasAxe = true;
        Axe.SetActive(true);
    }

    public void addShovel()
    {
        if (alreadyHasShovel) return;
        alreadyHasShovel = true;
        Shovel.SetActive(true);
    }

    /// <summary>
    /// set tool ui 1: Hand 2: Shovel 3: Axe
    /// </summary>
    /// <param name="toWhich">  </param>
    public void switchTool(int toWhich)
    {
        if(toWhich == 1)
        {
            HLHand.SetActive(true);
            HLAxe.SetActive(false);
            HLShovel.SetActive(false);
        }
        else if(toWhich == 2)
        {
            HLHand.SetActive(false);
            HLAxe.SetActive(false);
            HLShovel.SetActive(true);
        }
        else if (toWhich == 3)
        {
            HLHand.SetActive(false);
            HLAxe.SetActive(true);
            HLShovel.SetActive(false);
        }
        else
        {
            print("error, invalid input at UI_Tools");
        }
    }
}
