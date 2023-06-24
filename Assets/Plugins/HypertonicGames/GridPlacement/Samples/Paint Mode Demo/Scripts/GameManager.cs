using System.Collections;
using UnityEngine;

namespace Hypertonic.GridPlacement.Example.PaintMode
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private GridSettings _gridSettings;

        [SerializeField]
        private GameObject _gridObjectPrefab;

        private GridManager _gridManager;

        private void Start()
        {
            if(_gridSettings == null)
            {
                Debug.LogError("The GameManager needs to have the grid settings assigned.");
                return;
            }

            if(_gridObjectPrefab == null)
            {
                Debug.LogError("The GameManager needs to have the grid object prefab assigned.");
                return;
            }

            _gridManager = new GameObject("Grid Manager").AddComponent<GridManager>();
            _gridManager.Setup(_gridSettings);

            StartCoroutine(CheckForInput());
        }

        public void StartPaintMode()
        {
            _gridManager.StartPaintMode(_gridObjectPrefab);
        }

        public void EndPaintMode()
        {
            _gridManager.EndPaintMode();
        }

        private IEnumerator CheckForInput()
        {
            while (true)
            {

                yield return null;

                // Don't check for input if the grid manager isn't placing an object
                if (!_gridManager.IsPlacingGridObject)
                    continue;

                // Check input from new Input System
#if ENABLE_INPUT_SYSTEM
                if (UnityEngine.InputSystem.Mouse.current.leftButton.isPressed)
                {
                    _gridManager.ConfirmPlacement();
                }
#endif


                // Check input from old Input System
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetMouseButtonDown(0))
            {
                _gridManager.ConfirmPlacement();
            }
#endif
            }
        }
    }
}