using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    static MouseManager instance;
    public static MouseManager i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MouseManager>();
            }
            return instance;
        }
    }

    public enum MouseState {Browsing, DraggingCharacterIcon}
    private MouseState mouseState = MouseState.Browsing;

    private void Update()
    {
        /*
        if (IsState(MouseState.Browsing))
        {
            if (WorldUtility.TryMouseHitPoint(WorldUtility.LAYER.EXPLORATION_SPOT, true))
            {
                ExploreSpot es = WorldUtility.GetMouseHitObject(WorldUtility.LAYER.EXPLORATION_SPOT, true).GetComponent<ExploreSpot>();
                //print(WorldUtility.GetMouseHitObject(WorldUtility.LAYER.EXPLORATION_SPOT, true).name);
                ExploreSpotViewer.i.DisplayExploreSpot(es);
                ChangeMouseState(MouseState.ViewingExploreSpot);
            }
        }*/
        /*
        if (mouseState == MouseState.ViewingExploreSpot)
        {
            if (!WorldUtility.TryMouseHitPoint(WorldUtility.LAYER.EXPLORATION_SPOT, true) && !WorldUtility.TryMouseHitPoint(WorldUtility.LAYER.EXPLORATION_SPOT_VIEWER, true))
            {
                ExploreSpotViewer.i.CancelDisplay();
                mouseState = MouseState.Browsing;
            }
        }*/
    }

    public void ChangeMouseState(MouseState toState)
    {
        if(toState == MouseState.Browsing)
        {
            transform.Find("Background").gameObject.SetActive(false);
        }
        else
        {
            transform.Find("Background").gameObject.SetActive(true);
        }
        mouseState = toState;
    }

    public bool IsState(MouseState isThisState)
    {
        
        return isThisState == mouseState;
    }
}
