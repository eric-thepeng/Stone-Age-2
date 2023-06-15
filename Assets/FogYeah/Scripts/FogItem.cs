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
    internal class FogItem : IFogItem
    {
        // Unique Id for each fog item.
        internal int Id;

        // Cache for the GameObject - Used when setting the object active.
        private GameObject Item;

        // Cache for the image to apply changes to.
        internal Image img;

        // Current state of the fog item.
        private FogState State = FogState.Darkest;

        // Position cache of the fog
        private Vector3 pos;

        // Set by state changes, this is the opacity amount the actioner will step towards.
        private float DesiredOpacityAmount;

        // Cache for the last opacity known so that we don't have to open the images alpha value each time.
        internal float LastKnownOpacity;

        // Cleanup for threads
        internal bool ApplicationIsExiting = false;

        internal FogItem(int id, GameObject item)
        {
            // Assigned from parameter, this will always be unique.
            Id = id;
            // Cache the gameobject
            Item = item;

            // Cache the position of the gameobject
            pos = item.transform.position;

            // Cache the image component
            img = item.GetComponent<Image>();

            // Set the Opacity of the image only if one was found.
            if (img != null)
            {
                Color c = img.color;
                // Initially all images should be at opacity 1.
                c.a = 1;
                img.color = c;

                // Set the initial values for the Actioner.
                LastKnownOpacity = 1;
                DesiredOpacityAmount = 1;
            }
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

            // Sets the Desired Opacity amount for each of the states specified.
            switch (state)
            {
                case FogState.Darkest:
                    DesiredOpacityAmount = 1;
                    break;

                case FogState.FullTransparent:
                    DesiredOpacityAmount = 0;
                    break;

                case FogState.SemiTransparent:
                    DesiredOpacityAmount = FogSystem.Instance._CommonSettings.ShroudAmount;
                    break;
            }

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
            // If no image is cached, do nothing.
            if (img == null) return;

            // Get the new opacity amount and store in the LastKnownOpacity variable.
            LastKnownOpacity = DesiredOpacityAmount > LastKnownOpacity ? LastKnownOpacity + val : LastKnownOpacity = LastKnownOpacity - val;

            // If the desired state is FullTransparent and the LastKnown is about to be less than zero
            if (State == FogState.FullTransparent && LastKnownOpacity <= 0)
            {
                // Set the Last Known to zero to avoid large floating point numbers being used next time.
                LastKnownOpacity = 0;
                // Set the desired opacity to zero also, to avoid awkward numbers being used next time.
                DesiredOpacityAmount = LastKnownOpacity;
            }
            else if (State != FogState.FullTransparent)
                // If the state is not full transparent - ensure the amount of opacity required is minimal enough to stop updating it.
                if (IsBetween(LastKnownOpacity, DesiredOpacityAmount - 0.1f, DesiredOpacityAmount + 0.1f))
                    DesiredOpacityAmount = LastKnownOpacity;

            // Round the last known opacity to one floating point number.
            LastKnownOpacity = (float)Math.Round(LastKnownOpacity, 1);

            // Update the alpha of the color of the image.
            Color updatedColor = img.color;
            updatedColor.a = LastKnownOpacity;
            img.color = updatedColor;
        }

        /// <summary>
        /// Is there a change needed to bring the LastKnownOpacity to the DesiredOpacity?
        /// </summary>
        /// <returns>True if the LastKnownOpacity does not match the DesiredOpacityAmount</returns>
        public bool RequiresChange() => LastKnownOpacity != DesiredOpacityAmount;

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