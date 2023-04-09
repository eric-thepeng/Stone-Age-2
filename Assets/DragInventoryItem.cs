using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragInventoryItem : MonoBehaviour
{
    UI_InventoryBlock myUIIBlock;
    protected bool placed = false;

    public virtual void SetUp(UI_InventoryBlock uiib)
    {
        myUIIBlock = uiib;
    }

    protected void PlaceDrag()
    {
        if (placed) return;
        placed = true;
        myUIIBlock.PlaceDrag();
    }
}
