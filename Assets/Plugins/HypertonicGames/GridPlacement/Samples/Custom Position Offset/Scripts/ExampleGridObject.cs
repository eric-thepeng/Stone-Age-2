using UnityEngine;
using UnityEngine.EventSystems;

namespace Hypertonic.GridPlacement.Example.PositionOffsetDemo
{
    /// <summary>
    /// This is an example of a component that should be added to an object you place on the grid.
    /// This simple implementation just fires of an event so the demo scene knows that the player wants to
    /// modify the position of the grid object.
    /// 
    /// For the purposes of the adjust placement height demo it also contains an enum to determine the animation type.
    /// </summary>
    public class ExampleGridObject : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public delegate void OnObjectSelectedEvent(GameObject gameObject);
        public static event OnObjectSelectedEvent OnObjectSelected;

        public PlacementAnimationType PlacementAnimationType { get => _placementAnimationType; }

        [SerializeField]
        private PlacementAnimationType _placementAnimationType = PlacementAnimationType.NONE;

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