using System.Collections;
using UnityEngine;
using Hypertonic.GridPlacement;

public class GridOperationManager : MonoBehaviour
{
    [SerializeField]
    private GridSettings _gridSettings;

    [SerializeField]
    private GameObject _gridEmptyObjectPrefab;

    public GridManager _gridManager;

    public bool operateStatus;
    public GameObject itemPlaced;

    private void Start()
    {
        if(_gridSettings == null)
        {
            Debug.LogError("The GameManager needs to have the grid settings assigned.");
            return;
        }

        if(_gridEmptyObjectPrefab == null)
        {
            Debug.LogError("The GameManager needs to have the grid object prefab assigned.");
            return;
        }

        _gridManager = new GameObject("Grid Manager").AddComponent<GridManager>();
        _gridManager.Setup(_gridSettings);

        StartCoroutine(CheckForInput());
        AddGridCoordinateManager();

    }


    public void SwitchPaintMode()
    {
        if (GridManagerAccessor.GridManager.IsPlacingGridObject)
        {
            EndPaintMode();
        } else
        {
            StartPaintMode();
        }
    }

    public void StartPaintMode()
    {
        _gridManager.StartPaintMode(_gridEmptyObjectPrefab);
    }

    public void EndPaintMode()
    {
        _gridManager.EndPaintMode();
    }

    private void AddGridCoordinateManager()
    {
        if (GetComponent<GridCoordinateManager>() == null)
        {
            gameObject.AddComponent<GridCoordinateManager>();

        }
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
        if (Input.GetMouseButton(0) && !GridManagerAccessor.GridManager.ObjectToPlace.GetComponent<GridObjectTags>().containsTag("EmptyObject"))
        {
                //Debug.Log(_gridManager.ConfirmPlacement());
                itemPlaced = _gridManager.ObjectToPlace;
            operateStatus = _gridManager.ConfirmPlacement();
        }
#endif
        }
    }
}
