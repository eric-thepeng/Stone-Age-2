using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceButton : MonoBehaviour
{
    [SerializeField] protected Color32 normalColor;
    [SerializeField] protected Color32 hoverColor;
    [SerializeField] protected Color32 pressColor;

    
    protected void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().color = hoverColor;
    }

    private void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().color = normalColor;
    }

    private void OnMouseDown()
    {
        GetComponent<SpriteRenderer>().color = pressColor;
    }

    private void OnMouseUpAsButton()
    {
        PressedAction();
    }

    public virtual void PressedAction()
    {

    }
}
