using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement
{
    public class GridDisplayManager : MonoBehaviour
    {
        public bool IsDisplaying
        {

            get
            {
                GameObject currentGrid = GameObject.Find("Placement Grid Canvas " + GridSettings.Key);
                return currentGrid != null;
            }
        }

        public Transform GridTransform
        {
            get
            {
                return _gridCanvasRectTransform;
            }
        }

        /// <summary>
        /// The image that displays the tiled grid
        /// </summary>
        private Image _gridImage;

        private int _gridCellDisplayImageSize = 10;

        private Canvas _gridCanvas;

        private SpriteRenderer _gridSpriteRenderer;
        private RectTransform _gridCanvasRectTransform;

        public GridSettings GridSettings { get; private set; }

        public GameObject GridDisplay { get; private set; }

        // The grid manager won't be set if setup was called from the GridSettings editor.
        private GridManager _gridManager;

        private float _runtimeRotation => _gridManager != null ? _gridManager.RuntimeGridRotation : GridSettings.GridRotation;

        private Vector3 _runtimePosition => _gridManager != null ? _gridManager.RuntimeGridPosition : GridSettings.GridPosition;

        public void Setup(GridManager gridManager, GridSettings gridSettings)
        {
            _gridManager = gridManager;
            GridSettings = gridSettings;
        }

        public void Display()
        {
            GenerateGrid(GridSettings);
        }

        public void UpdateRotation(float rotation)
        {
            _gridCanvasRectTransform.rotation = Quaternion.Euler(90, rotation, 0);
        }

        private void GenerateGrid(GridSettings gridSettings)
        {
            DestroyExistingGrid();
            GridDisplay = GenerateGridCanvas(gridSettings);
            GenerateGridBackground(GridSettings, GridDisplay.transform);
        }

        private void DestroyExistingGrid()
        {
            if(GridSettings == null)
            {
                return;
            }

            GameObject currentGrid = GameObject.Find("Placement Grid Canvas " + GridSettings.Key);

            if (currentGrid != null)
            {
                DestroyImmediate(currentGrid);
            }
        }

        private GameObject GenerateGridCanvas(GridSettings gridSettings)
        {
            GameObject canvasGameObject = new GameObject("Placement Grid Canvas " + gridSettings.Key);

            canvasGameObject.layer = LayerMask.NameToLayer("Grid");

            _gridCanvas = canvasGameObject.AddComponent<Canvas>();

            Camera camera = GridUtilities.GetCameraForGrid(gridSettings);
            _gridCanvas.worldCamera = camera;

            _gridCanvasRectTransform = canvasGameObject.GetComponent<RectTransform>();
            _gridCanvasRectTransform.sizeDelta = new Vector2((float)gridSettings.Width * gridSettings.CellSize, (float)gridSettings.Height * gridSettings.CellSize);
            _gridCanvasRectTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            _gridCanvasRectTransform.rotation = Quaternion.Euler(90, _runtimeRotation, 0);
            _gridCanvasRectTransform.localPosition = _runtimePosition;

            AddCanvasScaler(canvasGameObject);

            return canvasGameObject;
        }

        //private void GenerateGridBackground(GridSettings gridSettings, Transform gridTransform)
        //{
        //    GameObject backgroundImage = new GameObject("Background Image");
        //    backgroundImage.transform.SetParent(gridTransform, false);

        //    backgroundImage.layer = LayerMask.NameToLayer("Grid");

        //    RectTransform backGroundImageRectTransform = backgroundImage.AddComponent<RectTransform>();
        //    backGroundImageRectTransform.anchorMin = Vector2.zero;
        //    backGroundImageRectTransform.anchorMax = Vector2.one;
        //    backGroundImageRectTransform.localPosition = Vector3.zero;
        //    backGroundImageRectTransform.localScale = Vector3.one;
        //    backGroundImageRectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        //    backGroundImageRectTransform.sizeDelta = Vector2.zero;

        //    _gridImage = backgroundImage.AddComponent<Image>();
        //    _gridImage.sprite = GridSettings.CellImage;
        //    _gridImage.type = Image.Type.Tiled;

        //    SetGridSize(gridSettings);
        //}


        private void GenerateGridBackground(GridSettings gridSettings, Transform gridTransform)
        {
            GameObject backgroundImage = new GameObject("Background Image");
            backgroundImage.transform.SetParent(gridTransform, false);

            backgroundImage.layer = LayerMask.NameToLayer("Grid");

            _gridSpriteRenderer = backgroundImage.AddComponent<SpriteRenderer>();
            _gridSpriteRenderer.sprite = GridSettings.CellImage;
            _gridSpriteRenderer.drawMode = SpriteDrawMode.Tiled;

            _gridSpriteRenderer.size = new Vector2(GridSettings.AmountOfCellsX, GridSettings.AmountOfCellsY);
            _gridSpriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

            //RectTransform backGroundImageRectTransform = backgroundImage.AddComponent<RectTransform>();
            //backGroundImageRectTransform.anchorMin = Vector2.zero;
            //backGroundImageRectTransform.anchorMax = Vector2.one;
            //backGroundImageRectTransform.localPosition = Vector3.zero;
            //backGroundImageRectTransform.localScale = Vector3.one;
            //backGroundImageRectTransform.localRotation = Quaternion.Euler(0, 0, 0);
            //backGroundImageRectTransform.sizeDelta = Vector2.zero;

            //_gridImage = backgroundImage.AddComponent<Image>();
            //_gridImage.sprite = GridSettings.CellImage;
            //_gridImage.type = Image.Type.Tiled;
            _gridSpriteRenderer.gameObject.transform.localScale = new Vector3(gridSettings.CellSize*10, gridSettings.CellSize * 10, gridSettings.CellSize * 10);

            //SetGridSize(gridSettings);

        }

        private void SetGridSize(GridSettings gridSettings)
        {
            _gridCanvasRectTransform.sizeDelta = (new Vector2((float)gridSettings.Width, (float)gridSettings.Height) * _gridCellDisplayImageSize);

            // Tile amount
            float tileMultiplier = (float)1 / gridSettings.CellSize;
            _gridImage.pixelsPerUnitMultiplier = tileMultiplier * 10;
        }

        private void AddCanvasScaler(GameObject canvasGameObject)
        {
            CanvasScaler canvasScaler = canvasGameObject.AddComponent<CanvasScaler>();

            var cellImageSize = GridSettings.CellImage.bounds.size;
            canvasScaler.referencePixelsPerUnit = 100 / cellImageSize.x;
        }

        public void Hide()
        {
            DestroyExistingGrid();
        }

        public void UpdateGridPosition(Vector3 position)
        {
            _gridCanvasRectTransform.localPosition = position;
        }
    }
}