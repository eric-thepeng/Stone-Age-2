using System;
namespace FogSystems
{
    public class FogSystemConstants
    {
        /// <summary>
        /// Default sight range is the range in which the initial effectors will be created, if none are specified and the setting 'AutoCreateEffectorFromPlayers' is true.
        /// </summary>
        internal const float EFFECTOR_DEFAULT_SIGHT_RANGE = 500;

        /// <summary>
        /// The 2D Canvas generation will create a canvas with this name in your scene.
        /// It will also be used on awake to find the Image tiles within.
        /// </summary>
        internal const string FOG_2D_CANVAS_DEFAULT_NAME = "FogImageTileCanvas";

        /// <summary>
        /// For backwards compatibility: PREV_NAME will be checked if DEFAULT_NAME is empty. 
        /// This is to ensure existing scenes will not error at runtime.
        /// </summary>
        internal const string FOG_2D_CANVAS_PREV_NAME = "FogTileCanvas";

        /// <summary>
        /// The 3D Parent transform name of the Tile Holder created to hold the 3D Fog Tiles
        /// It will also be used on awake to find the Fog Tile Prefab clone tiles within.
        /// </summary>
        internal const string FOG_3D_PARENT_DEFAULT_NAME = "FogTileParent";

        /// <summary>
        /// The 2D Sprite Map will be created using this prefix.
        /// </summary>
        internal const string FOG_2D_SPRITE_MAP_DEFAULT_NAME_PREFIX = "MapCanvas";
    }
}