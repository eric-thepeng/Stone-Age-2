using System;
namespace FogSystems
{
    public class FogSystemEnums
    {
        /// <summary>
        /// Will the tiles be in 3D or 2D space?
        /// </summary>
        public enum FogSystemMode
        {
            _2DMapSize,
            _2DMapSprite,
            _3D
        }

        /// <summary>
        /// States for the individual Fog Items
        /// </summary>
        public enum FogState : byte
        {
            Darkest, // Alpha = 1
            SemiTransparent, // Alpha = < 1 and > 0 (From FogSystemSettings > ShroudAmount)
            FullTransparent // Alpha = 0
        }

        /// <summary>
        /// Regrowth modes are as follows:
        /// Regrowing - Fog will reappear as zero opacity in the wake of the effectors moving through it.
        /// Shrouding - Fog will reappear as specified opacity in the wake of the effectors moving through it.
        /// None - Fog will not reappear after being revealed.
        /// </summary>
        internal enum FogRegrowthMode : byte
        {
            Regrowing,
            Shrouding,
            None
        }

        /// <summary>
        /// Use XZ for 3d Projects
        /// Use XY for 2d Projects
        /// </summary>
        internal enum Axis : byte
        {
            XZ,
            XY
        }

        /// <summary>
        /// Choose when to start the Fog Systems tracking feature at runtime.
        /// If you are using the Event Hooks, please choose 'Start' method.
        /// </summary>
        internal enum StartModes : byte
        {
            Awake,
            Start,
            Enable,
            Manual
        }
    }
}