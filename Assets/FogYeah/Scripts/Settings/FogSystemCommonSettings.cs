using System.Collections.Generic;
using UnityEngine;
using static FogSystems.FogSystemEnums;

namespace FogSystems
{
    /// <summary>
    /// Common properties for fog system runtime and generators are held here. 
    /// </summary>
    [System.Serializable]
    public class FogSystemCommonSettings
    {
        [SerializeField]
        [Header("Runtime Initialisation")]
        [Tooltip("Choose when the Fog System should begin at runtime. If you wish to use Manual, you can activate the Fog System through code by calling FogSystem.Instance.Initialise();")]
        internal StartModes StartMode = StartModes.Awake;

        [SerializeField]
        [Header("Runtime Activity")]
        [Tooltip("Regrowth is how the fog that is left behind reacts. Does it regrow or go Transparent(shrouding).")]
        internal FogRegrowthMode RegrowthMode = FogRegrowthMode.Shrouding;

        [SerializeField]
        [Tooltip("The alpha to leave 'shrouding' tiles once the effectors pass through them.")]
        internal float ShroudAmount = 0.2f;

        [SerializeField]
        [Tooltip("The opacity increment between each fade iteration.")]
        internal float FadeSpeed = 0.05f;

        [SerializeField]
        [Tooltip("How much delay between fade iterations & checks for changes is there? (In seconds)")]
        internal float RefreshDelay = 0.1f;

        [Header("Generation Appearance")]
        [SerializeField]
        [Tooltip("Customise your tileset colors for the fog here.")]
        internal List<Color> FogTileColorVariations;

        [SerializeField]
        [Tooltip("Customise your tileset sprites for the fog here.")]
        internal List<Sprite> FogTileImageVariations;

        [SerializeField]
        [Tooltip("Randomise the colors of the fog tiles.")]
        internal bool RandomiseColor = false;
    }
}