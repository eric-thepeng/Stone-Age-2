using System.Collections.Generic;
using UnityEngine;
using static FogSystems.FogSystemEnums;

namespace FogSystems
{
    /// <summary>
    /// 2D Settings related to the generation, speeds and configuration for the fog system.
    /// </summary>
    [System.Serializable]
    public class FogSystemMapSprite2DSettings
    {
        [SerializeField]
        [Tooltip("The Axis to use when generating the Fog. Use XZ for 3d projects, and XY for 2d projects.")]
        internal Axis MapAxis = Axis.XZ;

        [SerializeField]
        [Tooltip("The sprite of the image that you want to cover.")]
        internal Sprite MapSprite;

        [Tooltip("Higher size gives more smooth transitions by reducing the size of each image but can negativly effect performance.")]
        internal int GridSize = 50;

        [Tooltip("Higher density gives more smooth transitions by incrasing the amount of overlap but can negativly effect performance.")]
        internal float Density = 3;

        [SerializeField]
        [Tooltip("Random scattering will ensure less visible tiling. You can disable this for a more fish-scale effect.")]
        internal bool RandomScattering = true;

        [SerializeField]
        [Tooltip("Rotating the images will display a much smoother edge - Disable for a more tiled/pixelated effect.")]
        internal bool RandomiseRotation = true;
    }
}