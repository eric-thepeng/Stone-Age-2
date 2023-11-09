using System.Collections;
using System.Collections.Generic;
using Hypertonic.GridPlacement.CustomSizing;
using Hypertonic.GridPlacement.Enums;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(GridHeightPositioner))]
[RequireComponent(typeof(GridValidator))]

public class PlaceableObject : MonoBehaviour
{
    public string GridKey { get; private set; }

    [Header("Runtime Info")]
    [SerializeField]
    private Vector2 gridCellIndex;

    public Vector2 GridCellIndex
    {
        get { return gridCellIndex; }
        private set { gridCellIndex = value; }
    }

    public ObjectAlignment ObjectAlignment { get; private set; }

    public Vector3? ObjectBounds { get; set; }

    public Vector3? PositionOffset { get; set; }

    public bool HasBeenPlaced => !string.IsNullOrEmpty(GridKey);

    public void Setup(string gridKey, Vector2Int gridCellIndex, ObjectAlignment objectAlignment)
    {
        GridKey = gridKey;
        GridCellIndex = gridCellIndex;
        ObjectAlignment = objectAlignment;
    }

    [Header("BISO Settings")]
    [SerializeField]
    private BuildingISO BISO;

    [SerializeField]
    private List<string> _gridObjectTags;


    public bool containsTag(string ObjectTag)
    {
        if (_gridObjectTags.Contains(ObjectTag)) return true;
        else return false;
    }

    public BuildingISO GetBuildingISO()
    {
        return BISO;
    }


    private List<GameObject> objectsWithEffects = new List<GameObject>();

    private BuildingManager buildingManager;

    private void Start()
    {
        buildingManager = FindObjectOfType<BuildingManager>();
        CheckEffects(transform);
    }

    private void Update()
    {
        if (buildingManager.SwitchedPlacementMode)
        {
            if (buildingManager.CurrentPlacementMode)
            {
                DisableEffects();
            } else
            {
                EnableEffects();
            }
        }
    }

    void CheckEffects(Transform parent)
    {
        foreach (Transform child in parent)
        {
            VisualEffect visualEffect = child.GetComponent<VisualEffect>();
            ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
            if ((visualEffect != null && visualEffect.enabled == true)
                || particleSystem != null && visualEffect.enabled == true)
            {
                objectsWithEffects.Add(child.gameObject);
            }


            if (child.childCount > 0)
            {
                CheckEffects(child);
            }
        }
    }

    public void DisableEffects()
    {
        if (objectsWithEffects != null)
        {
            foreach (GameObject obj in objectsWithEffects)
            {
                VisualEffect visualEffect = obj.GetComponent<VisualEffect>();
                if (visualEffect != null)
                {
                    visualEffect.enabled = false;
                }

                ParticleSystem particleSystem = obj.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    obj.SetActive(false);
                    //particleSystem.Play();
                }
            }

            //objectsWithEffects.Clear();
        }
    }

    public void EnableEffects()
    {
        if (objectsWithEffects != null)
        {
            foreach (GameObject obj in objectsWithEffects)
            {
                VisualEffect visualEffect = obj.GetComponent<VisualEffect>();
                if (visualEffect != null)
                {
                    visualEffect.enabled = true;
                }

                ParticleSystem particleSystem = obj.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    obj.SetActive(true);
                    //particleSystem.Play();
                }
            }

        }
    }





}