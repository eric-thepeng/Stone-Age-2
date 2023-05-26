using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLDTrashToClear : LevelUp
{
    private GameObject UI;
    protected override void BeginMouseHover()
    {
        base.BeginMouseHover();
        TurnOnUI();
    }

    private void TurnOnUI()
    {
        UI.SetActive(true);
    }

    public void TurnOffUI()
    {
        UI.SetActive(false);
    }
}
