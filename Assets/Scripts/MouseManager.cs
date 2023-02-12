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
            if(WorldUtility.TryMouseHitPoint(20, true))
            {
                ExploreSpot es = WorldUtility.GetMouseHitObject(20, true).GetComponent<ExploreSpot>();
                ExploreSpotViewer.i.DisplayExploreSpot(es);
                mouseState = MouseState.ViewingExploreSpot;
            }
        }

        if(mouseState == MouseState.ViewingExploreSpot)
        {
            if (!WorldUtility.TryMouseHitPoint(20, true))
            {
                ExploreSpotViewer.i.CancelDisplay();
                mouseState = MouseState.Browsing;
            }
        }
    }
}
