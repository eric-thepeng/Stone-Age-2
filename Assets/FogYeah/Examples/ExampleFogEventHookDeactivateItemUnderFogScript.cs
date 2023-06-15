using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static FogSystems.FogSystemEnums;

namespace FogSystems
{
    public class ExampleFogEventHookDeactivateItemUnderFogScript : MonoBehaviour
    {
        /// <summary>
        /// Mode by witch to collect objects for the Cache
        /// </summary>
        private enum SearchMode { ByLayer, ByTag }

        /// <summary>
        /// Hide the GameObject itself, by SetActive? or just disable the renderer.
        /// </summary>
        private enum HideModes { GameObject, Renderer }

        /// <summary>
        /// What range to collect items for the Cache
        /// </summary>
        private const int RANGE_TO_CACHE = 100;

        /// <summary>
        /// This dictionary holds a collection of renderers to enable / disable. 
        /// You can also change this to GameObject if you want to just enable and disable, without doing any GetComponents later on.
        /// </summary>
        private readonly Dictionary<int, List<Renderer>> objectCollectionMap = new Dictionary<int, List<Renderer>>();

        [SerializeField, Tooltip("Example of how to disable objects vs disable just their renderers.")]
        private HideModes HideMode = HideModes.Renderer;

        [SerializeField, Tooltip("Collect objects based off their Layers or their Tag.")]
        private SearchMode searchMode;

        [SerializeField, Tooltip("What layers are the items you want to show/hide on? (if Search Mode is Layer)")]
        private LayerMask layersToSearch;

        [SerializeField, Tooltip("What tag does the items you want to show/hide have? (if Search Mode is Tag)")]
        private string TagToSearch;

        /// <summary>
        ///  Class to hold all objects that are collected.
        /// </summary>
        private class Actionable
        {
            // Corresponding FogItem ID
            internal int Id = -1;
            // Has this been actioned this cycle?
            internal bool Actioned = false;
            // Do we want to show/hide this item
            internal bool SetActiveFlag = false;
        }

        // Holds all items to show or hide this cycle.
        private List<Actionable> actionables = new List<Actionable>();

        // How often do we want the checker to run?
        private const float DELAY_TIMER_INCREMENT = 0.5f;

        // Timer for delaying the update method.
        private float timeToTakeAction = 0;

        private void Awake()
        {
            // Hook the event for State Changes.
            FogSystem.Instance.EventHooks.OnStateChangedForFogItemIDs += ActionStateChangesToFogSystem;
            FogSystem.Instance.EventHooks.OnFogSystemHasStarted += StartTrackingItems;
        }

        void StartTrackingItems()
        {
            // Validate that the tag chosen exists.
            if(searchMode == SearchMode.ByTag && GameObject.FindGameObjectsWithTag(TagToSearch).Count() == 0)
            {
                Debug.Log("That tag does not exist.");
                return;
            }

            // Collect all fog tiles
            foreach (IFogItem tile in FogSystem.Instance.GetFogTiles())
            {
                // Collect all objects near this tile.
                var objectsAroundThisTile = CollectObjectsAround(tile);

                // If there are objects around this tile, 
                if (objectsAroundThisTile?.Count > 0)
                    // Add the collected objects to the cache, with the corresponding Fog ID
                    objectCollectionMap.Add(tile.GetId(), objectsAroundThisTile);
            }

            // Useful debug feature; uncomment this statement if you want to see which items are being collected in the cache.
            // foreach (KeyValuePair<int, List<Renderer>> kvp in objectCollectionMap)
            //     // Change the color of anything found in the cache to yellow
            //     foreach (Renderer go in kvp.Value)
            //         ChangeColorOfGo(go);

            // Set initial state of the items in the collection to invisible
            foreach (KeyValuePair<int, List<Renderer>> kvp in objectCollectionMap)
            {
                foreach (Renderer r in kvp.Value)
                    SetVisibility(r, false);
            }

            Debug.Log($"Object Collection Map has amount: {objectCollectionMap.Count}");
        }

        /// <summary>
        /// Sets visiblility of the item given
        /// </summary>
        /// <param name="r">Renderer of the item</param>
        /// <param name="active">Show or hide</param>
        private void SetVisibility(Renderer r, bool active)
        {
            switch (HideMode)
            {
                // Enable / disable the Renderer
                case HideModes.Renderer:
                    r.enabled = active;
                    break;
                // Enable/disable the GameObject
                case HideModes.GameObject:
                    r.gameObject.SetActive(active);
                    break;
            }
        }

        /// <summary>
        /// Timer to slow down so that we're not doing the search every frame of the application
        /// </summary>
        /// <returns>True if the desired time has passed.</returns>
        private bool TimeToTakeAction() => Time.time > timeToTakeAction;

        /// <summary>
        /// Increments the timer to a time in the future, by a value from constant: DELAY_TIMER_INCREMENT
        /// </summary>
        private void ResetTimeToTakeAction() => timeToTakeAction = Time.time + DELAY_TIMER_INCREMENT;

        private void LateUpdate()
        {
            // Has time passed enough since last cycle?
            if (!TimeToTakeAction()) return;

            // Ensure we are not doing this every frame
            ResetTimeToTakeAction();

            /// If any actionables exist that have not yet been actioned, action them now and remove them
            if (actionables.Any(o => !o.Actioned))
            {
                // Iterate through the actionables found
                foreach (Actionable i in actionables)
                {
                    // If any ids that need action exist in the collection map
                    if (objectCollectionMap.ContainsKey(i.Id))
                        // Foreach renderer in this cached item
                        foreach (Renderer go in objectCollectionMap[i.Id])
                        {
                            // Set the visibility
                            SetVisibility(go, i.SetActiveFlag);
                            // Set it to not be actioned again.
                            i.Actioned = true;
                        }
                }

                // Remove any actioned items
                actionables.RemoveAll(o => o.Actioned);
            }
        }

        /// <summary>
        /// Event hook that reads the id's changed by the fog system and creates map of the effected items.
        /// </summary>
        /// <param name="ids">ID's of the Fog Items changing state</param>
        /// <param name="state">The state that the items have changed to</param>
        private void ActionStateChangesToFogSystem(int[] ids, FogState state)
        {
            // For each changed tile
            foreach (int i in ids)
                // Add an actionable item to the array to ensure they reflect the change to the fog changes
                actionables.Add(
                    new Actionable()
                    {
                        Id = i,
                        // If the fog item is now darkest, then we should set the actionable to hide the object, else - we should set it to visible.
                        SetActiveFlag = !(state == FogState.Darkest || state == FogState.SemiTransparent)
                    }
                );
        }

        /// <summary>
        /// Find any gameobjects based off the settings criteria given that are within a range of the Fog Item.
        /// </summary>
        /// <param name="fogItem">The fog item to search nearby</param>
        /// <returns>List of all renderers near the given FogItem Tile</returns>
        private List<Renderer> CollectObjectsAround(IFogItem fogItem)
        {
            List<Renderer> gameObjectsToSearchThrough = new List<Renderer>();
            switch (searchMode)
            {
                case SearchMode.ByLayer:
                    // If using By Layer, Grab all renderers
                    gameObjectsToSearchThrough = GameObject.FindObjectsOfType<Renderer>().ToList();
                    // Filter out all that are not on the layers given
                    gameObjectsToSearchThrough.RemoveAll(o => !MaskContains(layersToSearch, o.gameObject.layer));
                    break;

                case SearchMode.ByTag:
                    // If using a tag, Get All Objects in that tag, and select their Renderers
                    gameObjectsToSearchThrough = GameObject.FindGameObjectsWithTag(TagToSearch).Select(
                        o => o.GetComponent<Renderer>()
                    ).ToList();
                    break;
            }

            // Ensure the distance between the fog item and the items are within the range to search
            gameObjectsToSearchThrough.RemoveAll(o => Vector3.Distance(o.transform.position, fogItem.GetPosition()) > RANGE_TO_CACHE);
            return gameObjectsToSearchThrough;
        }

        /// <summary>
        /// Useful for debugging, this will change the renderer color to Yellow
        /// </summary>
        /// <param name="rend">The Renderer to change</param>
        private void ChangeColorOfGo(Renderer rend) => rend.material.color = Color.yellow;

        /// <summary>
        /// Checks that a given mask is within the layers provided
        /// </summary>
        /// <param name="mask">The mask to check</param>
        /// <param name="layer">The layers to check</param>
        /// <returns>True if the mask is within the layer</returns>
        private bool MaskContains(LayerMask mask, int layer) => mask == (mask | (1 << layer));
    }
}