using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintAndResearchManager : MonoBehaviour
{
    static BlueprintAndResearchManager instance;
    public static BlueprintAndResearchManager i
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<BlueprintAndResearchManager>();
            }
            return instance;
        }
    }
    
    [SerializeField] private GameObject researchPanelGO, blueprintPanelGO;
    [SerializeField] private GameObject researchSwitchButton, blueprintSwitchButton;
    [SerializeField] private Vector3 activePanelLocalPosition;

    private bool isResearchPanelOpen = false;
    private bool isBlueprintPanelOpen = false;

    public bool IsResearchPanelOpen()
    {
        return isResearchPanelOpen;
    }

    public bool IsBlueprintPanelOpen()
    {
        return isBlueprintPanelOpen;
    }
    
    public void TabSwitchToResearch()
    {
        isResearchPanelOpen = true;
        isBlueprintPanelOpen = false;
        researchPanelGO.transform.localPosition = activePanelLocalPosition;
        blueprintPanelGO.transform.localPosition = activePanelLocalPosition + new Vector3(0,-10,0);
        researchSwitchButton.SetActive(false);
        blueprintSwitchButton.SetActive(true);
    }

    public void TabSwitchToBlueprint()
    {
        isResearchPanelOpen = false;
        isBlueprintPanelOpen = true;
        researchPanelGO.transform.localPosition = activePanelLocalPosition + new Vector3(0,-10,0);
        blueprintPanelGO.transform.localPosition = activePanelLocalPosition;
        researchSwitchButton.SetActive(true);
        blueprintSwitchButton.SetActive(false);
    }

    public void OpenPanel()
    {
        TabSwitchToBlueprint();
    }
    
    public void ClosePanel()
    {
        isResearchPanelOpen = false;
        isBlueprintPanelOpen = false;
    }
}
