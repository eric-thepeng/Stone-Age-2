using System.Collections;
using System.Collections.Generic;
using Hypertonic.GridPlacement;
using UnityEngine;

public class BuildingInteractable : WorldInteractable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void BeginMouseHover()
    {
        mouseHovering = true;
        if (!GridManagerAccessor.GridManager.IsPlacingGridObject)
        {
            TurnOnHighlight();
        }

    }
}
