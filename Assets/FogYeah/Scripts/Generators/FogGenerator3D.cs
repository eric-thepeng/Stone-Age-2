using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FogSystems
{
    /// <summary>
    /// 3D Terrain fog generator. Takes a Terrain provided, and covers it with 3D fog prefabs.
    /// </summary>
    internal class FogGenerator3D : FogGenerator
    {
        // Hold references to the validator and the Fog System instance.
        private FogSystem fs;
        private FogSettingsValidator fogSettingsValidator = new FogSettingsValidator();
        public FogGenerator3D(FogSystem fogSystem) => this.fs = fogSystem;

        public override void GenerateFog()
        {
            base.GenerateFog();

            // Counting the tiles as they are created for visual feedback.
            int counter = 0;

            if (!fogSettingsValidator.SettingsAreValidForFogCreation(fs._3DSettings)) return;

            // Create a parent if its not created already.
            if (fs._3DSettings.FogTileParent == null)
                fs._3DSettings.FogTileParent = new GameObject(FogSystemConstants.FOG_3D_PARENT_DEFAULT_NAME).transform;

            // Set up the spacer size for the position map grid
            fs.utils.SetSpacerSize(fs._3DSettings.TerrainMapToCover.terrainData.detailWidth / fs._3DSettings.GridSize);

            // Get the positions to cover the map in fog tiles.
            List<Vector3> PositionsToSpawnAt = fs.utils.Get2DPositionMapCoverPositions(fs._3DSettings.GridSize);

            // If scattering is enabled in the settings, randomise positioning of the items in the vector3 array
            if (fs._3DSettings.RandomScattering)
                PositionsToSpawnAt = PositionsToSpawnAt.OrderBy(x => System.Guid.NewGuid()).ToList();


            foreach (Vector3 v3 in PositionsToSpawnAt)
            {
                counter++;
                fs.CreateItem(
                    fs._3DSettings.FogTilePrefab,
                    new Vector3(v3.x, fs._3DSettings.TerrainMapToCover.SampleHeight(v3), v3.z),
                    fs._3DSettings.RandomiseRotation ? Random.rotation : Quaternion.identity,
                    fs._3DSettings.FogTileParent
                );
            }

            // Creates an example effector to demonstrate effector usage.
            if (fs.EffectorOptions.CreateExampleEffector)
                fs.utils.CreateAnExampleEffector();

            Debug.Log($"{counter} Fog tiles generated.");
        }

        public override void RemoveFog()
        {
            base.RemoveFog();

            fs.utils.TryGetFogCanvas3D();

            // Validation
            if (!fogSettingsValidator.SettingsAreValidForFogRemoval(fs._3DSettings)) 
                return;

            fs.DestroyItem(fs._3DSettings.FogTileParent.gameObject);
        }

        public override void CleanupArtifacts() { }

        public override void FinaliseFogRemoval()
        {
            fs._3DSettings.FogTileParent = null;
            Debug.Log($"{FogSystemConstants.FOG_3D_PARENT_DEFAULT_NAME} has been removed.");
        }
    }
}