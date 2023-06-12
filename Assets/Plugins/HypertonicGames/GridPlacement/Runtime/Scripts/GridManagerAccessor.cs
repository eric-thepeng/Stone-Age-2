using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hypertonic.GridPlacement
{
    /// <summary>
    /// This class is used to easily get a reference to the GridManager instance in the scene.
    /// </summary>
    public class GridManagerAccessor : MonoBehaviour
    {
        public delegate void GridManagerAccessorEvent(GridManager gridManager);

        public static event GridManagerAccessorEvent OnGridManagerRegistered;
        public static event GridManagerAccessorEvent OnGridManagerUnregistered;

 
        private static Dictionary<string, GridManager> _gridManagerDictionary = new Dictionary<string, GridManager>();

        private static string _currentlySelectedGridManagerKey = null;

        /// <summary>
        /// If multiple grid managers have been register then this function will return the selected grid manager. If there is only one grid manager registered then it will returned by default.
        /// </summary>
        public static GridManager GridManager
        {
            get
            {
                if(_gridManagerDictionary.Count == 0)
                {
                    Debug.LogError("No grid managers have been registered");
                    return null;
                }

                if(string.IsNullOrEmpty(_currentlySelectedGridManagerKey))
                {
                    return _gridManagerDictionary.First().Value;
                }

                if(!HasGridManager(_currentlySelectedGridManagerKey))
                {
                    Debug.LogErrorFormat("The selected grid manager key is: {0} but there is not a registered grid manager with that key.", _currentlySelectedGridManagerKey);
                    return null;
                }

                return GetGridManagerByKey(_currentlySelectedGridManagerKey);
            
            }
        }

        /// <summary>
        /// Returns a grid manager by selecting it by it's key.
        /// </summary>
        /// <param name="key">The key set in the grid settings </param>
        /// <returns>The grid manager with that key </returns>
        public static GridManager GetGridManagerByKey(string key)
        {
            if (!_gridManagerDictionary.ContainsKey(key))
            {
                Debug.LogError("Error in GetGridManagerByKey. There is no grid manager registered with the key: " + key);
                return null;
            }

            return _gridManagerDictionary[key];
        }

        /// <summary>
        /// Registers a Grid Manager with a key. The key should be set from the key value in the Grid Settings object.
        /// </summary>
        /// <param name="key">The key for the grid manager </param>
        /// <param name="gridManager">The grid manager to register in the Grid Manager Accessor </param>
        public static void RegisterGridManager(string key, GridManager gridManager)
        {
            if (_gridManagerDictionary.ContainsKey(key))
            {
                Debug.LogError("Cannot Register this grid manager because there is already a grid manager registered with the key: " + key);
                return;
            }

            _gridManagerDictionary.Add(key, gridManager);
            OnGridManagerRegistered?.Invoke(gridManager);
        }

        /// <summary>
        /// Unregisters a Grid Manager by it's key 
        /// </summary>
        /// <param name="key">The key associated with the grid manager </param>
        /// <param name="gridManager"> The Grid Manager to be unregistered </param>
        public static void UnregisterGridManager(string key, GridManager gridManager)
        {
            if (!_gridManagerDictionary.ContainsKey(key))
            {
                Debug.LogError("Cannot Unregister this grid manager because there is no grid manager registered with the key: " + key);
                return;
            }

            _gridManagerDictionary.Remove(key);
            OnGridManagerUnregistered?.Invoke(gridManager);
        }

        /// <summary>
        /// A check to see if the Grid Manager associated with a given key has been registered
        /// </summary>
        /// <param name="key">The key for the grid manager</param>
        /// <returns>True if the Grid Manager is registered </returns>
        public static bool HasGridManager(string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                return _gridManagerDictionary.Count > 0;
            }

            return _gridManagerDictionary.ContainsKey(key);
        }

        /// <summary>
        /// Trys to get the default Grid Manager
        /// </summary>
        /// <param name="gridManager"> The reference to the Grid Manager if it exists </param>
        /// <returns>True if the Grid Manager has been registered. False if not. </returns>
        public static bool TryGetGridManager(out GridManager gridManager)
        {
            if (HasGridManager())
            {
                gridManager = GridManager;
                return true;
            }
            else
            {
                gridManager = null;
                return false;
            }
        }

        /// <summary>
        /// Trys to get a Grid Manager by it's key
        /// </summary>
        /// <param name="key">The key of the grid manager</param>
        /// <param name="gridManager"> The reference to the Grid Manager if it exists </param>
        /// <returns>True if the Grid Manager has been registered. False if not. </returns>
        public static bool TryGetGridManager(string key, out GridManager gridManager)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("The key cannot be null");
                gridManager = null;
                return false;
            }

            if (HasGridManager(key))
            {
                gridManager = GetGridManagerByKey(key);
                return true;
            }
            else
            {
                gridManager = null;
                return false;
            }
        }

        /// <summary>
        /// Set the selected (default) Grid Manager. After setting the selected Grid Manager by it's key you
        /// can use the Grid Manager property to access the selected Grid Manager quicker.
        /// </summary>
        /// <param name="key"> The key of the selected grid manager</param>
        public static void SetSelectedGridManager(string key)
        {
            _currentlySelectedGridManagerKey = key;
        }
    }
}