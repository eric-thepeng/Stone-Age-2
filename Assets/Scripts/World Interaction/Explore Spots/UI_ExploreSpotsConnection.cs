using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class UI_ExploreSpotsConnection : MonoBehaviour
{
    // Singleton
    static UI_ExploreSpotsConnection instance;
    public static UI_ExploreSpotsConnection i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_ExploreSpotsConnection>();
            }
            return instance;
        }
    }

    [Serializable]public class ConnectionLine
    {
        public GameObject lineGameObject;

        public SO_SerialEffectIdentifier serialEffectIdentifier_1 = null;
        public SO_SerialEffectIdentifier serialEffectIdentifier_2 = null;



        public void ChangeColor(Color32 tarColor)
        {
            lineGameObject.GetComponent<SpriteRenderer>().color = tarColor;
        }

        public bool CheckSEIMatch(SO_SerialEffectIdentifier sei1, SO_SerialEffectIdentifier sei2)
        {
            if (serialEffectIdentifier_1 == sei1 & serialEffectIdentifier_2 == sei2) return true;
            if (serialEffectIdentifier_1 == sei2 & serialEffectIdentifier_2 == sei1) return true;
            return false;
        }
    }

    public SO_SerialEffectIdentifierGroup TargetSEIGroup = null;
    
    /* Two Types of Lines: 
     * Hidden - Hidden: hiddenColor
     * Hidden - Locked: discoveredColor
     * Locked - Unlocked: discoveredColor
     */
    [Header("Hidden Color setup does not work rn")]
    public Color32 hiddenColor;
    public Color32 discoveredColor;
    public GameObject lineTemplate;
    public List<ConnectionLine> allConnectionLines = new List<ConnectionLine>();

    public void UnlockLine(SO_SerialEffectIdentifier origionSEI, SO_SerialEffectIdentifier targetSEI)
    {
        foreach (ConnectionLine cline in allConnectionLines)
        {
            if(cline.CheckSEIMatch(origionSEI, targetSEI)) cline.ChangeColor(discoveredColor);
        }
    }
}
