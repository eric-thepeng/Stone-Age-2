using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLDTrashToClear : LevelUp
{
    [SerializeField]private GameObject UI;
    protected override void BeginMouseHover()
    {
        base.BeginMouseHover();
        TurnOnUI();
    }

    private void TurnOnUI()
    {
        UI.SetActive(true);
        IPassResourceSet iprs = GetComponent<IPassResourceSet>();
        if(iprs != null)iprs.PassResourceSet(GetCurrentUnlockState().unlockCost);
    }

    public void TurnOffUI()
    {
        UI.SetActive(false);
    }
}
