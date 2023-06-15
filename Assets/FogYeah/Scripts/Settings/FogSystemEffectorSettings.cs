using UnityEngine;

namespace FogSystems
{
    [System.Serializable]
    public class FogSystemEffectorSettings
    {
        /// <summary>
        /// This flag is used to automatically create the effector by using the Player Tag on fog system awake.
        /// </summary>
        [SerializeField]
        [Tooltip("If checked, searches the scene for players, and adds them to the Effector list at startup.")]
        internal bool AutoCreateEffectorFromPlayers = true;

        /// <summary>
        /// Creates a cube that can be moved around in the inspector to demonstrate a working effector.
        /// </summary>
        [SerializeField]
        [Tooltip("Create an example Cube primitive effector that can be used as an example for testing purposes.")]
        public bool CreateExampleEffector = false;
    }
}