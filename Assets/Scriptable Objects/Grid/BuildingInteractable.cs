using System.Collections;
using System.Collections.Generic;
using Hypertonic.GridPlacement.CustomSizing;
using Hypertonic.GridPlacement.Enums;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(GridHeightPositioner))]
[RequireComponent(typeof(GridValidator))]

public class BuildingInteractable : MonoBehaviour
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

}
