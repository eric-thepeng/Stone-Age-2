using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreSpotUnlockButton : WorldSpaceButton
{
    public override void PressedAction()
    {
        ExploreSpotViewer.i.UnlockSpot();
    }
}
