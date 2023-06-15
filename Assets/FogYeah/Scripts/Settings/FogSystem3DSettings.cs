using System.Collections.Generic;
using UnityEngine;
using static FogSystems.FogSystemEnums;

namespace FogSystems
{
    /// <summary>
    /// 3D Settings related to the generation, speeds and configuration for the fog system.
    /// </summary>
    [System.Serializable]
    public class FogSystem3DSettings
    {
        [SerializeField]
        [Tooltip("A gameobject to cover your Mesh/Terrain with as Fog.")]
        internal GameObject FogTilePrefab;

        [HideInInspector]
        [Tooltip("Fog parent that holds all Fog Tiles.")]
        internal Transform FogTileParent;

        [SerializeField]
        [Tooltip("Terrain object to cover in FogTiles")]
        internal Terrain TerrainMapToCover;

        [SerializeField]
        [Tooltip("Random scattering will ensure less visible tiling. You can disable this for a more fish-scale effect.")]
        internal bool RandomScattering = true;

        [SerializeField]
        [Tooltip("Rotating the images will display a much smoother edge - Disable for a more tiled/pixelated effect.")]
        internal bool RandomiseRotation = true;

        [HideInInspector]
        [Tooltip("Mesh object to cover in FogTiles - Coming soon!")]
        internal Mesh MeshToCover;

        [SerializeField]
        [Tooltip("Higher size gives more smooth transitions by reducing the size of each image but can negativly effect performance.")]
        internal int GridSize = 50;

        [Header("Appearance")]
        [SerializeField]
        [Tooltip("Enable this to randomise color from the list below.")]
        internal bool UseColorsFromList;

        [SerializeField]
        [Tooltip("Customise your tileset colors for the fog here.")]
        internal List<Color> FogTileColorVariations;

        [SerializeField]
        [Tooltip("Randomise the colors of the fog tiles.")]
        internal bool RandomiseColorHSV = false;
    }
}