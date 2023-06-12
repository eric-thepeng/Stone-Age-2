using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hypertonic.GridPlacement.GridInput
{
    public class PointerOverUIDetector : MonoBehaviour
    {
        /// <summary>
        /// Returns true if we touched or hovering on Unity UI element.
        /// </summary>
        /// <param name="inputPosition"> The position to fire the raycast from</param>
        /// <returns></returns>
        public static bool IsPointerOverUIElement(Vector3 inputPosition)
        {
            return IsPointerOverUIElement(GetEventSystemRaycastResults(inputPosition));
        }

        public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets all event systen raycast results of current input position.
        /// </summary>
        /// <param name="inputPosition"> The position to fire the raycast from</param>
        /// <returns></returns>
        private static List<RaycastResult> GetEventSystemRaycastResults(Vector3 inputPosition)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = inputPosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }
    }
}