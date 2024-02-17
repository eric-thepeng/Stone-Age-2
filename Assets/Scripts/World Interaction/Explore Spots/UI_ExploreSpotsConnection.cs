using System;
using System.Collections;
using System.Collections.Generic;
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
        public MonoBehaviour iSerialEffect1;
        public MonoBehaviour iSerialEffect2;

        private SO_SerialEffectIdentifier _se1 = null;
        private SO_SerialEffectIdentifier _se2 = null;

        public SO_SerialEffectIdentifier se1
        {
            get
            {
                if (_se1 == null)
                {
                    SO_SerialEffectIdentifier ise = ((ISerialEffect)iSerialEffect1).mySEI;
                }
                return _se1;
            }
        }
        
        public SO_SerialEffectIdentifier se2
        {
            get
            {
                if (_se2 == null)
                {
                    SO_SerialEffectIdentifier ise = ((ISerialEffect)iSerialEffect2).mySEI;
                }

                return _se2;
            }
        }
        
        

        public void ChangeColor(Color32 tarColor)
        {
            lineGameObject.GetComponent<SpriteRenderer>().color = tarColor;
        }

        public bool CheckSEIMatch(SO_SerialEffectIdentifier sei1, SO_SerialEffectIdentifier sei2)
        {
            if (se1 == sei1 & se2 == sei2) return true;
            if (se1 == sei2 & se2 == sei1) return true;
            return false;
        }
    }
    
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
