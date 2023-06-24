using Hypertonic.GridPlacement.Models;
using UnityEngine;

namespace Hypertonic.GridPlacement.Example.PaintMode
{
    public class DeleteGridObjectManager : MonoBehaviour
    {
        private void Update()
        {
            // Check input from new Input System
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Mouse.current.rightButton.isPressed)
            {
                FireDeleteRay(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
            }
#endif


            // Check input from old Input System
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetMouseButtonDown(1))
            {
                 FireDeleteRay(Input.mousePosition);
            }
#endif
        }

        /// <summary>
        /// Fire a raycast from the mouse position. If the raycast hits a grid object it'll be deleted
        /// </summary>
        private void FireDeleteRay(Vector2 mousePosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                Transform gameObjectHit = hit.transform;
                
                GridObjectInfo gridObjectInfo = gameObjectHit.GetComponent<GridObjectInfo>();

                // Check to see if a grid object was hit
                if(gridObjectInfo != null && gridObjectInfo.HasBeenPlaced)
                {
                    GridManagerAccessor.GridManager.DeleteObject(gameObjectHit.gameObject, false);
                }

            }
        }
    }
}