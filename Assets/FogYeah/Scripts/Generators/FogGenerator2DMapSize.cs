using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static FogSystems.FogSystemEnums;

namespace FogSystems
{
    /// <summary>
    /// 2D Map Size Fog Generator.
    /// This is used by the Map Size mode. Taking a grid sized value, density and overlap amount to create fog tiles. 
    /// </summary>
    internal class FogGenerator2DMapSize : FogGenerator
    {
        // Hold references to the validator and the Fog System instance.
        private FogSystem fogSysRef;
        private FogSettingsValidator fogSettingsValidator = new FogSettingsValidator();
        public FogGenerator2DMapSize(FogSystem fogSystem) => this.fogSysRef = fogSystem;

        public override void GenerateFog()
        {
            base.GenerateFog();

            // Counting the tiles as they are created for visual feedback.
            int counter = 0;

            // Ensure we don't have any, find any active scene objects that exist from previous runs.
            fogSysRef.utils.TryGetFogCanvas2D();

            // Validate the settings given
            if (!fogSettingsValidator.SettingsAreValidForFogCreation(fogSysRef._CommonSettings, fogSysRef._2DMapSizeSettings, fogSysRef._FogSystemAdvancedSettings))
                return;

            // Reset any orphan values.
            fogSysRef._FogSystemAdvancedSettings.FogCanvas = null;
            GameObject fogCanvas = null;
            List<Vector3> PositionsToSpawnAt = new List<Vector3>();
            float imageSize = 0;

            // Re-assign values for readability.
            float xVal = fogSysRef._2DMapSizeSettings.MapSize.x;
            float yVal = fogSysRef._2DMapSizeSettings.MapSize.y;
            float dens = fogSysRef._2DMapSizeSettings.Density;
            
            float totalNumImages = dens * xVal * yVal;
            float perRowNumImages = dens * xVal;
            float requiredSizeToFit = xVal / perRowNumImages;
            imageSize = requiredSizeToFit;
            
            List<Vector3> rtn = new List<Vector3>();

            // Build the grid.
            for (float rowIndex = 0; rowIndex < perRowNumImages; rowIndex++)
                for (float columnIndex = 0; columnIndex < dens * yVal; columnIndex++)
                    rtn.Add(
                        FogSystem.Instance._2DMapSpriteSettings.MapAxis == Axis.XZ ? 
                        new Vector3(rowIndex * imageSize, 0, columnIndex * imageSize) :
                        new Vector3(rowIndex * imageSize, columnIndex * imageSize, 0)
                    );

            PositionsToSpawnAt = rtn;

            // Give the image some extra size for overlap.
            imageSize *= fogSysRef._2DMapSizeSettings.OverlapAmountMultiplier;

            // Create the fog Canvas;
            fogCanvas = fogSysRef.CreateItem(fogSysRef._FogSystemAdvancedSettings.FogCanvasPrefab);

            if (fogSysRef._2DMapSizeSettings.MapAxis == Axis.XY)
            {
                Vector3 euler = fogCanvas.transform.eulerAngles;
                euler = Vector3.up;
                fogCanvas.transform.eulerAngles = euler;
            }

            // Assign cameras to the Fog Canvas
            fogCanvas.GetComponent<Canvas>().worldCamera = Camera.main;

            // If scattering is enabled in the settings, randomise positioning of the items in the vector3 array
            if (fogSysRef._2DMapSizeSettings.RandomScattering)
                PositionsToSpawnAt = PositionsToSpawnAt.OrderBy(x => System.Guid.NewGuid()).ToList();

            // Create the fog tiles.
            foreach (Vector3 pos in PositionsToSpawnAt)
            {
                // Create a tile
                GameObject go = fogSysRef.CreateItem(
                    fogSysRef._FogSystemAdvancedSettings.FogTilePrefab,
                               fogCanvas.transform.position + pos, // Space it out
                               fogCanvas.transform.rotation, // Copy rotation of the fog canvas
                               fogCanvas.transform // Parent
                    );

                // Randomise the Rotation of the new Fog GameObject
                if (fogSysRef._2DMapSizeSettings.RandomiseRotation)
                {
                    var euler = go.transform.eulerAngles;

                    euler.z = Random.Range(0f, 360f);

                    go.transform.eulerAngles = euler;
                }

                // Get the image of the spawned tile
                Image spawnedTileImage = go.GetComponent<Image>();

                // Set it to a sprite from the settings
                spawnedTileImage.sprite = fogSysRef.utils.GetRandomImageVariation(fogSysRef._CommonSettings.FogTileImageVariations);

                // Randomise the color between specified settings.
                spawnedTileImage.color = fogSysRef._CommonSettings.RandomiseColor ? Random.ColorHSV() : fogSysRef.utils.GetRandomColorVariation(fogSysRef._CommonSettings.FogTileColorVariations);

                // Ensure initial alpha of image is 1.
                var x = spawnedTileImage.color;
                x.a = Mathf.Clamp01(1);
                spawnedTileImage.color = x;
                

                // Resize to cover whole map
                spawnedTileImage.rectTransform.sizeDelta = new Vector2(imageSize, imageSize);

                // Counter is only used for debug output.
                counter++;
            }

            // Creates an example effector to demonstrate effector usage.
            if (fogSysRef.EffectorOptions.CreateExampleEffector)
                fogSysRef.utils.CreateAnExampleEffector();

            Debug.Log($"{counter} Fog tiles generated.");
        }

        public override void RemoveFog()
        {
            // Ensure the fog canvas is available
            fogSysRef.utils.TryGetFogCanvas2D();

            // Validation
            if (!fogSettingsValidator.SettingsAreValidForFogRemoval(fogSysRef._2DMapSizeSettings, fogSysRef._FogSystemAdvancedSettings)) return;

            // Remove the fog canvas and in turn will remove the created fog tiles.
            fogSysRef.DestroyItem(fogSysRef._FogSystemAdvancedSettings.FogCanvas);
        }

        public override void FinaliseFogRemoval()
        {
        }

        public override void CleanupArtifacts()
        {
        }
    }

}