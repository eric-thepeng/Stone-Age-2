using UnityEngine;
using UnityEditor;

namespace Hypertonic.GridPlacement.Editor
{
    public class CreateGridManagerMenu : MonoBehaviour
    {
        [MenuItem("Tools/Grid Placement/Create Grid Manager")]
        static void CreateGridManager()
        {
            GameObject gameObject = new GameObject("Grid Manager");
            gameObject.AddComponent<GridManager>();
        }
    }
}