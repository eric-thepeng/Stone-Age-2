using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Hypertonic.GridPlacement;
using Newtonsoft.Json;
using UnityEngine.Events;

public class WorldInteractable : MonoBehaviour
{
    [Serializable]
    public class InteractionType
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName">Click or Long Press</param>
        /// <param name="triggerAction">Assign what action/function to trigger as interaction completes</param>
        /// <param name="resetAfterTrigger">True: interaction reset and stays after being triggered, can be triggered again instantly. False: Interaction is claered and need to assign a new when it is ready for an interaction.</param>
        /// <param name="pressDuration">Leaves empty if it is a Click Event, otherwise, assign required press duration.</param>
        public InteractionType(TypeName typeName, UnityAction triggerAction, float pressDuration = 0)
        {
            this.typeName = typeName;
            triggerEvent = new UnityEvent();
            triggerEvent.AddListener(triggerAction);
            this.pressDuration = pressDuration;
            ResetProgress();
        }
        public enum TypeName { LongPress, Click }

        public TypeName typeName;
        [Header("Ignore if type is Click")]public float pressDuration;
        public UnityEvent triggerEvent;
        
        private float progressDuration;
        private bool isCompleted = false;

        public float GetProgressPercent()
        {
            return progressDuration/pressDuration;
        }
        public void ResetProgress(bool resetUI = false)
        {
            progressDuration = 0;
            if(resetUI)UniversalUIManager.i.DisplayComponent(this);
        }
        public void AdvanceProgress()
        {
            progressDuration += Time.deltaTime;
            UniversalUIManager.i.DisplayComponent(this);
            if (progressDuration >= pressDuration)
            {
                CompleteInteraction();
            }
        }

        public void CompleteClick()
        {
            if (typeName == TypeName.Click)
            {
                CompleteInteraction();
            }
        }
        private void CompleteInteraction()
        {
            isCompleted = true;
            triggerEvent.Invoke();
            DisplayUI(false);
        }
        public bool IsCompleted()
        {
            return isCompleted;
        }
        public void DisplayUI(bool display)
        {
            if (display)
            {
                UniversalUIManager.i.DisplayComponent(this);
            }
            else
            {
                UniversalUIManager.i.CancelDisplayComponent(this);
            }
        }
    }

    public InteractionType currentInteraction = null;

    //BISO and CanInteract
    protected bool isBuildingInteractable
    {
        get { return this is BuildingInteractable; }
    }

    protected bool CanInteract()
    {
        return !isBuildingInteractable || !GridManagerAccessor.GridManager.IsPlacingGridObject;
    }
    
    
    //Action: HOVER//
    [HideInInspector]
    public bool mouseHovering = false;
    protected virtual void BeginMouseHover()
    {
        if(!CanInteract()) return;
        mouseHovering = true;
        TurnOnHighlight();
        currentInteraction?.DisplayUI(true);
    }

    protected virtual void EndMouseHover()
    {
        if(!CanInteract()) return;
        mouseHovering = false;
        TurnOffHighlight();
        currentInteraction?.DisplayUI(false);
    }

    protected bool isMouseHovering()
    {
        return mouseHovering;
    }

    //Action: PRESSING//
    private bool mousePressing = false;

    protected virtual void BeginMousePress()
    {
        if(!CanInteract()) return;
        mousePressing = true;
    }

    protected virtual void EndMousePress()
    {
        if(!CanInteract()) return;
        mousePressing = false;
        currentInteraction?.ResetProgress(true);
    }

    protected virtual void WhileMousePress()
    {
        if(!CanInteract()) return;
        currentInteraction?.AdvanceProgress();
        if (currentInteraction.IsCompleted())
        {
            currentInteraction = null;
        }
    }

    protected bool isMousePressing()
    {
        return mousePressing;
    }

    //Action: CLICK//
    protected virtual void MouseClick()
    {
        if(!CanInteract()) return;
        currentInteraction?.CompleteClick();
        if (currentInteraction.IsCompleted())
        {
            currentInteraction = null;
        }
    }
    
    //UI Related//
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
