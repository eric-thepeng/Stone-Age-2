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
        
        //subscribe
    }

    public void CancelCurrentSetUp()
    {
        
    }
    
    public void ChangeAmount(int delta)
    {
        currentAmount += delta;
        if (currentAmount < 1) currentAmount = 1;
        UpdateDisplay();
    }

    private void CheckEnoughMaterialStatus(int placeholder = 0)
    {
        if (HasEnoughMaterialToCraft())
        {
            
        }
        else
        {
            
        }
    }

    private bool HasEnoughMaterialToCraft()
    {
        foreach (var VARIABLE in displayingBlueprintCard.GetICSO().GetResourceSet().resources)
        {
            if (Inventory.i.GetISOInstockAmount(VARIABLE.iso) < VARIABLE.amount * currentAmount) return false;
        }
        return true;
    }

    public void CraftButton()
    {
        if(!HasEnoughMaterialToCraft()) return;
        foreach (var VARIABLE in displayingBlueprintCard.GetICSO().GetResourceSet().resources)
        {
            Inventory.i.UseItemFromStock(VARIABLE.iso, VARIABLE.amount * currentAmount);
        }
        Inventory.i.AddInventoryItem(displayingBlueprintCard.GetICSO().ItemCrafted,currentAmount);
        ChangeAmount(1-currentAmount);
    }

    private void UpdateDisplay()
    {
        // Update Button Text
        buttonTMP.text = "Craft ( " + currentAmount + " )";
        
        // Update Resource Set Display
        resourceSetDisplayer.Display(displayingBlueprintCard.GetICSO().GetResourceSet(),currentAmount);
    }
    
}
