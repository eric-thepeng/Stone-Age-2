using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragInventoryItem : MonoBehaviour
{
    UI_InventoryBlock myUIIBlock;

    public void SetUp(UI_InventoryBlock uiib)
    {
        myUIIBlock = uiib;

    }

    public virtual void SendBackToBlock()
    {

    }
}
