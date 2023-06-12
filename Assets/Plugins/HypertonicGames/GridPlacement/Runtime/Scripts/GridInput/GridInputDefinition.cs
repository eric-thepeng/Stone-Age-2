using UnityEngine;

namespace Hypertonic.GridPlacement.GridInput
{
    /// <summary>
    /// An abstract class that contains the functions needed to provide input to the Grid Manager.
    /// Implementations of this class will be able to define how the players interact with the grid.
    /// </summary>
    public abstract class GridInputDefinition : ScriptableObject
    {
        private bool _enabled = true;

        /// <summary>
        /// Determines if the input definition should be used or not. This is based on if the definition has been enabled
        /// and if the implementation of should interact is also true
        /// </summary>
        /// <returns></returns>
        public bool CanInteract()
        {
            if (!_enabled)
                return false;

            return ShouldInteract();
        }

        /// <summary>
        /// Defines if the input should update the grid objects position or be ignored.
        /// </summary>
        /// <returns></returns>
        public abstract bool ShouldInteract();

        /// <summary>
        /// Defnines the position of the user's input to the Grid System. This position will be used to fire a raycast from the given 
        /// screen position to see where on the grid it hits, if at all.
        /// </summary>
        /// <returns> The position of the screen to fire a raycast from </returns>
        public abstract Vector3? InputPosition();

        private void OnEnable()
        {
            _enabled = true;
        }

        /// <summary>
        /// Enables the detection of input onto the grid
        /// </summary>
        public void EnableInput()
        {
            _enabled = true;
        }

        /// <summary>
        /// Disables the detection of input onto the grid
        /// </summary>
        public void DisableInput()
        {
            _enabled = false;
        }
    }
}