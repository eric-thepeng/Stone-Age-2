using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static FogSystems.FogSystemEnums;

namespace FogSystems
{
    /// <summary>
    /// This class performs the searching for the Fog System.
    /// </summary>
    public class FogSystemSearchHelper
    {
        // These lists are used in the search helpers to hold what fog tiles should be manipulated.
        private List<IFogItem> inRangeOfPlayers, noPlayersInRange;

        internal bool EffectorsChanged = true;

        /// <summary>
        /// Updates the positions of each FogSystemEffector to a thread safe Vector3 and returns True if a change / search is required. Or false if nothing has changed since the last search.
        /// </summary>
        internal bool CheckUpdateNescessaryAndUpdatePositionsToThreadSafe()
        {
            bool RequiresChange = false;

            // If a change has happened in the Effector Collection - trigger a search.
            if (EffectorsChanged)
            {
                EffectorsChanged = false;
                RequiresChange = true;
            }

            // Find all effectors that have been fully set up. The where clause here is to ensure we don't pick up an effector that is being added via editor.
            foreach (FogSystemEffector fse in FogSystem.Instance.Effectors.Where(o => !o.TransformIsNull()))
            {
                // If the effector has not been actioned before - trigger a search & update the Actioned flag within the effector.
                if (!fse.HasBeenActioned())
                {
                    fse.SetActioned(true);
                    RequiresChange = true;
                }

                // If the effector has changed position - update the thread safe position and trigger a search
                if (!fse.IsUnchangedAndUpdateThreadsafePosition())
                    RequiresChange = true;
            }

            return RequiresChange;
        }

        // Clears the lists for new searches.
        private void ResetListHolders()
        {
            inRangeOfPlayers = new List<IFogItem>();
            noPlayersInRange = new List<IFogItem>();
        }

        /// <summary>
        /// Searches all tiles given and sets them to active/inactive based on their proximity to effectors.
        /// </summary>
        /// <param name="FogTiles">FogTiles to search</param>
        internal void SearchAndSetActiveImmediate(IEnumerable<IFogItem> FogTiles)
        {
            ResetListHolders();

            // Foreach effector
            foreach (FogSystemEffector t in FogSystem.Instance.Effectors)
            {
                // if the effector is static and has been actioned, filter the tiles by the Id's already calculated.
                if (t.IsStatic() && t.HasBeenActioned())
                {
                    // If the static effector has not been actioned yet. 
                    if (t.GetFogIdsCovered() == null)
                    {
                        // Set the fog ids being covered.
                        t.SetFogIdsCovered(
                            FogTiles.Where(
                                o => Vector3.SqrMagnitude(t.GetThreadsafePosition() - o.GetPosition()) < t.GetSightRange())?
                                .Select(o => o.GetId())?
                                .ToList()
                            );

                        if (t.GetFogIdsCovered() != null)
                            // If fog tiles have been set, add them to the list of things to reveal.
                            inRangeOfPlayers.AddRange(FogTiles.Where(o => t.GetFogIdsCovered().Contains(o.GetId())));
                        else
                            // If the fog effector still has not been set - ensure it is not actioned again like this.
                            t.SetFogIdsCovered(new List<int>());
                    }
                    else
                        if(t.GetFogIdsCovered().Count > 0)
                            inRangeOfPlayers.AddRange(FogTiles.Where(o => t.GetFogIdsCovered().Contains(o.GetId())));
                }
                else
                    // Otherwise, we find all within sightrange of each effector and add them to the list.
                    inRangeOfPlayers.AddRange(FogTiles.Where(o => Vector3.SqrMagnitude(t.GetThreadsafePosition() - o.GetPosition()) < t.GetSightRange()));
            }

            // Everything found for each effector is then set inactive.
            foreach (IFogItem fi in inRangeOfPlayers.Where(o => o.GetState() != FogState.FullTransparent))
                fi.SetItemInactive();
        }

        /// <summary>
        /// This is only nescessary if a block of tiles should be updated instantly, instead of being triggered by the search system checker.
        /// An example is if the effectors list is empty.. The fog tiles should be still updated, even though the search system has no effectors.
        /// </summary>
        /// <param name="FogTiles">The tiles to update</param>
        /// <param name="stateToSet">The state to make the tiles</param>
        internal void ManualInstantUpdate(IEnumerable<IFogItem> FogTiles, FogState stateToSet)
        {
            foreach (IFogItem fi in FogTiles.Where(o => o.GetState() == FogState.FullTransparent))
                fi.SetState(stateToSet);
        }

        // This is used to prevent duplicate threads.
        private bool SearcherRunning = false;

        /// <summary>
        /// Searches tiles to see which ones need manipulation
        /// </summary>
        /// <param name="FogTiles">The tiles to search</param>
        internal async void SearchForFogTilesAsync(IEnumerable<IFogItem> FogTiles)
        {
            // Ensure a search isn't already running.
            if (!SearcherRunning)
            {
                SearcherRunning = true;

                ResetListHolders();

                // Creates a background thread to process the heavy lifting.
                await Task.Run(() =>
                {
                    // Check for players in range and add any that are inside sight range for each effector.
                    foreach (FogSystemEffector t in FogSystem.Instance.Effectors.ToList())
                        inRangeOfPlayers.AddRange(FogTiles.Where(o => Vector3.SqrMagnitude(t.GetThreadsafePosition() - o.GetPosition()) < t.GetSightRange()));

                    // Set the state of the fog items to FullTransparent, if they are within sight range.
                    FogSystem.Instance.ActionRunner.SetStateOfFogItems(inRangeOfPlayers.Where(o => o.GetState() != FogState.FullTransparent), FogState.FullTransparent);

                    // Get the fog tiles that require update outside of the sight range of all the effectors.
                    foreach (FogSystemEffector t in FogSystem.Instance.Effectors.ToList())
                        noPlayersInRange.AddRange(FogTiles.Where(o => o.GetState() == FogState.FullTransparent && !inRangeOfPlayers.Contains(o) && Vector3.SqrMagnitude(t.GetThreadsafePosition() - o.GetPosition()) > t.GetSightRange()));

                    // Set state of the fog tiles that are no longer in range of effectors.
                    FogSystem.Instance.ActionRunner.SetStateOfFogItems(noPlayersInRange, FogSystem.Instance._CommonSettings.RegrowthMode == FogRegrowthMode.Regrowing ? FogState.Darkest : FogState.SemiTransparent);
                });

                // Allow this method to be ran again next cycle.
                SearcherRunning = false;
            }
        }
    }
}