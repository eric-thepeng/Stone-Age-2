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
        public BLDExploreSpot exploreSpot1;
        public BLDExploreSpot exploreSpot2;

        public void ChangeColor(Color32 tarColor)
        {
            lineGameObject.GetComponent<SpriteRenderer>().color = tarColor;
        }

        public bool CheckSEIMatch(SO_SerialEffectIdentifier sei1, SO_SerialEffectIdentifier sei2)
        {
            if (exploreSpot1.mySEI == sei1 & exploreSpot2.mySEI == sei2) return true;
            if (exploreSpot1.mySEI == sei2 & exploreSpot2.mySEI == sei1) return true;
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
