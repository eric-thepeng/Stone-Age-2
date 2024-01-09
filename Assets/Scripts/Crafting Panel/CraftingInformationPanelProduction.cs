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
        UpdateButtonText();
    }
    
    public void ChangeAmount(int delta)
    {
        currentAmount += delta;
        if (currentAmount < 0) currentAmount = 0;
        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        buttonTMP.text = "Craft ( " + currentAmount + " )";
    }
    
}
