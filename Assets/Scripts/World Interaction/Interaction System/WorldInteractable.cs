using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Hypertonic.GridPlacement;
using Newtonsoft.Json;
using UnityEngine.Events;

public class MouseState
{
    public bool isHovering = false;
    public bool isPressing = false;
    public float pressingDuration = 0f;
}

public class WorldInteractable : MonoBehaviour
{
    //Interaction Type Realted
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
        /// <summary>
        /// Advance pressing progression by delta. Returns if this advancement completes the interaction.
        /// </summary>
        /// <returns></returns>
        public bool AdvanceProgress()
        {
            progressDuration += Time.deltaTime;
            UniversalUIManager.i.DisplayComponent(this);
            if (progressDuration >= pressDuration)
            {
                CompleteInteraction();
                return true;
            }
            return false;
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

    [SerializeField]private InteractionType currentInteraction = null;

    //Variables
    private MouseState mouseState = new MouseState();

    //Functions
    public void SetCurrentInteraction(InteractionType newInteractionType)
    {
        currentInteraction = newInteractionType;
    } 

    //BISO and CanInteract
    protected bool isBuildingInteractable
    {
        get { return this is BuildingInteractable; }
    }

    protected bool CanInteract()
    {
        if (GridManagerAccessor.GridManager.IsPlacingGridObject) return false;
        if (isBuildingInteractable)
        {
            return ((BuildingInteractable)this).allowInteraction;
        }
        else
        {
            return true;
        }
    }
    
    
    //Action: HOVER//
    protected virtual void BeginMouseHover()
    {
        if(!CanInteract()) return;
        TurnOnHighlight();
        currentInteraction?.DisplayUI(true);
    }

    protected virtual void EndMouseHover()
    {
        if(!CanInteract()) return;
        TurnOffHighlight();
        currentInteraction?.DisplayUI(false);
    }

    //Action: PRESSING//
    protected virtual void BeginMousePress()
    {
        if(!CanInteract()) return;
    }

    protected virtual void EndMousePress()
    {
        if(!CanInteract()) return;
        currentInteraction?.ResetProgress(true);
    }

    protected virtual void WhileMousePress()
    {
        if(!CanInteract()) return;
        if (currentInteraction != null)
        {
            if (currentInteraction.AdvanceProgress())
            {
                OnMouseUp();
            }
        }
        if (currentInteraction != null && currentInteraction.IsCompleted())
        {
            currentInteraction = null;
        }
    }

    //Action: CLICK//
    protected virtual void MouseClick()
    {
        if(!CanInteract()) return;
        if (!mouseState.isPressing) return; //To prevent the case that a click event is triggered instantly after a pressing event
        currentInteraction?.CompleteClick();
        if (currentInteraction!=null && currentInteraction.IsCompleted())
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
        mouseState.isHovering = true;
    }

    private void OnMouseExit()
    {
        EndMouseHover();
        mouseState.isHovering = false;
    }

    private void OnMouseUpAsButton()
    {
        MouseClick();
        OnMouseUp();
    }
    
    private void OnMouseDown()
    {
        BeginMousePress();
        mouseState.isPressing = true;
    }

    private void OnMouseDrag()
    {
        WhileMousePress();
        if (mouseState.isPressing)
        {
            mouseState.pressingDuration += Time.deltaTime;
        }
    }

    private void OnMouseUp()
    {
        EndMousePress();
        mouseState.isPressing = false;
        
    }

    #endregion

}
