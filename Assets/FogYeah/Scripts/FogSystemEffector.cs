using System.Collections.Generic;
using UnityEngine;

namespace FogSystems
{
    /// <summary>
    /// Fog system effectors effect fog by sight range specified.
    /// The effect that the effectors will have is defined in the fog system settings.
    /// </summary>
    [System.Serializable]
    public class FogSystemEffector
    {
        // Multiplier to allow for arbitrary values to be used instead of huge numbers for view distances / sight ranges.
        private const int SIGHT_RANGE_MULTIPLIER = 500;

        [Tooltip("Transform of the Effector.")]
        [SerializeField]
        private Transform effectorTransform;

        [Tooltip("How far can this effector see into the fog.")]
        [SerializeField]
        private float sightRange;

        [Tooltip("Will this effector ever move?")]
        [SerializeField]
        private bool isStatic;

        // Has this effector been actioned already? - Useful for Static effectors.
        private bool Actioned = false;

        // Should this effector be actioned?
        public bool HasBeenActioned() => Actioned;

        // Ensure the system skips this effector next cycle.
        public bool SetActioned(bool v) => Actioned = v;

        // Stores a position that can be read instead of transform.position inside of threads.
        private Vector3 threadSafePosition;

        // Stores the Fog Item Id's that are being revealed by this item
        private List<int> idsBeingCovered = new List<int>();

        /// <summary>
        /// Creates a FogSystemEffector that will reveal items around it based on a range specified.
        /// </summary>
        /// <param name="transform">The transform of this effector</param>
        /// <param name="sightRange">The distance between this transforms center and the fog items before it manipulates it</param>
        public FogSystemEffector(Transform transform, float sightRange)
        {
            effectorTransform = transform;

            // Multiplies the sight range to allow for a smaller value. Sight range multiplier is Safe to change.
            this.sightRange = sightRange;

            // If this item is static, we can assume it will not move. If you have issues with this, you should Not have static items that can move as effectors unless they are indeed static.
            isStatic = transform.gameObject.isStatic;
        }

        /// <summary>
        /// Grabs the transform component of this Effector
        /// </summary>
        /// <returns></returns>
        public Transform GetTransform() => effectorTransform;

        /// <summary>
        /// Checks to see if the transform has been fully created. Useful for items that are not fully set up in the editor during runtime.
        /// </summary>
        /// <returns>True if the transform is null</returns>
        public bool TransformIsNull() => effectorTransform == null;

        /// <summary>
        /// Gets the transform position (Not thread safe)
        /// </summary>
        /// <returns>The transform.position of the associated transform</returns>
        private Vector3 GetPosition() => effectorTransform.position;

        /// <summary>
        /// Gets the sight range specified when constructing this effector.
        /// </summary>
        /// <returns>Sight range value as an int</returns>
        public float GetSightRange() => sightRange * SIGHT_RANGE_MULTIPLIER;

        /// <summary>
        /// Thread safe vector3 version of transform.position. Created and refreshed when the system is checking if a search is nescessary.
        /// </summary>
        /// <returns>A thread safe vector3 of the transform.position</returns>
        public Vector3 GetThreadsafePosition() => threadSafePosition;

        /// <summary>
        /// Checks if the transform associated with this effector has changed position and refreshes its Thread Safe Position for use in the Search facility.
        /// </summary>
        /// <returns>True if the item has not changed position since the last search iteration.</returns>
        public bool IsUnchangedAndUpdateThreadsafePosition()
        {
            // Store the old position used in last iteration
            Vector3 oldPos = threadSafePosition;

            // Gets the position from Transform
            threadSafePosition = GetPosition();

            // checks old position with the new position and returns true if they are the same.
            if (oldPos == threadSafePosition)
                return true;

            // Has changed
            return false;
        }

        /// <summary>
        /// True if this Effector is known not to move.
        /// </summary>
        public bool IsStatic() => isStatic;

        /// <summary>
        /// Gets the ID's of the FogItems that are revealed by this effector.
        /// </summary>
        public List<int> GetFogIdsCovered() => idsBeingCovered;

        /// <summary>
        /// Sets the ID's of the FogItems that are revealed by this effector.
        /// </summary>
        public List<int> SetFogIdsCovered(List<int> ids) => idsBeingCovered = ids;
    }
}