using Hypertonic.GridPlacement.Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hypertonic.GridPlacement.GridInput
{
    public class GridInputDetector : MonoBehaviour
    {
        public Action<Vector3> _interactionCallback;

        public GridInputDefinition GridInputDefinition { get; private set; }

        private GridSettings _gridSettings;
        private GridManager _gridManager;

        private RaycastHit _hit;

        private LayerMask _gridLayerMask;
        private LayerMask _originalLayerMask;

        private Camera _gridCamera;
        private PhysicsRaycaster _physicsRaycaster;

        private void FixedUpdate()
        {
            if (CanInteract())
            {
                Vector3? input = GetInputPosition();

                if (!input.HasValue)
                    return;

                if (PointerOverUIDetector.IsPointerOverUIElement(input.Value) && _gridSettings.PreventInputThroughUI)
                {
                    return;
                }

                if (!input.HasValue)
                {
                    return;
                }

                if (_gridCamera == null)
                {
                    Debug.LogError("The grid camera has not been set.");
                    return;
                }

                Ray ray = _gridCamera.ScreenPointToRay(input.Value);

                if (Physics.Raycast(ray, out _hit, Mathf.Infinity, _gridLayerMask))
                {
                    _interactionCallback?.Invoke(_hit.point);
                }
            }
        }

        private void OnDestroy()
        {
            _gridManager.OnGridShown -= HandleGridShown;
            _gridManager.OnGridHidden -= HandleGridHidden;
        }

        public void Setup(Action<Vector3> interactionCallback, GridSettings gridSettings, GridManager gridManager)
        {
            _interactionCallback = interactionCallback;
            _gridSettings = gridSettings;
            _gridManager = gridManager;
            GridInputDefinition = _gridSettings.PlatformGridInputsDefinitionMappings.Find(x => x.RuntimePlatform.Equals(Application.platform)).GridInputDefinition;

            Camera gridCamera = GridUtilities.GetCameraForGrid(_gridSettings);

            if (!gridCamera.TryGetComponent<PhysicsRaycaster>(out var physicsRaycaster))
            {
                physicsRaycaster = gridCamera.gameObject.AddComponent<PhysicsRaycaster>();
                Debug.LogFormat("There is no PhyicsRaycaster attached to the grid camera. Gameobject name: {0}. Adding raycaster to camera.",
                string.IsNullOrEmpty(gridSettings.GridCanvasEventCameraName) ? Camera.main.gameObject.name : gridSettings.GridCanvasEventCameraName);
            }

            _gridCamera = gridCamera;
            _physicsRaycaster = physicsRaycaster;

            int layerMask = LayerMask.GetMask("Grid", "UI");

            _gridLayerMask = layerMask;
            _originalLayerMask = _physicsRaycaster.eventMask;

            _gridManager.OnGridShown += HandleGridShown;
            _gridManager.OnGridHidden += HandleGridHidden;
        }

        public void UpdateplatformGridInputsDefinitionMappings(List<PlatformGridInputsDefinitionMapping> platformGridInputsDefinitionMappings)
        {
            GridInputDefinition = platformGridInputsDefinitionMappings.Find(x => x.RuntimePlatform.Equals(Application.platform)).GridInputDefinition;
        }

        private void HandleGridShown()
        {
            // Store copy of original event mask
            _originalLayerMask = _physicsRaycaster.eventMask;

            // Set the raycaster to only work on grid objects.
            _physicsRaycaster.eventMask = _gridLayerMask;
        }

        private void HandleGridHidden()
        {
            _physicsRaycaster.eventMask = _originalLayerMask;
        }

        private Vector3? GetInputPosition()
        {
            return GridInputDefinition.InputPosition();
        }

        private bool CanInteract()
        {
            return GridInputDefinition.CanInteract();
        }
    }
}
