using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FogSystems.FogSystemEnums;

namespace FogSystems
{
    internal class FogSystemUtilities
    {
        /// Helper property to get the Spacer Size from the system.
        private float SpacerSize { get => FogSystem.Instance.GeneratorSharedProps.SpacerSize; }

        /// <summary>
        /// Sets the space between fog tiles for Vector3 map generator
        /// </summary>
        /// <param name="val">The value to set spacer to</param>
        internal void SetSpacerSize(float val) => FogSystem.Instance.GeneratorSharedProps.SpacerSize = val;

        /// <summary>
        /// Generate a map of Vector3's to cover the specified map in.
        /// </summary>
        /// <param name="GridSize">How many rows/columns are required.</param>
        /// <returns>A spaced grid of Vector3s that will cover the maps</returns>
        internal List<Vector3> Get2DPositionMapCoverPositions(int GridSize)
        {
            List<Vector3> rtn = new List<Vector3>();
            for (int rowIndex = 0; rowIndex < GridSize + 1; rowIndex++)
                for (int columnIndex = 0; columnIndex < GridSize + 1; columnIndex++)
                {
                    Vector3 pos;
                    if (FogSystem.Instance.CurrentMode == FogSystemMode._2DMapSprite)
                        // Generate the grid on the appropriate axis
                        pos = FogSystem.Instance._2DMapSpriteSettings.MapAxis == Axis.XZ
                            ? new Vector3(SpacerSize * rowIndex, 0, SpacerSize * columnIndex)
                            : new Vector3(SpacerSize * rowIndex, SpacerSize * columnIndex, 0);
                    else
                        pos = new Vector3(SpacerSize * rowIndex, 0, SpacerSize * columnIndex);

                    rtn.Add(pos);
                }

            return rtn;
        }

        /// <summary>
        /// Generate a map of Vector3's to cover the specified map in.
        /// </summary>
        /// <param name="GridSize">How many rows/columns are required.</param>
        /// <returns>A spaced grid of Vector3s that will cover the maps</returns>
        internal List<Vector3> Get2DPositionMapCoverPositionsByWidthAndHeight(float width, float height, float imageSize)
        {
            List<Vector3> rtn = new List<Vector3>();
            for (float rowIndex = 0; rowIndex < width; rowIndex += imageSize)
                for (float columnIndex = 0; columnIndex < height; columnIndex += imageSize)
                    rtn.Add(new Vector3(rowIndex, 0, columnIndex));

            return rtn;
        }

        /// <summary>
        /// Creates an example effector under the FogSystem gameobject. 
        /// Feel free to move this gameObject around at runtime in scene view so you can see the fog system reacting to it.
        /// </summary>
        internal void CreateAnExampleEffector()
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Example Effector";
            // Sets the example's parent to the fog system gameobject.
            go.transform.SetParent(FogSystem.Instance.transform);
            // Tags the example game object so that the fog system will automatically pick it up on awake.
            go.tag = "Player";
        }

        /// <summary>
        /// Gets the Fog Canvas containing the fog tiles and sets the value of settings.FogCanvas
        /// </summary>
        /// <returns>True if a canvas is found.</returns>
        internal bool TryGetFogCanvas2D()
        {
            // If FogCanvas is null - try to find it in scene by filtering all the canvases.
            if (FogSystem.Instance._FogSystemAdvancedSettings.FogCanvas == null)
            {
                // Get the first canvas with the name 'FogTileCanvas'
                FogSystem.Instance._FogSystemAdvancedSettings.FogCanvas =
                    GameObject.FindObjectsOfType<Canvas>()?
                    .FirstOrDefault(o => o != null && o.name.StartsWith(FogSystemConstants.FOG_2D_CANVAS_DEFAULT_NAME))?
                    .gameObject;

                // Backwards compatability:
                if (FogSystem.Instance._FogSystemAdvancedSettings.FogCanvas == null)
                    FogSystem.Instance._FogSystemAdvancedSettings.FogCanvas =
                        GameObject.FindObjectsOfType<Canvas>()?
                        .FirstOrDefault(o => o != null && o.name.StartsWith(FogSystemConstants.FOG_2D_CANVAS_PREV_NAME))?
                        .gameObject;

                // If still null, ask user to assign manually. They may have changed the name of the GameObject.
                if (FogSystem.Instance._FogSystemAdvancedSettings.FogCanvas == null)
                    return false;
            }

            // Above checks passed, the Fog Canvas has been set.
            return true;
        }

        /// <summary>
        /// Gets the transform containing the fog tiles and sets the value of settings.FogTileParent
        /// </summary>
        /// <returns>True if a canvas is found.</returns>
        internal bool TryGetFogCanvas3D()
        {
            // If FogTileParent is null - try to find it in scene by filtering all the canvases.
            if (FogSystem.Instance._3DSettings.FogTileParent == null)
            {
                // Get the first canvas with the name 'FogTileParent'
                FogSystem.Instance._3DSettings.FogTileParent =
                    GameObject.FindObjectsOfType<GameObject>()?.FirstOrDefault(o => o != null && o.name.StartsWith(FogSystemConstants.FOG_3D_PARENT_DEFAULT_NAME))?.transform;

                // If still null, ask user to assign manually. They may have changed the name of the GameObject.
                if (FogSystem.Instance._3DSettings.FogTileParent == null)
                    return false;
            }

            // All dependancies have been set.
            return true;
        }

        /// <summary>
        /// Attempts to get the Sprite map by default name. 
        /// </summary>
        /// <returns>The sprite map instance in the scene.</returns>
        internal GameObject TryGetSpriteMapCanvas2D()
        {
            return 
                GameObject.FindObjectsOfType<GameObject>()?
                .FirstOrDefault(o => o != null && o.name.StartsWith(FogSystemConstants.FOG_2D_SPRITE_MAP_DEFAULT_NAME_PREFIX))?
                .gameObject;
        }

        /// <summary>
        /// Grabs a random image from the provided sprites
        /// </summary>
        /// <returns>Random image from sprites provided in settings</returns>
        internal Sprite GetRandomImageVariation(List<Sprite> list) => list[UnityEngine.Random.Range(0, list.Count)];

        /// <summary>
        /// Grabs a random color from the provided colors
        /// </summary>
        /// <returns>Random color from colors provided in settings</returns>
        internal Color GetRandomColorVariation(List<Color> list) => list.Count > 0 ? list[UnityEngine.Random.Range(0, list.Count)] : Color.black;
    }
}