using UnityEngine;

namespace Hypertonic.GridPlacement.CustomSizing
{
    /// <summary>
    /// This class allows you to set where the object will be vertically aligned to on the grid.
    /// Most common use cases for this will be to set the grid height to the base of the object
    /// </summary>
    public class GridHeightPositioner : MonoBehaviour
    {
        public float GridHeight
        {
            get { return _gridHeight; }
            set { _gridHeight = value; }
        }

        public bool EnableHandles => _enableHandles;

        [Header("Height Settings")]
        [SerializeField]
        [Tooltip("The grid height is used to set the vertical position of where the y axis will be snapped to on the grid")]
        private float _gridHeight = 0f;
      
        [Header("Gizmo Settings")]

        [SerializeField]
        [Tooltip("Displays a gizmo in the scene view to visulise the position of the grid height position")]
        private bool _drawGridHeightGizmo = true;

        [SerializeField]
        [Tooltip("Sets the size of the grid height gizmo in the scene view")]
        private float _gridHeightGizmoSize = 0.05f;

        [SerializeField]
        [Tooltip("Sets the colour of the grid height gizmo")]
        private Color _gridHeightGizmoColor = Color.magenta;

        [SerializeField]
        [Tooltip("When enabled display the unity handle gizmo to enable you to move the position of the grid height")]
        private bool _enableHandles = true;


        private void OnDrawGizmos()
        {
            if (!_drawGridHeightGizmo)
                return;

            Vector3 gizmoPosition = transform.position + new Vector3(0, GridHeight, 0);

            if(TryGetBoundsController(out CustomBoundsController boundsController))
            {
                gizmoPosition += boundsController.Bounds.center;
            }

            Gizmos.color = _gridHeightGizmoColor;
            Gizmos.DrawSphere(gizmoPosition, _gridHeightGizmoSize);
        }

        private bool TryGetBoundsController(out CustomBoundsController customBoundsController)
        {
            customBoundsController = null;

            CustomBoundsController controller = GetComponent<CustomBoundsController>();
            
            if(controller != null)
            {
                customBoundsController = controller;
                return true;
            }

            return false;
        }
    }
}
