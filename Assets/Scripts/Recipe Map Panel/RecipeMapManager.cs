using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class RecipeMapManager : SerializedMonoBehaviour
{
    // Transform Animation
    Transform PanelTransform, OpenPanelTransform, ClosePanelTransform, MiddlePanelTransform;
    bool panelOpen = false;
    public AnimationCurve panelDisplayAC;

    // Map Table
    [TableMatrix(HorizontalTitle = "Square Celled Matrix", SquareCells = true)]
    //public RecipeBlockSO[,] theTable;

    // Recipe Viewer
    GameObject RecipeViewer;

    public RecipeMapBlock DisplayBlock;

    GameObject RecipeUpgradeSet;

    GameObject[] RecipeViewerLevels = new GameObject[4];

    GameObject[] LockIcons = new GameObject[2];

    // Colors
    public Color32 unlockedColor;
    public Color32 lockedColor;
    public Color32 unknownColor;
    public Color32 unlockedPathColor;
    public Color32 lockedPathColor;
    public Color32 unknownPathColor;
    public Color32 unlockedBackgroundColor;
    public Color32 lockedBackgroundColor;

    class ShadowBoxManager
    {
        private Transform parentGO;
        private Transform boxTemplate;
        private List<Transform> allBoxes;
        private float distance = 0.21f;
        public ShadowBoxManager(Transform parent)
        {
            parentGO = parent;
            boxTemplate = parent.transform.GetChild(0);
            allBoxes = new List<Transform>();
        }
        public void GenerateBoxes(List<Vector2> coords)
        {
            HideBoxes();
            boxTemplate.gameObject.SetActive(true);
            foreach (Vector2 c in coords)
            {
                GameObject newGO = Instantiate(boxTemplate.gameObject, parentGO);
                newGO.transform.localPosition += new Vector3(c.x * distance, c.y * distance, 0f);
                allBoxes.Add(newGO.transform);
            }
            boxTemplate.gameObject.SetActive(false);
        }

        public void HideBoxes()
        {
            for (int i = allBoxes.Count - 1; i >= 0; i--)
            {
                Destroy(allBoxes[i].gameObject);
            }
            allBoxes.Clear();
        }
        
    }
    
    // All Blocks
    public List<RecipeMapBlock> allBlocks = new List<RecipeMapBlock>();
    private ShadowBoxManager shadowBoxManager;
    
    
    // Singleton
    static RecipeMapManager instance;
    public static RecipeMapManager i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RecipeMapManager>();
            }
            return instance;
        }
    }

    [OnInspectorInit]
    private void Start()
    {
        // theTable = new RecipeBlockSO[9, 9];

        PanelTransform = transform.Find("Recipe Map Panel");
        OpenPanelTransform = transform.Find("Open Panel Transform");
        ClosePanelTransform = transform.Find("Close Panel Transform");
        MiddlePanelTransform = transform.Find("Middle Panel Transform");

        RecipeViewer = transform.parent.gameObject.transform.Find("===Recipe Viewer===").Find("Recipe Viewer Panel").gameObject;
        RecipeUpgradeSet = RecipeViewer.transform.Find("Upgrade Set").gameObject;

        RecipeViewerLevels[0] = RecipeViewer.transform.Find("Name").gameObject;
        RecipeViewerLevels[1] = RecipeViewer.transform.Find("Materials").gameObject;
        RecipeViewerLevels[2] = RecipeViewer.transform.Find("Description").gameObject;
        RecipeViewerLevels[3] = RecipeViewer.transform.Find("Graph").gameObject;

        LockIcons[0] = RecipeViewer.transform.Find("Lock 1").gameObject;
        LockIcons[1] = RecipeViewer.transform.Find("Lock 2").gameObject;
        
        shadowBoxManager = new ShadowBoxManager(RecipeViewer.transform.Find("Shadow Box Manager"));
    }


    // Check if there is locked/unknown recipe block. Is true, unlock it and discovered adjacent recipe blocks
    public void CheckUnlock(ItemScriptableObject itemToCheck) {
        foreach (RecipeMapBlock block in allBlocks)
        {
            if (block.myISO == itemToCheck)
            {
                block.RecipeUnlock();
            } 
        }
    }

    public void DisplayRecipe(RecipeMapBlock RMB)
    {
        DisplayBlock = RMB;

        RecipeViewer.transform.Find("Name").GetComponent<TextMeshPro>().text = 
            RMB.name; //+ " (" + DisplayBlock.GetLevelString()+ ")";

        RecipeUpgradeSet.SetActive(true);

        for (int count = 0; count < RecipeViewerLevels.Length; count ++)
        {
            RecipeViewerLevels[count].SetActive(false);
            if (count + 1 <= RMB.GetLevelInt())
            {
                RecipeViewerLevels[count].SetActive(true);
            }
        }

        LockIcons[0].SetActive(true);
        LockIcons[1].SetActive(true);

        if (RMB.GetLevelInt() >= 1) {
            // Dont do shit, since name is already set up there

            RecipeUpgradeSet.transform.position = RecipeViewerLevels[1].transform.position;
        }
        if (RMB.GetLevelInt() >= 2)
        {
            LockIcons[0].SetActive(false);

            RecipeViewerLevels[1].GetComponent<TextMeshPro>().text =
                RMB.material;

            RecipeUpgradeSet.transform.position = RecipeViewerLevels[2].transform.position;
        }
        if (RMB.GetLevelInt() >= 3)
        {
            LockIcons[1].SetActive(false);

            RecipeViewerLevels[2].GetComponent<TextMeshPro>().text =
                RMB.craftDescription;

            RecipeUpgradeSet.transform.position = RecipeViewerLevels[3].transform.position;
        }
        if (RMB.GetLevelInt() >= 4)
        {
            RecipeViewerLevels[3].GetComponent<TextMeshPro>().text = "Shape:";
            shadowBoxManager.GenerateBoxes(RMB.myICSO.GetDefaultRecipeCoords());
            //RecipeViewerLevels[3].transform.position
        }

        if (RMB.GetLevelInt() == 4)
        {
            RecipeUpgradeSet.SetActive(false);
        }
        else {
            shadowBoxManager.HideBoxes();
            RecipeUpgradeSet.transform.Find("Spirit Point Amount").GetComponent<TextMeshPro>().text =
                RMB.CurrentCost().ToString();
            RecipeUpgradeSet.SetActive(true);
        }
    }

    public void StopDisplayRecipe()
    {
        DisplayBlock = null;
        RecipeViewer.SetActive(false);
    }

    public void RecipeUpgrade()
    {
        if (SpiritPoint.i.Use(DisplayBlock.CurrentCost()))
        {
            Debug.Log("upgrade" + DisplayBlock.name);
            DisplayBlock.RecipeUpgrade();

            DisplayRecipe(DisplayBlock);
        }
    }

    private void Update()
    {
        foreach (RecipeMapBlock block in allBlocks)
        {
            //Debug.Log(Mathf.Abs(block.transform.position.x - block.recipeMap.transform.position.x));
            //Debug.Log(Mathf.Abs(block.transform.position.y - block.recipeMap.transform.position.y));
            if (Mathf.Abs(block.transform.position.x - block.recipeMap.transform.position.x) >= 8.5)
            {
                block.gameObject.SetActive(false);
            }
            else if (Mathf.Abs(block.transform.position.y - block.recipeMap.transform.position.y) >= 3.5)
            {
                block.gameObject.SetActive(false);
            }
            else
            {
                block.gameObject.SetActive(true);
            }
        }
    }

    private void MoveToMiddle()
    {
        PanelTransform.localPosition = MiddlePanelTransform.localPosition;
    }

    private void MoveToClose()
    {
        PanelTransform.localPosition = ClosePanelTransform.localPosition;
    }

    public void OpenPanel()
    {
        StartCoroutine(OpenPanelCor());
    }

    public void ClosePanel()
    {
        StartCoroutine(ClosePanelCor());
    }

    IEnumerator OpenPanelCor()
    {
        MoveToMiddle();
        panelOpen = true;
        float timeNeed = 0.5f, timeCount = 0f;
        while (timeCount < timeNeed)
        {
            PanelTransform.localPosition = Vector3.Lerp(MiddlePanelTransform.localPosition, OpenPanelTransform.localPosition, panelDisplayAC.Evaluate(Mathf.Clamp(timeCount / timeNeed, 0f, 1f)));
            timeCount += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ClosePanelCor()
    {
        panelOpen = false;
        float timeNeed = 0.5f, timeCount = 0f;
        while (timeCount < timeNeed)
        {
            PanelTransform.localPosition = Vector3.Lerp(OpenPanelTransform.localPosition, MiddlePanelTransform.localPosition, panelDisplayAC.Evaluate(Mathf.Clamp(timeCount / timeNeed, 0f, 1f)));
            timeCount += Time.deltaTime;
            yield return null;
            MoveToClose();
        }
    }
}
