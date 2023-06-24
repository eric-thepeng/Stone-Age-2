using Hypertonic.GridPlacement.Enums;
using Hypertonic.GridPlacement.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Hypertonic.GridPlacement
{
    [CreateAssetMenu(fileName = "Grid Settings", menuName ="Grid/Grid Settings")]
    public class GridSettings : ScriptableObject
    {
        public string Key = "DefaultGrid";

        [HideInInspector] 
        [Tooltip("The width of the grid in Unity World Space")]
        public double Width = 10;

        [HideInInspector]
        [Tooltip("The height of the grid in Unity World Space")]
        public double Height = 10;

        [HideInInspector]
        public Vector2Int GridSizeRatio = Vector2Int.one;

        [HideInInspector]
        [Tooltip("The amount of cells in the x dimension")]
        public int AmountOfCellsX = 10;

        [HideInInspector]
        [Tooltip("The amount of cells in the y dimension")]
        public int AmountOfCellsY = 10;

        [HideInInspector]
        public float CellSize { get => (float)Width / (float)AmountOfCellsX; }

        [HideInInspector]
        public float DesiredCellSize;

        [HideInInspector]
        [Tooltip("The position to display the grid.")]
        public Vector3 GridPosition;

        [Tooltip("The rotation of the grid")]
        [Range(0, 359)]
        public float GridRotation;

        [Tooltip("Determines if the objects being added to the grid should be parented to the grid transform.")]
        public bool ParentToGrid;

        [Space(10)]
        [Header("Grid Cell Display")]
        
        [Tooltip("This should be a tilable sprite so it can repeat across the background")]
        public Sprite CellImage;

        [Tooltip("This image is overlayed on the grid cells that are taken up by grid objects")]
        public Sprite OccupiedCellImage;
        
        [Tooltip("This colour of the occupied cell image sprite when the placement of the grid object is valid")]
        public Color CellColourAvailable = new Color(0.6383647f, 1, 0.6383647f);

        [Tooltip("This colour of the occupied cell image sprite when the placement of the grid object is not valid")]
        public Color CellColourNotAvailable = new Color(1, 0.5157232f, 0.5157232f);

        [Tooltip("This colour of the occupied cells for an object that has already been placed")]
        public Color CellColourPlaced = new Color(0.8207547f, 0.8207547f, 0.8207547f);

        [Tooltip("Determines if the occipied cells should show when the grid object is partially outside of the grid area when being placed.")]
        public bool HidePlacementCellsIfOutsideOfGrid = true;

        [Tooltip("Determines if the occipied cells should be displayed.")]
        public bool ShowOccupiedCells = true;

        [Space(10)]
        [Tooltip("The name of the gameobject that the camera is attached to. The camera will be set to fire raycasts at the grid. If null it'll default to the main camera")]
        public string GridCanvasEventCameraName;

        [Header("Grid Object Placement Materials")]

        [Tooltip("Determines if the object's materials should change to the placeable material when in a placeable state.")]
        public bool UsePlaceableMaterial = true;

        [Tooltip("Determines if the object's materials should change to the placeable material when in a unplaceable state.")]
        public bool UseUnplaceableMaterial = true;

        [Tooltip("The material the object that is being placed if the placement is valid")]
        public Material ObjectPlaceableMaterial;

        [Tooltip("The material the object that is being placed if the placement is not valid")]
        public Material ObjectUnPlaceableMaterial;

        [Header("Placement Settings")]
        [Tooltip("This sets the initial alignment of an object being added to the grid")]
        public ObjectAlignment DefaultAlignment = ObjectAlignment.UPPER_LEFT;

        [Tooltip("This sets the default grid cell all new objects will be placed at when EnterPlacementMode is called. This is overwritten by the optional placement settings.")]
        public Vector2Int InitialPlacementCellIndex = Vector2Int.zero;

        [Space(10)]
        [Header("Grid Input")]
        [Tooltip("Define the grid input definition for each runtime platform")]
        public List<PlatformGridInputsDefinitionMapping> PlatformGridInputsDefinitionMappings = new List<PlatformGridInputsDefinitionMapping>()
        {
            new PlatformGridInputsDefinitionMapping(RuntimePlatform.Android, null),
            new PlatformGridInputsDefinitionMapping(RuntimePlatform.IPhonePlayer, null),
            new PlatformGridInputsDefinitionMapping(RuntimePlatform.LinuxEditor, null),
            new PlatformGridInputsDefinitionMapping(RuntimePlatform.LinuxPlayer, null),
            new PlatformGridInputsDefinitionMapping(RuntimePlatform.OSXEditor, null),
            new PlatformGridInputsDefinitionMapping(RuntimePlatform.OSXPlayer, null),
            new PlatformGridInputsDefinitionMapping(RuntimePlatform.PS4, null),
            new PlatformGridInputsDefinitionMapping(RuntimePlatform.PS5, null),
            new PlatformGridInputsDefinitionMapping(RuntimePlatform.Stadia, null),
            new PlatformGridInputsDefinitionMapping(RuntimePlatform.Switch, null),
            new PlatformGridInputsDefinitionMapping(RuntimePlatform.tvOS, null),
            new PlatformGridInputsDefinitionMapping(RuntimePlatform.WindowsEditor, null),
            new PlatformGridInputsDefinitionMapping(RuntimePlatform.WindowsPlayer, null),
            new PlatformGridInputsDefinitionMapping(RuntimePlatform.XboxOne, null),
            new PlatformGridInputsDefinitionMapping(RuntimePlatform.GameCoreXboxOne, null),
            new PlatformGridInputsDefinitionMapping(RuntimePlatform.GameCoreXboxSeries, null),
            new PlatformGridInputsDefinitionMapping(RuntimePlatform.WebGLPlayer, null),
        };

        [Tooltip("When true, the input detection of the grid will not trigger when pointer position is over the UI.")]
        public bool PreventInputThroughUI = true;

        [HideInInspector]
        public bool DisplayInEditor = false;

        [HideInInspector]
        public int SizingOption;
    }
}