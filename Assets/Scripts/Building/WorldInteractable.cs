using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldInteractable : MonoBehaviour
{
    //hovering//
    private bool mouseHovering = false;
    protected virtual void BeginMouseHover()
    {
        mouseHovering = true;
    }

    protected virtual void EndMouseHover()
    {
        mouseHovering = false;
    }

    protected bool isMouseHovering()
    {
        return mouseHovering;
    }

    //pressing//
    private bool mousePressing = false;

    protected virtual void BeginMousePress()
    {
        mousePressing = true;
    }

    protected virtual void EndMousePress()
    {
        mousePressing = false;
    }

    protected virtual void WhileMousePress()
    {

    }

    protected bool isMousePressing()
    {
        return mousePressing;
    }

    //click//
    protected virtual void MouseClick()
    {

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

    private void OnMouseDown()
    {
        BeginMousePress();
    }

    private void OnMouseDrag()
    {
        WhileMousePress();
    }

    private void OnMouseUp()
    {

        EndMousePress();
    }

}
