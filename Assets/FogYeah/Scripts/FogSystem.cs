using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FogSystems.FogSystemEnums;
using static FogSystems.FogSystemConstants;

namespace FogSystems
{
    /// <summary>
    /// Core Fog System class.
    /// Contains references to all included classes and properties.
    /// Usage: Use the singleton instance FogSystem.Instance to access public properties.
    /// [e.g.] FogSystem.Instance.AddEffector(transform, 100f);
    /// </summary>
    public class FogSystem : MonoBehaviour
    {
        #region Public / Serializable
        /// <summary>
        /// Field to serialise a dropdown for switching between Fog Modes
        /// </summary>
        [SerializeField]
        public FogSystemMode CurrentMode;

        /// <summary>
        /// Singleton for handiness
        /// </summary>
        public static FogSystem Instance;
        
        /// <summary>
        /// Initialises the Singleton Instance. Moved from awake so that we can call it from Editor Script. 
        /// </summary>
        void InitialiseSingleton()
        {
            if(Instance == null)
                Instance = this;
        }

        #region Fog System Settings
        /// <summary>
        /// Common Fog System Settings.
        /// </summary>
        public FogSystemCommonSettings _CommonSettings;

        /// <summary>
        /// 2D Map Size Settings.
        /// </summary>
        public FogSystemMapSize2DSettings _2DMapSizeSettings;

        /// <summary>
        /// 2D Map Sprite Settings.
        /// </summary>
        public FogSystemMapSprite2DSettings _2DMapSpriteSettings;

        /// <summary>
        /// 3D Terrain Settings.
        /// </summary>
        public FogSystem3DSettings _3DSettings;
        
        /// <summary>
        /// Advanced Settings.
        /// </summary>
        public FogSystemAdvancedSettings _FogSystemAdvancedSettings;
        #endregion

        /// <summary>
        /// Properties shared between all Generators.
        /// </summary>
        internal FogSystemGeneratorSharedProperties GeneratorSharedProps = new FogSystemGeneratorSharedProperties();
        
        /// <summary>
        /// Utilities for use by the generators.
        /// </summary>
        internal FogSystemUtilities utils = new FogSystemUtilities();
        
        /// <summary>
        /// This is a holder for default settings.
        /// Effectors used by the fog system are placed into the Effectors list. 
        /// </summary>
        public FogSystemEffectorSettings EffectorOptions;

        /// <summary>
        /// Any units / players that should effect the fog.
        /// </summary>
        [SerializeField]
        [Tooltip("Effectors are players, or units that can effect the Fog. Fog System will assign all 'Player' tagged items as effectors if not specified. To add to this list, use the FogSystem.Instance.AddEffector")]
        public List<FogSystemEffector> Effectors = new List<FogSystemEffector>();

        #endregion Public / Serialisable

        #region Private Variables
        /// <summary>
        /// Current Generator is refreshed when the Mode is changed. 
        /// </summary>
        private FogGenerator generator;

        /// <summary>
        /// Tiles of fog that were generated
        /// </summary>
        private List<IFogItem> FogTiles = new List<IFogItem>();

        /// <summary>
        /// Next time to update tiles
        /// </summary>
        private float NextUpdateDue = 0;
        #endregion Private Variables

        #region internal variables

        /// <summary>
        /// Performs searching + distance checks on the fog tiles
        /// </summary>
        internal FogSystemSearchHelper Searcher;

        /// <summary>
        /// Runs actions on tiles that need state changes
        /// </summary>
        internal FogSystemActionHelper ActionRunner;

        /// <summary>
        /// Used to cancel running or to deactivate the system.
        /// </summary>
        internal bool FogSystemActive = false;

        #endregion internal variables

        #region EventHooks

        /// <summary>
        /// Event hooks are useful for any custom code you would like to run alongside certain events in the fog system.
        /// </summary>
        internal FogSystemEventHooks EventHooks;

        #endregion

        private void Awake()
        {
            // Multiple Fog Systems are not supported.
            if (GameObject.FindObjectsOfType<FogSystem>().Count() > 1)
            {
                Debug.LogError("Multiple fog systems exist in the scene. This fog system only works with one active fog object in the scene.");
                return;
            }

            InitialiseSingleton();

            // Instantiate the Searcher / Actioner
            Searcher = new FogSystemSearchHelper();
            ActionRunner = new FogSystemActionHelper();

            // Instantiate an event system hook so that people can hook into events to run their own code
            EventHooks = new FogSystemEventHooks();

            if(_CommonSettings.StartMode == StartModes.Awake)
                Initialise();
        }

        private void Start()
        {
            // The "Start" start mode should be used if you wish to use EventHooks.
            if (_CommonSettings.StartMode == StartModes.Start)
                Initialise();
        }

        private void OnEnable()
        {
            if (_CommonSettings.StartMode == StartModes.Enable)
                Initialise();
        }

        /// <summary>
        /// Starts the Fog Systems tracking and activation/deactivation of Fog Tiles.
        /// Call this method only if the fog system is not active, and you have chosen Start Mode Manual.
        /// </summary>
        internal void Initialise()
        {
            if (FogSystemActive) return;

            // Remove effectors that are not fully initialised in the inspector
            if (Effectors.RemoveAll(o => o.TransformIsNull()) > 0)
                Debug.LogWarning($"[FogSystem] A number of effectors had no Transform and have been removed.");

            // If the effectors are not specified, and the AutoCreate feature is enabled in settings, create a cube that the user can move around to demonstrate the effector aspect of the fog system.
            if (Effectors.Count() < 1 && EffectorOptions.AutoCreateEffectorFromPlayers)
                foreach (Transform trans in GameObject.FindGameObjectsWithTag("Player").Select(o => o.transform))
                {
                    // Create the effectors from "Player" tagged game objects.
                    Debug.Log($"No effectors were found. Adding {trans.name} as it is tagged \"Player\" to the Effector list with sight range of {EFFECTOR_DEFAULT_SIGHT_RANGE}. If you would like to disable this behavior, please deselect {nameof(EffectorOptions.AutoCreateEffectorFromPlayers)} in FogSystem settings.");
                    AddEffector(trans, EFFECTOR_DEFAULT_SIGHT_RANGE);
                }

            // If no fog canvas exists, deactivate the system and inform the user.
            if (CurrentMode == FogSystemMode._2DMapSprite || CurrentMode == FogSystemMode._2DMapSize)
            {
                if (!utils.TryGetFogCanvas2D())
                {
                    Debug.LogError($"No 2D fog canvas with name {FOG_2D_CANVAS_DEFAULT_NAME} could be found. Please regenerate the 2D Fog. If you would like to assign a custom name, please change the constant name {nameof(FOG_2D_CANVAS_DEFAULT_NAME)} in FogSystem.cs");
                    return;
                }
            }
            else if (!utils.TryGetFogCanvas3D())
            {
                Debug.LogError($"No 3D fog holder with name {FOG_3D_PARENT_DEFAULT_NAME} could be found. Please regenerate the 3D fog.  If you would like to assign a custom name, please change the constant name {nameof(FOG_3D_PARENT_DEFAULT_NAME)} in FogSystem.cs");
                return;
            }

            // Get the tiles from the canvas.
            Transform[] tiles;

            if (CurrentMode == FogSystemMode._2DMapSprite || CurrentMode == FogSystemMode._2DMapSize)
                tiles = _FogSystemAdvancedSettings.FogCanvas.GetComponentsInChildren<Transform>();
            else
                tiles = _3DSettings.FogTileParent.GetComponentsInChildren<Transform>().Skip(1).ToArray();

            // Ensure the fog has been generated.
            if (tiles.Count() == 0)
            {
                Debug.LogError("Error from FogSystem: Please ensure you have generated tiles for the Fog System", transform);
                return;
            }

            // Initialise each tile as a fog item and add to the tiles list.
            foreach (Transform tile in tiles)
            {
                if (CurrentMode == FogSystemMode._2DMapSprite || CurrentMode == FogSystemMode._2DMapSize)
                    FogTiles.Add(new FogItem(FogTiles.Count, tile.gameObject));
                else
                    FogTiles.Add(new FogItem3D(FogTiles.Count, tile.gameObject));
            }

            // Activate the system.
            FogSystemActive = true;

            // Fire the Fog System Started event.
            EventHooks.InvokeOnFogSystemStarted();
        }

        private void LateUpdate()
        {
            if (!FogSystemActive) return;

            // Only activate a search if the next update is due
            if (Time.time > NextUpdateDue)
            {
                // Is an update nescessary? & Updates the thread safe positions.
                if (Searcher.CheckUpdateNescessaryAndUpdatePositionsToThreadSafe())
                {
                    switch (_CommonSettings.RegrowthMode)
                    {
                        // If regrowth mode is none, use the fastest method of inactive/active immediate tiling.
                        case FogRegrowthMode.None:
                            Searcher.SearchAndSetActiveImmediate(FogTiles);
                            break;

                        // If shrouding or regrowing is selected, use the async Searcher to find tiles that require changes.
                        case FogRegrowthMode.Regrowing:
                        case FogRegrowthMode.Shrouding:
                            Searcher.SearchForFogTilesAsync(FogTiles);
                            break;
                    }
                }

                // Set the next update to RefreshDelay from Settings
                NextUpdateDue = Time.time + _CommonSettings.RefreshDelay;
            }

            // If the action runner task is not in progress, run the action runner.
            if (_CommonSettings.RegrowthMode != FogRegrowthMode.None && !ActionRunner.TaskInProgress)
                ActionRunner.StartOpacityStepperAsync(FogTiles);
        }

        /// <summary>
        /// Grabs all the fog tiles.
        /// </summary>
        /// <returns>The list of all Fog Tiles</returns>
        internal IEnumerable<IFogItem> GetFogTiles() => FogTiles;

        /// <summary>
        /// Add transforms to the search effectors at runtime.
        /// Effectors are transforms that effect fog visibility.
        /// </summary>
        /// <param name="effectorTransform">The transform to add.</param>
        /// <param name="sightRange">The sight range of this effector. If not specified, will take the EFFECTOR_DEFAULT_SIGHT_RANGE value.</param>
        public void AddEffector(Transform effectorTransform, float sightRange = EFFECTOR_DEFAULT_SIGHT_RANGE)
        {
            if (effectorTransform == null)
                Debug.LogError("[FogSystem] You are attempting to add a transform to the effector list that doesn't exist. ");
            else
            {
                // Trigger a forced re-search in the Searcher
                Searcher.EffectorsChanged = true;
                // Add the new effector using the values specified
                Effectors.Add(new FogSystemEffector(effectorTransform, sightRange));
            }
        }

        /// <summary>
        /// Removes an effector from the system
        /// </summary>
        /// <param name="effectorTransform">the transform to remove</param>
        public void RemoveEffector(Transform effectorTransform)
        {
            // Find the effector by transform
            FogSystemEffector effectorToRemove = Effectors.FirstOrDefault(o => o.GetTransform());

            if (effectorToRemove == null)
                Debug.LogError("[FogSystem] The effector you are trying to remove was not found in the effector list. ");
            else
            {
                // Trigger a forced re-search in the Searcher
                Searcher.EffectorsChanged = true;
                // Remove from the effector list.
                Effectors.Remove(effectorToRemove);
            }

            // Trigger a manual instant update if there is no effectors left in the system.
            if (Effectors.Count == 0 && _CommonSettings.RegrowthMode != FogRegrowthMode.None)
                Searcher.ManualInstantUpdate(GetFogTiles(), _CommonSettings.RegrowthMode == FogRegrowthMode.Regrowing ? FogState.Darkest : FogState.SemiTransparent);
        }

        #region Generator Code
        /// <summary>
        /// Refreshes the current generator used. Driven by selected FogSystemMode
        /// </summary>
        void RefreshGenerator()
        {
            switch (CurrentMode)
            {
                case FogSystemMode._2DMapSize:
                    generator = new FogGenerator2DMapSize(this);
                    break;
                case FogSystemMode._2DMapSprite:
                    generator = new FogGenerator2DMapSprite(this);
                    break;
                case FogSystemMode._3D:
                    generator = new FogGenerator3D(this);
                    break;
                default:
                    throw new System.Exception("Unsupported fog generation method selected.");
            }
        }

        /// <summary>
        /// Creates a fog map of tiles above the specified map tile
        /// </summary>
        public void GenerateFog()
        {
            InitialiseSingleton();
            RefreshGenerator();
            generator.GenerateFog();
            generator.FinaliseFogGeneration();
        }

        /// <summary>
        /// Undo the creation of the 2D fog tiles.
        /// </summary>
        public void RemoveFog()
        {
            InitialiseSingleton();
            RefreshGenerator();
            generator.RemoveFog();
            generator.FinaliseFogRemoval();
        }

        /// <summary>
        /// Used to clean up any leftover items after removing fog. I.e. The map item.
        /// </summary>
        public void CleanupArtifacts()
        {
            InitialiseSingleton();
            RefreshGenerator();
            generator.CleanupArtifacts();
        }
        #endregion

        #region MonoUtils
        // Some items can only be called from Mono, so this will serve as a proxy to the Generators.

        /// <summary>
        /// Instantiates an object and returns to sender. Only used by FogSystem Generators as a proxy.
        /// </summary>
        /// <param name="prefab">Prefab to clone</param>
        /// <param name="pos">Position</param>
        /// <param name="rot">Rotation</param>
        /// <param name="parent">Parent to assign</param>
        /// <returns>The instantiated gameobject</returns>
        internal GameObject CreateItem(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent) => Instantiate(prefab, pos, rot, parent);
        
        /// <summary>
        /// Instantiates an object and returns to sender. Only used by FogSystem Generators as a proxy.
        /// </summary>
        /// <param name="prefab">Prefab to clone</param>
        /// <returns></returns>
        internal GameObject CreateItem(GameObject prefab) => Instantiate(prefab);

        /// <summary>
        /// Calls destroy immediate on passed gameobject. Only used by FogSystem Generators as a proxy.
        /// </summary>
        /// <param name="item">GameObject to be destroyed</param>
        internal void DestroyItem(GameObject item) => DestroyImmediate(item);
        #endregion
    }
}