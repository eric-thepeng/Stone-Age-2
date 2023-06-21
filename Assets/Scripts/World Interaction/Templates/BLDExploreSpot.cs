using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLDExploreSpot : LevelUp
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            unlockToState_1_Locked();
        }
    }

    public void unlockToState_1_Locked()
    {
        UnlockToNextState();
    }

    public void unlockToState_2_Unlocked()
    {
        UnlockToNextState();
    }

}
