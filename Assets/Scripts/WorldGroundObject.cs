
using UnityEngine;

public class WorldGroundObject : MonoBehaviour
{
    private void OnMouseUpAsButton()
    {
        ClickInteraction();
    }

    private void OnMouseEnter()
    {
        EnterHover();
    }

    private void OnMouseExit()
    {
        ExitHover();
    }

    protected virtual void ClickInteraction()
    {
        print("Click Interaction: " + gameObject.name);
    }

    protected virtual void EnterHover()
    {
        print("Enter Hover: " + gameObject.name);
    }

    protected virtual void ExitHover()
    {
        print("Exit Hover: " + gameObject.name);
    }
}
