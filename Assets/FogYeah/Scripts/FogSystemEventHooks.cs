using UnityEngine;
using static FogSystems.FogSystemEnums;

namespace FogSystems
{
    /// <summary>
    /// Event hooks for adding your own custom logic to behavior by the Fog System.
    /// </summary>
    public class FogSystemEventHooks : MonoBehaviour
    {
        /// <summary>
        /// When a group of fog items change state, this method will be called.
        /// For an example of this usage, see example scene: 12_Under_Fog_Extension_Example
        /// </summary>
        /// <param name="ids">The Fog Tile ID's that have changed state.</param>
        /// <param name="state">The FogState enum that it is changing to.</param>
        public delegate void OnFogItemsChangedState(int[] ids, FogState state);

        /// <summary>
        /// Hook to this event to be notified every time a fog item or group of fog items has changed state. 
        /// </summary>
        public event OnFogItemsChangedState OnStateChangedForFogItemIDs;

        /// <summary>
        /// This method will fire after the fog system begins tracking movement.
        /// </summary>
        public delegate void OnFogSystemStarted();

        /// <summary>
        /// This even will fire after the fog system begins tracking movement.
        /// </summary>
        public event OnFogSystemStarted OnFogSystemHasStarted;

        // As events cannot be invoked from an outside source, create the invokation methods inside the event hooks class, and call the internal method each time the invokation is required.
        #region Invokations

        /// <summary>
        /// When a group of fog items change state, this method will be called internally.
        /// For an example of this usage, see example scene: 12_Under_Fog_Extension_Example
        /// </summary>
        /// <param name="ids">The Fog Tile ID's that have changed state.</param>
        /// <param name="state">The FogState enum that it is changing to.</param>
        internal void InvokeOnStateChanged(int[] ids, FogState state) => OnStateChangedForFogItemIDs?.Invoke(ids, state);

        /// <summary>
        /// This method fires at the end of the awake of FogSystem.
        /// </summary>
        internal void InvokeOnFogSystemStarted() => OnFogSystemHasStarted?.Invoke();

        #endregion
    }
}