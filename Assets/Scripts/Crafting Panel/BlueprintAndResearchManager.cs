using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintAndResearchManager : MonoBehaviour
{
    [SerializeField] private GameObject researchPanelGO, blueprintPanelGO;
    [SerializeField] private Vector3 activePanelLocalPosition;
    
    public void TabSwitchToResearch()
    {
        researchPanelGO.transform.localPosition = activePanelLocalPosition;
        blueprintPanelGO.transform.localPosition = activePanelLocalPosition + new Vector3(0,-10,0);
    }

    public void TabSwitchToBlueprint()
    {
        researchPanelGO.transform.localPosition = activePanelLocalPosition + new Vector3(0,-10,0);
        blueprintPanelGO.transform.localPosition = activePanelLocalPosition;
    }
}
