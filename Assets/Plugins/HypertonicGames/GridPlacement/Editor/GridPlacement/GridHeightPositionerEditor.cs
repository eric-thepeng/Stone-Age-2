using Hypertonic.GridPlacement.CustomSizing;
using UnityEditor;
using UnityEngine;

namespace Hypertonic.GridPlacement.Editor
{
    [CustomEditor(typeof(GridHeightPositioner))]
    public class GridHeightPositionerEditor : UnityEditor.Editor
    {
        public void OnEnable()
        {
            SceneView.duringSceneGui += HandleSceneGUI;
        }

        public void OnDisable()
        {
            SceneView.duringSceneGui -= HandleSceneGUI;
        }

        private void HandleSceneGUI(SceneView sv)
        {
            GridHeightPositioner gridHeightPositioner = (GridHeightPositioner)target;

            if (!gridHeightPositioner.EnableHandles) return;

            Vector3 gameObjectPosition = gridHeightPositioner.gameObject.transform.position;

            CustomBoundsController customBoundsController = gridHeightPositioner.gameObject.GetComponent<CustomBoundsController>();

            Vector3 boundsCenter = new Vector3();

            if(customBoundsController != null)
            {
                boundsCenter = customBoundsController.Bounds.center;
            }

            Vector3 point = gameObjectPosition += new Vector3(boundsCenter.x, gridHeightPositioner.GridHeight, boundsCenter.z);

            point = Handles.PositionHandle(point, Quaternion.identity);
            Handles.Label(point, "Grid Height Position");

            if (GUI.changed)
            {
                Undo.RecordObject(target, "Move Grid Height Position");
                gridHeightPositioner.GridHeight = point.y;
            }
        }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            GUILayout.Space(50);

            if (GUILayout.Button("Set grid height to world zero"))
            {
                GridHeightPositioner gridHeightPositioner = (GridHeightPositioner)target;
             
                Undo.RecordObject(gridHeightPositioner, "Changed Grid Height");

                float objectYPos = gridHeightPositioner.gameObject.transform.position.y;

                gridHeightPositioner.GridHeight = -objectYPos;

                SceneView.RepaintAll();
            }
        }
    }
}
