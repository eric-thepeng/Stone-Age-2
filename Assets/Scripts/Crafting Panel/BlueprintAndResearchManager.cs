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
    
    [SerializeField] private Animator blueprintPanelAnimator;
    [SerializeField] private Animator researchPanelAnimator;

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
        
        //THIS TWO LINES
        researchPanelAnimator.SetTrigger("TriggerOpen");
        blueprintPanelAnimator.SetTrigger("TriggerClose");
        researchPanelGO.transform.localPosition = activePanelLocalPosition;
        blueprintPanelGO.transform.localPosition = activePanelLocalPosition + new Vector3(0,-10,0);
        //Until here
        
        researchSwitchButton.SetActive(false);
        blueprintSwitchButton.SetActive(true);
    }

    public void TabSwitchToBlueprint()
    {
        isResearchPanelOpen = false;
        isBlueprintPanelOpen = true;
        
        //THIS TWO LINES
        researchPanelAnimator.SetTrigger("TriggerClose");
        blueprintPanelAnimator.SetTrigger("TriggerOpen");
        researchPanelGO.transform.localPosition = activePanelLocalPosition + new Vector3(0,-10,0);
        blueprintPanelGO.transform.localPosition = activePanelLocalPosition;
        //Until here
        
        researchSwitchButton.SetActive(true);
        blueprintSwitchButton.SetActive(false);
        
        
    }

    public void OpenPanel()
    {
        TabSwitchToBlueprint();
    }

    public bool isBnROpen()
    {
        if(isResearchPanelOpen || isBlueprintPanelOpen)
        {
            return true;
        }
        return false;
    }
    
    public void ClosePanel()
    {
        isResearchPanelOpen = false;
        isBlueprintPanelOpen = false;
    }
}
