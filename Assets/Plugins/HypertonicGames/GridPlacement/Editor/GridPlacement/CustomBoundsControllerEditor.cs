using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;


namespace Hypertonic.GridPlacement.CustomSizing
{
    [CustomEditor(typeof(CustomBoundsController))]
    public class CustomBoundsControllerEditor : UnityEditor.Editor
    {
        private BoxBoundsHandle _boundsHandle = new BoxBoundsHandle();

        // the OnSceneGUI callback uses the scene view camera for drawing handles by default
        protected virtual void OnSceneGUI()
        {
            CustomBoundsController boundsController = (CustomBoundsController)target;

            GridHeightPositioner gridHeightPositioner = boundsController.GetComponent<GridHeightPositioner>();
            float gridHeight = gridHeightPositioner != null ? gridHeightPositioner.GridHeight : 0;

            // copy the target object's data to the handle
            _boundsHandle.center = new Vector3(
                boundsController.transform.position.x + boundsController.Bounds.center.x,
                boundsController.transform.position.y + gridHeight,
                boundsController.transform.position.z + boundsController.Bounds.center.z);

            _boundsHandle.size = new Vector3(
                boundsController.Bounds.size.x,
                0,
                boundsController.Bounds.size.z);


            // draw the handle
            EditorGUI.BeginChangeCheck();

            _boundsHandle.DrawHandle();

            if (EditorGUI.EndChangeCheck())
            {
                // The objects position was added so it was drawn in the correct position. It now needs to be removed when setting the value back in the bounds controller.
                _boundsHandle.center -= boundsController.transform.position;

                // record the target object before setting new values so changes can be undone/redone
                Undo.RecordObject(boundsController, "Change Bounds");

                // copy the handle's updated data back to the target object
                Bounds newBounds = new Bounds();
                newBounds.center = new Vector3(_boundsHandle.center.x, 0, _boundsHandle.center.z);
                newBounds.size = new Vector3(_boundsHandle.size.x, 0, _boundsHandle.size.z); ;
                boundsController.Bounds = newBounds;
            }
        }
    }
}