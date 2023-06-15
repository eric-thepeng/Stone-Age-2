using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static FogSystems.FogSystemEnums;

namespace FogSystems
{
    /// <summary>
    /// 2D Map sprite fog generator. Takes a map image provided, and covers it with fog.
    /// </summary>
    internal class FogGenerator2DMapSprite : FogGenerator
    {
        // Hold references to the validator and the Fog System instance.
        private FogSystem fs;
        private FogSettingsValidator fogSettingsValidator = new FogSettingsValidator();
        public FogGenerator2DMapSprite(FogSystem fogSystem) => this.fs = fogSystem;

        public override void GenerateFog()
        {
            base.GenerateFog();

            // Counting the tiles as they are created for visual feedback.
            int counter = 0;

            // Ensure we don't have any, find any active scene objects that exist from previous runs.
            fs.utils.TryGetFogCanvas2D();

            // Validate the settings given
            if (!fogSettingsValidator.SettingsAreValidForFogCreation(fs._CommonSettings, fs._2DMapSpriteSettings, fs._FogSystemAdvancedSettings))
                return;

            // Reset any orphan values.
            fs._FogSystemAdvancedSettings.FogCanvas = null;
            GameObject fogCanvas = null;
            float fogPos = 0;
            List<Vector3> PositionsToSpawnAt = new List<Vector3>();
            float imageSize = 0;

            // Use first as sample for sizing.
            fs.utils.SetSpacerSize(fs._2DMapSpriteSettings.MapSprite.rect.width / fs._2DMapSpriteSettings.GridSize);

            // Ensure the size of the tile is sufficient to cover the map.
            imageSize = (fs._2DMapSpriteSettings.MapSprite.rect.width / fs._2DMapSpriteSettings.GridSize) * fs._2DMapSpriteSettings.Density;

            // Check for existing Map Canvas
            var mc = GameObject.FindObjectsOfType<Canvas>()?.FirstOrDefault(o => o != null && o.name.StartsWith(FogSystemConstants.FOG_2D_SPRITE_MAP_DEFAULT_NAME_PREFIX))?.gameObject;

            if (mc == null)
            {
                // Create the map canvas
                mc = fs.CreateItem(fs._FogSystemAdvancedSettings.MapCanvasPrefab);

                //Set its position
                mc.transform.position = Vector3.zero;

                var mi = mc.GetComponentInChildren<Image>();

                if (fs._2DMapSpriteSettings.MapAxis == Axis.XY)
                {
                    Vector3 euler = mi.transform.eulerAngles;
                    euler = Vector3.up;
                    mi.transform.eulerAngles = euler;
                }

                // Set the sprite to the correct size & sprite.
                mi.sprite = fs._2DMapSpriteSettings.MapSprite;
                mi.SetNativeSize();

                // Assign the camera. Can be changed after generation if nescessary.
                mc.GetComponent<Canvas>().worldCamera = Camera.main;

                // Create the fog Canvas;
                fogCanvas = fs.CreateItem(fs._FogSystemAdvancedSettings.FogCanvasPrefab, mc.transform.position, mc.transform.rotation, null);

                // Initial fog position is the Map Images width / 2.
                fogPos = -(fs._2DMapSpriteSettings.MapSprite.rect.width / 2);

                // Move the fog system generator to the corner of the map so the tiles can cover it.
                if (fs._2DMapSpriteSettings.MapAxis == Axis.XY)
                {
                    Vector3 euler = fogCanvas.transform.eulerAngles;
                    euler = Vector3.up;
                    fogCanvas.transform.eulerAngles = euler;

                    fogCanvas.transform.position = new Vector3(fogPos, fogPos, mc.transform.position.z); ;
                }
                else
                {
                    fogCanvas.transform.position = new Vector3(fogPos, mc.transform.position.y, fogPos);
                }
            }

            // Get the position map to place tiles on.
            PositionsToSpawnAt = fs.utils.Get2DPositionMapCoverPositions(fs._2DMapSpriteSettings.GridSize);

            // Assign cameras to the Fog Canvas
            fogCanvas.GetComponent<Canvas>().worldCamera = Camera.main;

            // If scattering is enabled in the settings, randomise positioning of the items in the vector3 array
            if (fs._2DMapSpriteSettings.RandomScattering)
                PositionsToSpawnAt = PositionsToSpawnAt.OrderBy(x => System.Guid.NewGuid()).ToList();

            // Create the fog tiles.
            foreach (Vector3 pos in PositionsToSpawnAt)
            {
                // Create a tile
                GameObject go = fs.CreateItem(
                    fs._FogSystemAdvancedSettings.FogTilePrefab,
                               fogCanvas.transform.position + pos, // Space it out
                               fogCanvas.transform.rotation, // Copy rotation of the fog canvas
                               fogCanvas.transform // Parent
                    );

                // Randomise the Rotation of the new Fog GameObject
                if (fs._2DMapSpriteSettings.RandomiseRotation)
                {
                    var euler = go.transform.eulerAngles;

                    euler.z = Random.Range(0f, 360f);

                    go.transform.eulerAngles = euler;
                }

                // Get the image of the spawned tile
                Image spawnedTileImage = go.GetComponent<Image>();

                // Set it to a sprite from the settings
                spawnedTileImage.sprite = fs.utils.GetRandomImageVariation(fs._CommonSettings.FogTileImageVariations);

                // Randomise the color between specified settings.
                spawnedTileImage.color = fs._CommonSettings.RandomiseColor ? Random.ColorHSV() : fs.utils.GetRandomColorVariation(fs._CommonSettings.FogTileColorVariations);

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
            if (fs.EffectorOptions.CreateExampleEffector)
                fs.utils.CreateAnExampleEffector();

            Debug.Log($"{counter} Fog tiles generated.");
        }

        public override void RemoveFog()
        {
            // Ensure the fog canvas is available
            fs.utils.TryGetFogCanvas2D();

            // Validation
            if (!fogSettingsValidator.SettingsAreValidForFogRemoval(fs._2DMapSpriteSettings, fs._FogSystemAdvancedSettings)) return;

            // Remove the fog canvas and in turn will remove the created fog tiles.
            fs.DestroyItem(fs._FogSystemAdvancedSettings.FogCanvas);
        }

        public override void CleanupArtifacts()
        {
            RemoveSpriteMap();
        }

        public override void FinaliseFogRemoval()
        {
            // Set the settings to null.
            fs._FogSystemAdvancedSettings.FogCanvas = null;
            Debug.Log($"{FogSystemConstants.FOG_2D_CANVAS_DEFAULT_NAME} has been removed.");
        }

        public void RemoveSpriteMap()
        {
            // Ensure the fog canvas is available
            GameObject instanceOfSpriteMap = fs.utils.TryGetSpriteMapCanvas2D();

            // Validation
            if (!fogSettingsValidator.SettingsAreValidForMapRemoval(instanceOfSpriteMap)) return;

            string instanceName = instanceOfSpriteMap.name;

            // Remove the fog canvas and in turn will remove the created fog tiles.
            fs.DestroyItem(instanceOfSpriteMap);

            Debug.Log($"SpriteMap {instanceName} has been removed.");
        }
    }
}