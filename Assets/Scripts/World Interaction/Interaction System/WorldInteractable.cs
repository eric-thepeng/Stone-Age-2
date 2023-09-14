using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorldInteractable : MonoBehaviour
{
    //hovering//
    private bool mouseHovering = false;
    protected virtual void BeginMouseHover()
    {
        mouseHovering = true;
        TurnOnHighlight();
    }

    protected virtual void EndMouseHover()
    {
        mouseHovering = false;
        TurnOffHighlight();
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
    
    protected virtual void TurnOnHighlight()
    {
        transform.DOShakePosition(0.3f, new Vector3(0.1f,0,0),10,0);
    }
    
    protected virtual void TurnOffHighlight()
    {
        
    }
    
    #region Mouse Interaction Configuration
    
    
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

    #endregion

}
