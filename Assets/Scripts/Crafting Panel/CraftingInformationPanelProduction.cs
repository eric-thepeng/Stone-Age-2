using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CraftingInformationPanelProduction : MonoBehaviour
{
    [SerializeField] private TextMeshPro buttonTMP;
    [SerializeField] private ResourceSetDisplayer resourceSetDisplayer;
    private BlueprintCard displayingBlueprintCard;
    private int currentAmount = 0;
    
    public void SetUpToActive(BlueprintCard blueprintCard)
    {
        displayingBlueprintCard = blueprintCard;
        currentAmount = 1;
        UpdateDisplay();
    }
    
    public void ChangeAmount(int delta)
    {
        currentAmount += delta;
        if (currentAmount < 1) currentAmount = 1;
        UpdateDisplay();
    }

    public void CraftButton()
    {
        
    }

    private void UpdateDisplay()
    {
        // Update Button Text
        buttonTMP.text = "Craft ( " + currentAmount + " )";
        
        // Update Resource Set Display
        resourceSetDisplayer.Display(displayingBlueprintCard.GetICSO().GetResourceSet(),currentAmount);
    }
    
}
