using System;
using UnityEngine;
using UnityEngine.UI;
using static FogSystems.FogSystemEnums;

namespace FogSystems
{
    /// <summary>
    /// Each fog tile must be created as a FogItem in order to be used by the FogSystem
    /// </summary>
    [System.Serializable]
    internal class FogItem3D : IFogItem
    {
        // Unique Id for each fog item.
        internal int Id;

        // Cache for the GameObject - Used when setting the object active.
        private GameObject Item;

        // Cache for the renderer to apply changes to.
        internal MeshRenderer renderer;

        // Current state of the fog item.
        private FogState State = FogState.Darkest;

        // Position cache of the fog
        private Vector3 pos;

        // Cache for the initial Material that the renderer loads with.
        private Material initialMaterial;

        // Create an instanced material that the system can change without effecting origonal
        private Material materialInstance;

        // Cache all materials that will be used
        private Material matTmplTransparent,
                    matTmplHalfAlpha,
                    matTmplFullVis;

        internal FogItem3D(int id, GameObject item)
        {
            // Assigned from parameter, this will always be unique.
            Id = id;
            // Cache the gameobject
            Item = item;

            // Cache the position of the gameobject
            pos = item.transform.position;

            // Cache the image component
            renderer = item.GetComponent<MeshRenderer>();

            // Set the Opacity of the image only if one was found.
            if (renderer != null)
            {
                initialMaterial = renderer.material;

                // Create a material instance to use instead of the origonal. 
                materialInstance = new Material(renderer.material);
                renderer.material = materialInstance;

                // Randomise the colors of each Fog Tile based off the colors provided
                if (FogSystem.Instance._3DSettings.UseColorsFromList && FogSystem.Instance._3DSettings.FogTileColorVariations.Count > 0)
                    renderer.material.color = FogSystem.Instance.utils.GetRandomColorVariation(FogSystem.Instance._3DSettings.FogTileColorVariations);

                // Randomise the color of the Fog Tiles
                if (FogSystem.Instance._3DSettings.RandomiseColorHSV)
                    renderer.material.color = UnityEngine.Random.ColorHSV();

                // Cache templates for Alpha on materials.
                matTmplTransparent = InstanceNewMaterialForAlpha(0);
                matTmplHalfAlpha = InstanceNewMaterialForAlpha(FogSystem.Instance._CommonSettings.ShroudAmount);
                matTmplFullVis = InstanceNewMaterialForAlpha(1);

                Color c = renderer.material.color;
                // Initially all images should be at opacity 1.
                c.a = 1;
                renderer.material.color = c;
            }
        }

        /// <summary>
        /// Sets up materials for lerping tolerance later.
        /// </summary>
        /// <param name="mat">The material to set</param>
        /// <param name="alpha">The desired transparency for the material</param>
        Material InstanceNewMaterialForAlpha(float alpha)
        {
            Material mat = new Material(renderer.material);
            var col = mat.color;
            col.a = alpha;
            mat.color = col;
            return mat;
        }

        /// <summary>
        /// Gets the unique identifier for this fog item.
        /// </summary>
        /// <returns>The unique identifier of this fog item</returns>
        public int GetId() => Id;

        /// <summary>
        /// Gets the current state of the fog item
        /// </summary>
        /// <returns>The current state</returns>
        public FogState GetState() => State;

        /// <summary>
        /// Gets the cached location of the fog item
        /// </summary>
        /// <returns>The location of the fog item</returns>
        public Vector3 GetPosition() => pos;

        /// <summary>
        /// Deactivates and sets the state for shrouding of type 'None'
        /// </summary>
        public void SetItemInactive()
        {
            State = FogState.FullTransparent;
            Item.SetActive(false);
        }

        /// <summary>
        /// Activates and sets the state for shrouding of type 'None'
        /// </summary>
        public void SetItemActive()
        {
            State = FogState.Darkest;
            Item.SetActive(true);
        }

        /// <summary>
        /// Changes the state of the fog item
        /// </summary>
        /// <param name="state">The desired state to change to</param>
        public void SetState(FogState state)
        {
            // If no state change is required, return.
            if (State == state) return;

            // Updates the state to the new state given.
            State = state;
        }

        /// <summary>
        /// Steps the colors opacity towards its desired opacity.
        /// This is called by the actioner every 'step' defined by fade speed in settings.
        /// If the desired opacity is within a reasonable range, it will force the desired opacity to its current amount.
        /// This method works alongside the 'Requires change' to action each Fog Item
        /// </summary>
        /// <param name="val">The increment of alpha to apply.</param>
        public void StepColor(float val)
        {            
            if (renderer == null) return;

            // Set the color to the correct color of the state 
            renderer.material.color = GetColorForState(State);

            // Get rid of fully transparent tiles in order to remove the shadow effect from a 0 alpha mesh renderer. 
            if (Item.activeInHierarchy && renderer.material.color.a == 0 && State == FogState.FullTransparent) Item.SetActive(false);
            // Enable any that have been previously disabled, but now require re-activation.
            else if (!Item.activeInHierarchy && renderer.material.color.a > 0) Item.SetActive(true);

            return;
        }

        /// <summary>
        /// Override this if you want your fog to act differently.
        /// </summary>
        /// <param name="st">State color required</param>
        /// <returns></returns>
        internal virtual Color GetColorForState(FogState st)
        {
            switch(st)
            {
                case FogState.FullTransparent:
                    return matTmplTransparent.color;
                case FogState.Darkest:
                    return matTmplFullVis.color;
                case FogState.SemiTransparent:
                    return matTmplHalfAlpha.color;
                default:
                    return Color.black;
            }
        }

        /// <summary>
        /// Is there a change needed to bring the LastKnownOpacity to the DesiredOpacity?
        /// </summary>
        public bool RequiresChange() => renderer.material.color != GetColorForState(State);

        /// <summary>
        /// Safety net for any thread cleanup. 
        /// </summary>
        public void OnBeforeApplicationExit()
        {
            renderer.material = initialMaterial;
            renderer = null;
        }

        /// <summary>
        /// Helper function to test if a value specified is between two values.
        /// </summary>
        /// <param name="testValue">The value to test</param>
        /// <param name="bound1">Boundry one</param>
        /// <param name="bound2">Boundry two</param>
        /// <returns>True if the test value appears within the boundry values</returns>
        public bool IsBetween(float testValue, float bound1, float bound2) => (testValue >= Math.Min(bound1, bound2) && testValue <= Math.Max(bound1, bound2));
    }
}