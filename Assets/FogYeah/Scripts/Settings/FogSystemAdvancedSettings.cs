using UnityEngine;

namespace FogSystems
{
    /// <summary>
    /// System Settings - These should remain unchanged unless you have a use-case specific need to do so.
    /// </summary>
    [System.Serializable]
    public class FogSystemAdvancedSettings
    {
#pragma warning disable 649
        [SerializeField]
        [Tooltip("Prefab that houses the fog tile image.")]
        internal GameObject FogTilePrefab;

        [SerializeField]
        [Tooltip("Canvas that will be used as parent to the tiles.")]
        internal GameObject FogCanvasPrefab;

        [SerializeField]
        [Tooltip("Canvas that will be covered by the fog tiles.")]
        internal GameObject MapCanvasPrefab;

        [HideInInspector]
        [Tooltip("Fog canvas that was created by the system.")]
        public GameObject FogCanvas;
#pragma warning restore 649
    }
}