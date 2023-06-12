using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypertonic.GridPlacement
{
    public class GridObjectVisualManager : MonoBehaviour
    {
        private GameObject _gridObject;
        private GridSettings _gridSettings;

        private Dictionary<MeshRenderer, Material[]> _meshRendererMaterials = new Dictionary<MeshRenderer, Material[]>();

        private List<MeshRenderer> _gridObjectMeshRenderers = new List<MeshRenderer>();

        public void Setup(GridSettings gridSettings, GameObject gridObject)
        {
            _gridSettings = gridSettings;
            _gridObject = gridObject;

            if (!gridSettings.UsePlaceableMaterial && !gridSettings.UseUnplaceableMaterial) return;

            Clear();

            MeshRenderer meshRenderer = _gridObject.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                MeshRenderer[] renderers = _gridObject.GetComponents<MeshRenderer>();

                foreach (var renderer in renderers)
                {
                    _gridObjectMeshRenderers.Add(renderer);
                }
            }

            FindMeshRenderersInChildren(_gridObject.transform, _gridObjectMeshRenderers);

            // Set the object placement material

            foreach (var renderer in _gridObjectMeshRenderers)
            {
                var meshRednererMaterialsArray = renderer.materials;
                _meshRendererMaterials.Add(renderer, meshRednererMaterialsArray);

                Material[] newMaterials = new Material[renderer.materials.Length];

                for (int i = 0; i < newMaterials.Length; i++)
                {
                    // Set the object material to the placement material
                    newMaterials[i] = gridSettings.ObjectPlaceableMaterial;
                }

                renderer.materials = newMaterials;
            }
        }

        private void FindMeshRenderersInChildren(Transform transform, List<MeshRenderer> meshRenderers)
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out MeshRenderer meshRenderer))
                {
                    MeshRenderer[] renderers = meshRenderer.GetComponents<MeshRenderer>();

                    foreach (var render in renderers)
                    {
                        _gridObjectMeshRenderers.Add(render);
                    }
                }

                FindMeshRenderersInChildren(child, meshRenderers);
            }
        }

        public void SetIsValid(bool isValid)
        {
            if (!_gridSettings.UsePlaceableMaterial && !_gridSettings.UseUnplaceableMaterial)
            {
                return;
            }

            if (isValid)
            {
                DisplayObjectAsValid();
            }
            else
            {
                DisplayObjectAsInvalid();
            }
        }

        private void DisplayObjectAsValid()
        {
            if (_gridSettings.UsePlaceableMaterial)
            {
                AssignPlaceableMaterials();
            }
            else
            {
                AssignOriginalMaterials();
            }
        }

        private void AssignPlaceableMaterials()
        {
            foreach (var meshRenderer in _gridObjectMeshRenderers)
            {
                Material[] newMaterials = new Material[meshRenderer.materials.Length];

                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = _gridSettings.ObjectPlaceableMaterial;
                }

                meshRenderer.materials = newMaterials;
            }
        }

        private void AssignUnPlaceableMaterials()
        {
            foreach (var meshRenderer in _gridObjectMeshRenderers)
            {
                Material[] newMaterials = new Material[meshRenderer.materials.Length];

                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = _gridSettings.ObjectUnPlaceableMaterial;
                }

                meshRenderer.materials = newMaterials;
            }
        }

        private void AssignOriginalMaterials()
        {
            foreach (var m in _gridObjectMeshRenderers)
            {
                var materials = _meshRendererMaterials[m];
                m.materials = materials;
            }
        }

        private void DisplayObjectAsInvalid()
        {
            if(_gridSettings.UseUnplaceableMaterial)
            {
                AssignUnPlaceableMaterials();
            }
            else
            {
                AssignOriginalMaterials();
            }
        }

        public void ResetObjectMaterial()
        {
            if (!_gridSettings.UsePlaceableMaterial && !_gridSettings.UseUnplaceableMaterial) return;

            foreach (var m in _gridObjectMeshRenderers)
            {
                var materials = _meshRendererMaterials[m];
                m.materials = materials;
            }

            Clear();
        }

        public void Clear()
        {
            if (_gridSettings == null) return;

            if (!_gridSettings.UsePlaceableMaterial && !_gridSettings.UseUnplaceableMaterial) return;

            _gridObjectMeshRenderers.Clear();
            _meshRendererMaterials.Clear();
        }
    }
}