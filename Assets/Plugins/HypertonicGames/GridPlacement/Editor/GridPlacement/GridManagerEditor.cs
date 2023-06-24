using UnityEngine;
using UnityEditor;

namespace Hypertonic.GridPlacement.Editor
{
    [CustomEditor(typeof(GridManager))]
    public class GridManagerEditor : UnityEditor.Editor
    {
        GridManager _gridManager;
        GridSettings _gridSettings;

        public void OnEnable()
        {
            _gridManager = (GridManager)target;
            _gridSettings = _gridManager.GridSettings;
        }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            if (_gridSettings == null) return;

            if (GUILayout.Button("Display Grid"))
            {
                _gridManager.DisplayGridEditor();
            }


            if (GUILayout.Button("Hide Grid"))
            {
                _gridManager.HideGridEditor();
            }

            if (GUILayout.Button("Create Grid Placement Unity Layers"))
            {
                GridLayersManager.CreateLayersIfNotPresent(new string[] { "Grid", "PlacementGrid" });
            }
        }
    }
}