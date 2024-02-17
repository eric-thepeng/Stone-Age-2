using Hypertonic.GridPlacement.GridInput;
using Hypertonic.GridPlacement.Models;
using UnityEditor;
using UnityEngine;

namespace Hypertonic.GridPlacement.Editor
{
    [InitializeOnLoad]
    [CustomEditor(typeof(GridSettings)), CanEditMultipleObjects]
    public class GridSettingsEditor : UnityEditor.Editor
    {
        public enum GridSizingMethod { GRID_SIZE_AND_CELL_COUNT, CELL_SIZE_AND_CELL_COUNT, CELL_SIZE_AND_GRID_SIZE }

        private GridSettings _gridSettings;

        private GameObject _gridDisplayManagerObject;
        private GridDisplayManager _gridDisplayManager;

        private double _oldGridWidth;

        private int _cellCountX = 1;
        private int _oldCellCountX = 1;

        private double _cellCountY = 1;
        private double _oldCellCountY = 1;

        private bool _refreshRequired = false;

        private Vector2Int _oldGridSizeRatio;
        private Vector3 _oldGridPosition;

        private Sprite _oldGridCellImage;

        private float _oldDesiredCellSize;

        string[] _sizingDropDownOptions = new string[]
           {
                "Grid Size & Cell Count", "Cell Size & Cell Count", "Grid Size & Cell Size",
           };

        private void Awake()
        {
            GridLayersManager.CreateLayersIfNotPresent(new string[] { "Grid", "PlacementGrid" });

            _gridSettings = (GridSettings)target;

            SetNullInputDefinitions();
        }

        public void OnEnable()
        {
            if (Application.isPlaying) return;

            if (_gridSettings == null) return;

            _oldGridSizeRatio = _gridSettings.GridSizeRatio;

            _cellCountX = _gridSettings.AmountOfCellsX;
            _oldCellCountX = _cellCountX;

            _cellCountY = _gridSettings.AmountOfCellsY;
            _oldCellCountY = _cellCountY;

            _oldGridCellImage = _gridSettings.CellImage;

            _gridSettings.DesiredCellSize = _gridSettings.CellSize;

            _oldDesiredCellSize = _gridSettings.DesiredCellSize;

            SceneView.duringSceneGui += HandleSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= HandleSceneGUI;
        }

        private void HandleSceneGUI(SceneView sv)
        {
            Vector3 point = _gridSettings.GridPosition;
            Handles.Label(_gridSettings.GridPosition, _gridSettings.Key);

            point = Handles.PositionHandle(point, Quaternion.identity);

            if (GUI.changed)
            {
                Undo.RecordObject(target, "Move Grid Position");
                _gridSettings.GridPosition = point;
            }
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(10);
            if (GUILayout.Button("Preview Grid"))
            {
                _gridSettings.DisplayInEditor = true;
                UpdateGridSize();
                DisplayGrid();
            }

            if (GUILayout.Button("Visualise Region Preset"))
            {
                DisplayPresetGrid();
            }
            
            GUILayout.Space(10);

            if (GUILayout.Button("Hide Grid"))
            {
                _gridSettings.DisplayInEditor = false;
                HideGrid();
                HidePresetGrid();
            }
            // if (GUILayout.Button("Hide Region Preset"))
            // {
            // }
            //
            GUILayout.Space(20);
            
            base.DrawDefaultInspector();

            if (Application.isPlaying) return;
            
            GUILayout.Space(50);
            GUILayout.Label("Grid Sizing", EditorStyles.boldLabel);
            GUILayout.Space(10);

            #region Grid Cell Image
            
            if(_oldGridCellImage != _gridSettings.CellImage)
            {
                _oldGridCellImage = _gridSettings.CellImage;
                _refreshRequired = true;
            }

            #endregion

            #region Ratio
            _gridSettings.GridSizeRatio = EditorGUILayout.Vector2IntField("Width to Height Ratio: ", _gridSettings.GridSizeRatio);

            if(_gridSettings.GridSizeRatio.x <= 0)
            {
                _gridSettings.GridSizeRatio.x = 1;
            }

            if (_gridSettings.GridSizeRatio.y <= 0)
            {
                _gridSettings.GridSizeRatio.y = 1;
            }

            if (_oldGridSizeRatio != _gridSettings.GridSizeRatio)
            {
                _oldGridSizeRatio = _gridSettings.GridSizeRatio;
                _refreshRequired = true;
            }

            #endregion Ratio

            #region Size

            _gridSettings.SizingOption = EditorGUILayout.Popup("Sizing Option", _gridSettings.SizingOption, _sizingDropDownOptions);

            DisplayGridSize();

            DisplayCellSize();

            DisplayCellCount();

            GUILayout.Space(20);

            #endregion Size


            #region Grid Position

            _gridSettings.GridPosition = EditorGUILayout.Vector3Field("Grid Position: ", _gridSettings.GridPosition);
            if (_gridSettings.GridPosition != _oldGridPosition)
            {
                if(_gridDisplayManager != null)
                {
                    _gridDisplayManager.UpdateGridPosition(_gridSettings.GridPosition);
                }
                else
                {
                    DisplayGrid();
                }
            }

            #endregion Grid Position

            if (_refreshRequired)
            {
                UpdateGridSize();
                RefreshGrid();
                EditorUtility.SetDirty(_gridSettings);
            }

            if(!GridUtilities.GridSettingsValid(_gridSettings))
            {
                EditorGUILayout.HelpBox("Invalid Grid Cell Count. Please adjust the cell count or the width to height ratio", MessageType.Error);
            }


        }

        private void DisplayGrid()
        {
            if (!_gridSettings.DisplayInEditor) return;

            HideGrid();
            HidePresetGrid();

            _gridDisplayManagerObject = new GameObject("Grid Display Manager - " + _gridSettings.Key);
            
            _gridDisplayManager = _gridDisplayManagerObject.AddComponent<GridDisplayManager>();

            _gridDisplayManager.Setup(null, _gridSettings);
            _gridDisplayManager.Display();
        }

        private void HideGrid()
        {
            // Finding object instead of storing reference incase its lost on script recomplie
            var gridManagerObject = GameObject.Find("Grid Display Manager - " + _gridSettings.Key);

            if (gridManagerObject == null)
            {
                return;
            }

            gridManagerObject.GetComponent<GridDisplayManager>().Hide();

            DestroyImmediate(gridManagerObject);
            
        }

        private void DisplayPresetGrid()
        {
            HideGrid();
            HidePresetGrid();

            GameObject gridObject = new GameObject("Grid Preset Manager - " + _gridSettings.Key);
            gridObject.AddComponent<Grid>();
            
            _gridDisplayManagerObject = Instantiate(_gridSettings.RegionTypePresetTilemap,gridObject.transform);
            
            _gridDisplayManagerObject.transform.localScale = new Vector3(2, 2, 2);
            _gridDisplayManagerObject.transform.rotation = Quaternion.Euler(90,0,0);
            
            _gridDisplayManagerObject.transform.position = _gridSettings.GridPosition -
                                                           new Vector3(_gridSettings.CellSize * 0.5f * _gridSettings.AmountOfCellsX, 0,
                                                               _gridSettings.CellSize * 0.5f *  _gridSettings.AmountOfCellsY);

        }

        private void HidePresetGrid()
        {
            var gridManagerObject = GameObject.Find("Grid Preset Manager - " + _gridSettings.Key);

            if (gridManagerObject == null)
            {
                return;
            }

            DestroyImmediate(gridManagerObject);
        }

        private void RefreshGrid()
        {
            if (!_gridSettings.DisplayInEditor) return;

            _refreshRequired = false;

            if(_gridDisplayManager == null)
            {
                DisplayGrid();
            }

            _gridDisplayManager.Setup(null, _gridSettings);
            _gridDisplayManager.Display();
        }

        private void UpdateGridSize()
        {
            double x = (double)_gridSettings.Width / _gridSettings.GridSizeRatio.x;
            _gridSettings.Height = (x * _gridSettings.GridSizeRatio.y);

            _gridSettings.AmountOfCellsX = _cellCountX;

            double cellSize = (double)_gridSettings.Width / _gridSettings.AmountOfCellsX;
            _cellCountY = _gridSettings.Height / cellSize;

            float countY = (float)_cellCountY;
            _gridSettings.AmountOfCellsY = Mathf.RoundToInt(countY);
        }

        private void SetNullInputDefinitions()
        {
            string[] mouseInputDefinitionGuids = AssetDatabase.FindAssets("t:MouseInputDefinition");
            GridInputDefinition mouseInputDefinition = null;

            if (mouseInputDefinitionGuids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(mouseInputDefinitionGuids[0]);
                mouseInputDefinition = AssetDatabase.LoadAssetAtPath<MouseInputDefinition>(path);
            }

            string[] touchInputDefinitionGuids = AssetDatabase.FindAssets("t:TouchInputDefinition");
            GridInputDefinition touchInputDefinition = null;

            if (touchInputDefinitionGuids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(touchInputDefinitionGuids[0]);
                touchInputDefinition = AssetDatabase.LoadAssetAtPath<TouchInputDefinition>(path);
            }

            PlatformGridInputsDefinitionMapping androidMapping = _gridSettings.PlatformGridInputsDefinitionMappings.Find(x => x.RuntimePlatform.Equals(RuntimePlatform.Android));
            if (androidMapping != null && androidMapping.GridInputDefinition == null)
            {
                androidMapping.GridInputDefinition = touchInputDefinition;
            }

            PlatformGridInputsDefinitionMapping iPhonePlayerMapping = _gridSettings.PlatformGridInputsDefinitionMappings.Find(x => x.RuntimePlatform.Equals(RuntimePlatform.IPhonePlayer));
            if (iPhonePlayerMapping != null && iPhonePlayerMapping.GridInputDefinition == null)
            {
                iPhonePlayerMapping.GridInputDefinition = touchInputDefinition;
            }

            PlatformGridInputsDefinitionMapping linuxEditorMapping = _gridSettings.PlatformGridInputsDefinitionMappings.Find(x => x.RuntimePlatform.Equals(RuntimePlatform.LinuxEditor));
            if (linuxEditorMapping != null && linuxEditorMapping.GridInputDefinition == null)
            {
                linuxEditorMapping.GridInputDefinition = mouseInputDefinition;
            }

            PlatformGridInputsDefinitionMapping linuxPlayerMapping = _gridSettings.PlatformGridInputsDefinitionMappings.Find(x => x.RuntimePlatform.Equals(RuntimePlatform.LinuxPlayer));
            if (linuxPlayerMapping != null && linuxPlayerMapping.GridInputDefinition == null)
            {
                linuxPlayerMapping.GridInputDefinition = touchInputDefinition;
            }

            PlatformGridInputsDefinitionMapping OSXEditorMapping = _gridSettings.PlatformGridInputsDefinitionMappings.Find(x => x.RuntimePlatform.Equals(RuntimePlatform.OSXEditor));
            if (OSXEditorMapping != null && OSXEditorMapping.GridInputDefinition == null)
            {
                OSXEditorMapping.GridInputDefinition = mouseInputDefinition;
            }

            PlatformGridInputsDefinitionMapping OSXPlayerMapping = _gridSettings.PlatformGridInputsDefinitionMappings.Find(x => x.RuntimePlatform.Equals(RuntimePlatform.OSXPlayer));
            if (OSXPlayerMapping != null && OSXPlayerMapping.GridInputDefinition == null)
            {
                OSXPlayerMapping.GridInputDefinition = touchInputDefinition;
            }

            PlatformGridInputsDefinitionMapping PS4Mapping = _gridSettings.PlatformGridInputsDefinitionMappings.Find(x => x.RuntimePlatform.Equals(RuntimePlatform.PS4));
            if (PS4Mapping != null && PS4Mapping.GridInputDefinition == null)
            {
                PS4Mapping.GridInputDefinition = touchInputDefinition;
            }

            PlatformGridInputsDefinitionMapping PS5Mapping = _gridSettings.PlatformGridInputsDefinitionMappings.Find(x => x.RuntimePlatform.Equals(RuntimePlatform.PS5));
            if (PS5Mapping != null && PS5Mapping.GridInputDefinition == null)
            {
                PS5Mapping.GridInputDefinition = touchInputDefinition;
            }

            PlatformGridInputsDefinitionMapping StadiaMapping = _gridSettings.PlatformGridInputsDefinitionMappings.Find(x => x.RuntimePlatform.Equals(RuntimePlatform.Stadia));
            if (StadiaMapping != null && StadiaMapping.GridInputDefinition == null)
            {
                StadiaMapping.GridInputDefinition = touchInputDefinition;
            }

            PlatformGridInputsDefinitionMapping SwitchMapping = _gridSettings.PlatformGridInputsDefinitionMappings.Find(x => x.RuntimePlatform.Equals(RuntimePlatform.Switch));
            if (SwitchMapping != null && SwitchMapping.GridInputDefinition == null)
            {
                SwitchMapping.GridInputDefinition = touchInputDefinition;
            }

            PlatformGridInputsDefinitionMapping tvOSMapping = _gridSettings.PlatformGridInputsDefinitionMappings.Find(x => x.RuntimePlatform.Equals(RuntimePlatform.tvOS));
            if (tvOSMapping != null && tvOSMapping.GridInputDefinition == null)
            {
                tvOSMapping.GridInputDefinition = touchInputDefinition;
            }

            PlatformGridInputsDefinitionMapping WindowsEditorMapping = _gridSettings.PlatformGridInputsDefinitionMappings.Find(x => x.RuntimePlatform.Equals(RuntimePlatform.WindowsEditor));
            if (WindowsEditorMapping != null && WindowsEditorMapping.GridInputDefinition == null)
            {
                WindowsEditorMapping.GridInputDefinition = mouseInputDefinition;
            }

            PlatformGridInputsDefinitionMapping WindowsPlayerMapping = _gridSettings.PlatformGridInputsDefinitionMappings.Find(x => x.RuntimePlatform.Equals(RuntimePlatform.WindowsPlayer));
            if (WindowsPlayerMapping != null && WindowsPlayerMapping.GridInputDefinition == null)
            {
                WindowsPlayerMapping.GridInputDefinition = mouseInputDefinition;
            }

            PlatformGridInputsDefinitionMapping XboxOneMapping = _gridSettings.PlatformGridInputsDefinitionMappings.Find(x => x.RuntimePlatform.Equals(RuntimePlatform.XboxOne));
            if (XboxOneMapping != null && XboxOneMapping.GridInputDefinition == null)
            {
                XboxOneMapping.GridInputDefinition = mouseInputDefinition;
            }
        }

        private void DisplayGridSize()
        {
            bool disabled = (_gridSettings.SizingOption != (int)GridSizingMethod.GRID_SIZE_AND_CELL_COUNT && _gridSettings.SizingOption != (int)GridSizingMethod.CELL_SIZE_AND_GRID_SIZE);

            EditorGUI.BeginDisabledGroup(disabled);

            // This check is done before it's set incase the the value was changed by another sizing mode.
            if (_oldGridWidth != _gridSettings.Width)
            {
                _refreshRequired = true;
            }

            _oldGridWidth = _gridSettings.Width;

            _gridSettings.Width = EditorGUILayout.FloatField(label: "Grid Size:", value: (float)_gridSettings.Width, options: null);

            if (_gridSettings.Width <= 0)
            {
                _gridSettings.Width = 1;
            }

            if (_oldGridWidth != _gridSettings.Width)
            {
                _refreshRequired = true;
            }

            EditorGUI.EndDisabledGroup();
        }

        private void DisplayCellSize()
        {
            bool disabled = (_gridSettings.SizingOption != (int)GridSizingMethod.CELL_SIZE_AND_CELL_COUNT && _gridSettings.SizingOption != (int)GridSizingMethod.CELL_SIZE_AND_GRID_SIZE);

            EditorGUI.BeginDisabledGroup(disabled);

            _oldDesiredCellSize = _gridSettings.DesiredCellSize;


            if(disabled)
            {
                EditorGUILayout.FloatField(label: "Cell Size:", value: (float)_gridSettings.CellSize, options: null);
            }
            else
            {
                _gridSettings.DesiredCellSize = EditorGUILayout.FloatField(label: "Cell Size:", value: (float)_gridSettings.DesiredCellSize, options: null);

                if (_gridSettings.DesiredCellSize <= 0)
                {
                    _gridSettings.DesiredCellSize = 1;
                }
            }

            if (_oldDesiredCellSize != _gridSettings.DesiredCellSize && !disabled)
            {
                _refreshRequired = true;
            }

            if (_gridSettings.SizingOption == (int)GridSizingMethod.CELL_SIZE_AND_CELL_COUNT)
            {
                _gridSettings.Width = _gridSettings.DesiredCellSize * _cellCountX;
            }

            EditorGUI.EndDisabledGroup();
        }

        private void DisplayCellCount()
        {
            bool disabled = (_gridSettings.SizingOption != (int)GridSizingMethod.CELL_SIZE_AND_CELL_COUNT && _gridSettings.SizingOption != (int)GridSizingMethod.GRID_SIZE_AND_CELL_COUNT);
            
            EditorGUI.BeginDisabledGroup(disabled);

            _oldCellCountX = _cellCountX;
            _cellCountX = EditorGUILayout.IntField("Horizontal Cell Count: ", _cellCountX);

            if (_cellCountX <= 0)
            {
                _cellCountX = 1;
            }

            if (_oldCellCountX != _cellCountX)
            {
                _refreshRequired = true;
            }

            _oldCellCountY = _cellCountY;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.DoubleField("Vertical Cell Count: ", _cellCountY);
            EditorGUI.EndDisabledGroup();

            if (_oldCellCountY != _cellCountY)
            {
                _refreshRequired = true;
            }

            EditorGUI.EndDisabledGroup();

            if (_gridSettings.SizingOption == (int)GridSizingMethod.CELL_SIZE_AND_GRID_SIZE)
            {
                float numberOfCells = CalculateNumberOfCellsRequred((float)_gridSettings.Width, _gridSettings.DesiredCellSize);

                if (IsWholeNumber(numberOfCells))
                {
                    _cellCountX = (int)numberOfCells;

                    if(_cellCountX != _oldCellCountX) 
                    { 
                        _refreshRequired = true;
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Invalid Cell Size for the Grid Width. The Grid Width must be divisable by the Cell Size", MessageType.Error);
                }
            }
        }

        private float CalculateNumberOfCellsRequred(float gridSize, float cellSize)
        {
            float numberOfCells = gridSize / cellSize;

            return Mathf.Round(numberOfCells * 10000) / 10000;
        }

        private bool IsWholeNumber(float number)
        {
            return number % 1 == 0;
        }
    }
}