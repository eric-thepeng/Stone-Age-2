using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static FogSystems.FogSystemEnums;

namespace FogSystems
{
    /// <summary>
    /// Performs actions such as shrouding / setting inactivity & state changes to fog items in the system.
    /// </summary>
    public class FogSystemActionHelper
    {
        private const float MINIMUM_FADE_SPEED = 0.001f;

        internal bool TaskInProgress = false;
        
        /// <summary>
        /// Updates the states of the items given
        /// </summary>
        /// <param name="list">List of FogItems to change</param>
        /// <param name="state">State to update to</param>
        internal void SetStateOfFogItems(IEnumerable<IFogItem> list, FogState state)
        {
            foreach (IFogItem fi in list)
                // Only updates if the state is different
                if (fi.GetState() != state)
                {
                    fi.SetState(state);
                    FogSystem.Instance.EventHooks.InvokeOnStateChanged(list.Select(o => o.GetId()).ToArray(), state);
                }
        }

        /// <summary>
        /// Sets the item full transparent immediatly
        /// </summary>
        /// <param name="list">The items to set to full transparent</param>
        internal void SetImmediateFullTransparentFogItems(IEnumerable<IFogItem> list)
        {
            foreach (IFogItem fi in list)
                fi.SetItemInactive();
        }

        /// <summary>
        /// Fades the fog items given to the desired opacity set by the Search function.
        /// Uses the 'RequiresChange()' method of FogItem to determine what items are still requiring updates.
        /// </summary>
        /// <param name="allFogItems">All the fog items from the system</param>
        internal async void StartOpacityStepperAsync(IEnumerable<IFogItem> allFogItems)
        {
            // If the FadeSpeed is less than this value, it is not possible to fade reliably to the desired without causing too many operations per tick.
            if (FogSystem.Instance._CommonSettings.FadeSpeed < MINIMUM_FADE_SPEED)
            {
                Debug.LogError($"A fade speed of {FogSystem.Instance._CommonSettings.FadeSpeed} is not valid. Please enter a positive value above 0.001.");
                // Deactivate the FogSystem.
                FogSystem.Instance.FogSystemActive = false;
                return;
            }

            // Ensures all fog items are actioned before stopping the task and prevents the fog system from calling again until finished.
            TaskInProgress = true;

            while (TaskInProgress && Application.isPlaying)
            {
                // Refreshes the action list incase of new additions.
                IEnumerable<IFogItem> fogitemsRequiringchange = allFogItems.Where(o => o.RequiresChange());

                // Updates each fog item that requires change
                foreach (IFogItem fi in fogitemsRequiringchange)
                {
                    if (fi.RequiresChange())
                        // Changes opacity towards desired by this amount.
                        fi.StepColor(0.1f);
                }

                // Awaits a delay to stagger opacity and give fade effect.
                await Task.Delay(TimeSpan.FromSeconds(FogSystem.Instance._CommonSettings.FadeSpeed));

                if (!Application.isPlaying) return;

                // Only allows the while loop to exit if the fog items are all finished changing to their desired opacity.
                TaskInProgress = allFogItems.Any(o => o.RequiresChange());
            }
        }
    }
}