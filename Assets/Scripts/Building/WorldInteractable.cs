using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldInteractable : MonoBehaviour
{
    private bool mouseHover = false;
    protected virtual void BeginMouseHover()
    {
        mouseHover = true;
    }

    protected virtual void EndMouseHover()
    {
        
    }
    
    protected virtual void MouseClick()
    {
        mouseHover = false;
    }

    protected bool isMouseHover()
    {
        return mouseHover;
    }

    /*
     * TEMPORARY CODE
     * INTERACTION WITH MOUSE WILL BE REPLACED BY A RAYCAST BASED SYSTEM INITIATED BY MOUSE MANAGER
     */
    private void OnMouseEnter()
    {
        BeginMouseHover();
    }

    private void OnMouseExit()
    {
        EndMouseHover();
    }

    private void OnMouseUpAsButton()
    {
        MouseClick();
    }
    
}
