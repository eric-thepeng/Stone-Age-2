using UnityEngine;
using static FogSystems.FogSystemEnums;

namespace FogSystems
{
    /// <summary>
    /// These are the settings that are used by the 2D Map Size mode.
    /// </summary>
    [System.Serializable]
    public class FogSystemMapSize2DSettings
    {
        [SerializeField]
        [Tooltip("The Axis to use when generating the Fog. Use XZ for 3d projects, and XY for 2d projects.")]
        internal Axis MapAxis = Axis.XZ;

        [SerializeField]
        [Tooltip("If you do not want to use an image to cover, and would prefer to use x:width and y:height use this field.")]
        internal Vector2 MapSize;

        [SerializeField]
        [Tooltip("Higher density gives more smooth transitions by incrasing the amount of images per grid square but can negativly effect performance.")]
        internal float Density = 3;

        [SerializeField]
        [Tooltip("Amount of overlap to apply to image sizes. This will be applied to image sizes to apply some overlap over density.")]
        internal float OverlapAmountMultiplier = 2f;

        [SerializeField]
        [Tooltip("Random scattering will ensure less visible tiling. You can disable this for a more fish-scale effect.")]
        internal bool RandomScattering = true;

        [SerializeField]
        [Tooltip("Rotating the images will display a much smoother edge - Disable for a more tiled/pixelated effect.")]
        internal bool RandomiseRotation = true;
    }
}