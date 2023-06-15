using System.Collections.Generic;
using UnityEngine;

namespace FogSystems
{
    internal class FogSettingsValidator
    {
        /// <summary>
        /// Validates the 2D fog settings supplied are sufficient enough to generate the fog tiles for Map Size.
        /// </summary>
        /// <returns>If settings are valid, true, else false.</returns>
        internal bool SettingsAreValidForFogCreation(FogSystemCommonSettings _CommonSettings, FogSystemMapSize2DSettings settings, FogSystemAdvancedSettings advSettings) {
            List<string> Errors = new List<string>();

            if (advSettings.FogCanvas != null)
                Errors.Add($"A FogCanvas already exists in the scene. Please clear this using the button below before generating new fog. {nameof(advSettings.FogCanvas)}");

            if (advSettings.FogCanvas != null && advSettings.FogCanvas?.transform?.childCount > 0)
                Errors.Add($"{advSettings.FogCanvas?.transform.childCount} Fog tiles already exist in scene! Please delete the FogCanvas before creating more fog tiles.");
           
            if (settings.MapSize.x <= 0 || settings.MapSize.y <= 0)
                Errors.Add($"A value greater than 0 must be specified for both x and y axis for {nameof(settings.MapSize)}");

            if (settings.Density <= 0)
                Errors.Add($"Density must be a positive number {nameof(settings.Density)}");

            if (_CommonSettings.FogTileImageVariations?.Count == 0)
                Errors.Add($"You must specify at least one Fog Tile Image Variation {nameof(_CommonSettings.FogTileImageVariations)}");

            if (Errors.Count > 0)
            {
                Debug.Log($"{Errors.Count} errors were found whilst trying to generate the fog grid. Please review Errors in console.");

                foreach (string e in Errors)
                {
                    Debug.LogError(e);
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates the 2D fog settings supplied are sufficient enough to generate the fog tiles for Map Sprite.
        /// </summary>
        /// <returns>If settings are valid, true, else false.</returns>
        internal bool SettingsAreValidForFogCreation(FogSystemCommonSettings _CommonSettings, FogSystemMapSprite2DSettings settings, FogSystemAdvancedSettings advSettings)
        {
            List<string> Errors = new List<string>();

            if (advSettings.FogCanvas != null)
                Errors.Add($"A FogCanvas already exists in the scene. Please clear this using the button below before generating new fog. {nameof(advSettings.FogCanvas)}");

            if (advSettings.FogCanvas != null && advSettings.FogCanvas?.transform?.childCount > 0)
                Errors.Add($"{advSettings.FogCanvas?.transform.childCount} Fog tiles already exist in scene! Please delete the FogCanvas before creating more fog tiles.");

            if (settings.MapSprite == null)
                Errors.Add($"A value must be specified for {nameof(settings.MapSprite)}");

            if (settings.MapSprite != null && settings.MapSprite?.rect != null && settings.MapSprite?.rect.width == 0)
                Errors.Add($"A sprite must not have a width value of zero in {nameof(settings.MapSprite)}");

            if (settings.GridSize <= 0)
                Errors.Add($"Grid size must be a positive number {nameof(settings.GridSize)}");

            if (_CommonSettings.FogTileImageVariations?.Count == 0)
                Errors.Add($"You must specify at least one Fog Tile Image Variation {nameof(_CommonSettings.FogTileImageVariations)}");

            if (Errors.Count > 0)
            {
                Debug.Log($"{Errors.Count} errors were found whilst trying to generate the fog grid. Please review Errors in console.");

                foreach (string e in Errors)
                {
                    Debug.LogError(e);
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates the 3D fog settings supplied are sufficient enough to generate the fog tiles.
        /// </summary>
        /// <returns>If settings are valid, true, else false.</returns>
        internal bool SettingsAreValidForFogCreation(FogSystem3DSettings settings)
        {
            List<string> Errors = new List<string>();

            if (settings.FogTileParent != null && settings.FogTileParent?.transform?.childCount > 0)
                Errors.Add($"{settings.FogTileParent?.transform.childCount} Fog tiles already exist in scene! Please delete the Fog tile Parent before creating more fog tiles.");

            if (settings.TerrainMapToCover == null)
                Errors.Add($"A value must be specified for {nameof(settings.TerrainMapToCover)}");

            if (settings.FogTilePrefab == null)
                Errors.Add($"The Fog Tile Prefab has not been specified. Please provide a prefab to use to cover the map.");

            if (settings.FogTilePrefab?.GetComponent<Renderer>() == null)
                Errors.Add($"The Fog Tile Prefab specified does not have a renderer. Your fog tile must be a renderable mesh.");

            if (settings.GridSize <= 0)
                Errors.Add($"Grid size must be a positive number {nameof(settings.GridSize)}");

            if (settings.FogTileColorVariations.Count == 0 && settings.UseColorsFromList)
                Errors.Add($"You have selected {settings.UseColorsFromList} but have not provided any Color Variations. Please provide some Color Variations.");

            if (Errors.Count > 0)
            {
                Debug.Log($"{Errors.Count} errors were found whilst trying to generate the fog grid. Please review Errors in console.");

                foreach (string e in Errors)
                {
                    Debug.LogError(e);
                }
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Validates if fog tiles exist.
        /// </summary>
        /// <returns>If settings are valid, true, else false.</returns>
        internal bool SettingsAreValidForFogRemoval(FogSystemMapSize2DSettings settings, FogSystemAdvancedSettings advSettings)
        {
            List<string> Errors = new List<string>();

            if (advSettings.FogCanvas == null)
                Errors.Add($"No fog canvas found - No fog has been generated that can be cleared. You may manually delete the fog canvas in the scene if there are any.");

            if (advSettings.FogCanvas != null && advSettings.FogCanvas?.transform.childCount < 1)
                Errors.Add($"No fog tiles exist in the scene. This feature is for removing tiles if you have created them.");

            if (Errors.Count > 0)
            {
                Debug.Log($"{Errors.Count} errors were found whilst trying to remove fog tiles. Please review Errors in console.");

                foreach (string e in Errors)
                {
                    Debug.LogError(e);
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates if fog tiles exist.
        /// </summary>
        /// <returns>If settings are valid, true, else false.</returns>
        internal bool SettingsAreValidForFogRemoval(FogSystemMapSprite2DSettings settings, FogSystemAdvancedSettings advSettings)
        {
            List<string> Errors = new List<string>();

            if (advSettings.FogCanvas == null)
                Errors.Add($"No fog canvas found - No fog has been generated that can be cleared. You may manually delete the fog canvas in the scene if there are any.");

            if (advSettings.FogCanvas != null && advSettings.FogCanvas?.transform.childCount < 1)
                Errors.Add($"No fog tiles exist in the scene. This feature is for removing tiles if you have created them.");

            if (Errors.Count > 0)
            {
                Debug.Log($"{Errors.Count} errors were found whilst trying to remove fog tiles. Please review Errors in console.");

                foreach (string e in Errors)
                {
                    Debug.LogError(e);
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// 3D Variant of the settings validator for fog removal.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        internal bool SettingsAreValidForFogRemoval(FogSystem3DSettings settings)
        {
            List<string> Errors = new List<string>();

            if (settings.FogTileParent == null)
                Errors.Add($"No fog parent found - No fog has been generated that can be cleared. You may manually delete the fog tile parent in the scene if there are any.");

            if (settings.FogTileParent != null && settings.FogTileParent?.transform.childCount < 1)
                Errors.Add($"No fog tiles exist in the scene. This feature is for removing tiles if you have created them.");

            if (Errors.Count > 0)
            {
                Debug.Log($"{Errors.Count} errors were found whilst trying to remove fog tiles. Please review Errors in console.");

                foreach (string e in Errors)
                {
                    Debug.LogError(e);
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Ensures the map sprite has been injected into the scene, before trying to destroy it.
        /// </summary>
        /// <param name="mapSprite"></param>
        /// <returns></returns>
        internal bool SettingsAreValidForMapRemoval(GameObject mapSprite)
        {
            List<string> Errors = new List<string>();

            if (mapSprite == null)
                Errors.Add($"No Map Sprite has been found. You can delete manually if required.");

            if (Errors.Count > 0)
            {
                Debug.Log($"{Errors.Count} errors were found whilst trying to remove map sprite. Please review Errors in console.");

                foreach (string e in Errors)
                {
                    Debug.LogError(e);
                }
                return false;
            }

            return true;
        }
    }
}