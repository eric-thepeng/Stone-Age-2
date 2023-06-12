using UnityEngine;
using UnityEngine.EventSystems;

namespace Hypertonic.GridPlacement.Example.MoveGridDemo
{
    /// <summary>
    /// This is an example of a component that should be added to an object you place on the grid.
    /// This simple implementation just fires of an event so the demo scene knows that the player wants to
    /// modify the position of the grid object.
    /// </summary>
    public class ExampleGridObject : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public delegate void OnObjectSelectedEvent(GameObject gameObject);
        public static event OnObjectSelectedEvent OnObjectSelected;

        public void OnPointerUp(PointerEventData eventData)
        {
            OnObjectSelected?.Invoke(gameObject);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // This handler declaration is needed if the OnPointerUp Handler is implemented.
        }
    }
}