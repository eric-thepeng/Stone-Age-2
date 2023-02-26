using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public enum MouseState {Browsing, DraggingCharacterIcon, ViewingExploreSpot}
    public static MouseState mouseState = MouseState.Browsing;

    private void Update()
    {
        if(mouseState == MouseState.Browsing)
        {
            if(WorldUtility.TryMouseHitPoint(WorldUtility.LAYER.EXPLORATION_SPOT, true))
            {
                ExploreSpot es = WorldUtility.GetMouseHitObject(WorldUtility.LAYER.EXPLORATION_SPOT, true).GetComponent<ExploreSpot>();
                print(WorldUtility.GetMouseHitObject(WorldUtility.LAYER.EXPLORATION_SPOT, true).name);
                ExploreSpotViewer.i.DisplayExploreSpot(es);
                mouseState = MouseState.ViewingExploreSpot;
            }
        }

        if(mouseState == MouseState.ViewingExploreSpot)
        {
            if (!WorldUtility.TryMouseHitPoint(WorldUtility.LAYER.EXPLORATION_SPOT, true) && !WorldUtility.TryMouseHitPoint(WorldUtility.LAYER.EXPLORATION_SPOT_VIEWER, true))
            {
                ExploreSpotViewer.i.CancelDisplay();
                mouseState = MouseState.Browsing;
            }
        }
    }
}
