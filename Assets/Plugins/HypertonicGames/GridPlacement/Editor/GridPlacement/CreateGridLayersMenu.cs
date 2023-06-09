using UnityEditor;
using UnityEngine;

namespace Hypertonic.GridPlacement.Editor
{
    public class CreateGridLayersMenu : MonoBehaviour
    {
        [MenuItem("Tools/Grid Placement/Create Grid Layers")]
        static void CreateGridLayers()
        {
            GridLayersManager.CreateLayersIfNotPresent(new string[] { "Grid", "PlacementGrid" });
        }
    }
}