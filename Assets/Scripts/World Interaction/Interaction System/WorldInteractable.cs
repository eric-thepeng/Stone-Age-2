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
        /// A standard interaction for WorldInteractable. Can be click or long press.
        /// </summary>
        /// <param name="typeName">Click, Long Press, NoAction</param>
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
        /// <returns>Returns if this advancement completes the interaction.</returns>
        public bool AdvanceProgress()
        {
            if (typeName != TypeName.LongPress) return false;
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
    public enum HighlightMode {NONE, SHAKE, OUTLINE }
    [SerializeField] private HighlightMode highlightMode = HighlightMode.NONE;
    private MouseState mouseState = new MouseState();
    private float outlineHighlightWidthRatio = 1.2f;
    private static Shader targetShader = null;

    //Functions
    public void SetCurrentInteraction(InteractionType newInteractionType)
    {
        currentInteraction = newInteractionType;
        if (mouseState.isHovering)
        {
            currentInteraction.DisplayUI(true);
        }
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
        if (currentInteraction == null)
        {
            UniversalUIManager.i.CancelDisplayComponent(null);
        }
        else
        {
            currentInteraction.DisplayUI(true);
        }
        
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
        switch (highlightMode)
        {
            case HighlightMode.SHAKE:
                transform.DOShakePosition(0.3f, new Vector3(0.1f,0,0),10,0);
                break;
            case HighlightMode.OUTLINE:
                OutlineHighlight(true);
                break;
        }
    }
    
    protected virtual void TurnOffHighlight()
    {
        switch (highlightMode)
        {
            case HighlightMode.SHAKE:
                break;
            case HighlightMode.OUTLINE:
                OutlineHighlight(false);
                break;
        }
    }

    public void OutlineHighlight(bool turnOn)
    {
        if (targetShader == null)
        {
            targetShader = Shader.Find("MK/Toon/URP/Standard/Physically Based + Outline");
        }
        
        Color outlineColor = turnOn ? Color.white :Color.black;
        //float outlineWidth = turnOn ? Color.white :Color.black;
        List<GameObject> allChildren = GetAllChildren();
        foreach (GameObject child in allChildren)
        {
            
            if (child.GetComponent<Renderer>() != null)
            {
                Material[] currentMats = child.GetComponent<Renderer>().materials;
                for (int i2 = 0; i2 < currentMats.Length; i2++)
                {
                    if (currentMats[i2].shader == targetShader)//HasProperty("_OutlineColor"))
                    {
                        currentMats[i2].SetColor("_OutlineColor", outlineColor);
                        float actualRatio = turnOn ? 1 * outlineHighlightWidthRatio : 1 / outlineHighlightWidthRatio;
                        currentMats[i2].SetFloat("_OutlineSize", currentMats[i2].GetFloat("_OutlineSize") * actualRatio);
                    }
                }
            }
        }
    }

    List<GameObject> GetAllChildren()
    {
        List<GameObject> listOfChildren = new List<GameObject>();
        GetAllChildrenRecursive(gameObject, listOfChildren);
        return listOfChildren;
    }

    private List<GameObject> GetAllChildrenRecursive(GameObject obj, List<GameObject> targetList)
    {
        if (obj == null)
        {
            return targetList;
        }

        foreach (Transform child in obj.transform)
        {
            if (child == null)
                continue;
            targetList.Add(child.gameObject);
            GetAllChildrenRecursive(child.gameObject, targetList);
        }

        return targetList;
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
