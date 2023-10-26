using System.Collections;
using System.Collections.Generic;
using Hypertonic.GridPlacement;
using UnityEngine;

public class BuildingInteractable : WorldInteractable
{

    protected override void BeginMouseHover()
    {
        mouseHovering = true;
        if (!GridManagerAccessor.GridManager.IsPlacingGridObject)
        {
            TurnOnHighlight();
        }

    }
}
