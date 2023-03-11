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

    // Singleton
    public static RecipeMapManager i;

    // Viewer
    GameObject RecipeViewer;
    RecipeMapBlock DisplayBlock;
    GameObject RecipeViewerButton;

    // Colors
    public Color32 unlockedColor;
    public Color32 lockedColor;
    public Color32 unknownColor;
    public Color32 unlockedPathColor;
    public Color32 lockedPathColor;
    public Color32 unknownPathColor;
    public Color32 unlockedBackgroundColor;
    public Color32 lockedBackgroundColor;

    // All Blocks
    public List<RecipeMapBlock> allBlocks = new List<RecipeMapBlock>();

    // Singleton
    void Awake()
    {
        if (i == null)
        {
            i = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [OnInspectorInit]
    private void Start()
    {
        //theTable = new RecipeBlockSO[9, 9];

        PanelTransform = transform.Find("Recipe Map Panel");
        OpenPanelTransform = transform.Find("Open Panel Transform");
        ClosePanelTransform = transform.Find("Close Panel Transform");
        MiddlePanelTransform = transform.Find("Middle Panel Transform");

        RecipeViewer = transform.Find("Recipe Map Panel").transform.Find("Recipe Block Viewer").gameObject;
        RecipeViewerButton = transform.Find("Recipe Map Panel").transform.Find("Recipe Block Viewer").transform.Find("Upgrade Button").gameObject;
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Hit the R key");
            if (panelOpen)
            {
                StartCoroutine(ClosePanelCor());
            }
            else
            {
                StartCoroutine(OpenPanelCor());
            }
        }*/

        if (RecipeViewer.activeSelf)
        {
            if (!WorldUtility.TryMouseHitPoint(WorldUtility.LAYER.RECIPE_BLOCK_VIEWER, true))
            {
                StopDisplayRecipe();
            }
        }
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
        RecipeViewer.SetActive(true);

        RecipeViewer.transform.localPosition = RMB.transform.localPosition + new Vector3(0, 2, 0);

        RecipeViewer.transform.Find("Name").GetComponent<TextMeshPro>().text = 
            RMB.name; //+ " (" + DisplayBlock.GetLevelString()+ ")";

        if (RMB.GetLevelInt() == 1) {
            RecipeViewer.transform.Find("Description").GetComponent<TextMeshPro>().text =
                "Upgrade the recipe to learn more about it!";
        }
        else if (RMB.GetLevelInt() == 2)
        {
            RecipeViewer.transform.Find("Description").GetComponent<TextMeshPro>().text =
                "Materials required: " + RMB.material;
        }
        else if (RMB.GetLevelInt() == 3)
        {
            RecipeViewer.transform.Find("Description").GetComponent<TextMeshPro>().text =
                "Materials required: " + RMB.material + "<br>" +
                "To craft: " + RMB.craftDescription;
        }
        else if (RMB.GetLevelInt() == 4)
        {
            RecipeViewer.transform.Find("Description").GetComponent<TextMeshPro>().text =
                "Materials required: " + RMB.material + "<br>" +
                "To craft: " + RMB.craftDescription;
        }

        if (RMB.GetLevelInt() == 4)
        {
            RecipeViewer.transform.Find("Cost").GetComponent<TextMeshPro>().text =
                "This recipe has reached the maximum level.";
            RecipeViewerButton.SetActive(false);
        }
        else {
            RecipeViewer.transform.Find("Cost").GetComponent<TextMeshPro>().text =
                "Recipe Upgrade Cost: " + RMB.CurrentCost();
            RecipeViewerButton.SetActive(true);
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

    // Currently not using
    public bool CheckCost() 
    {
        return true;

        if (DisplayBlock.baseCost < 31245)
        {
            return false;
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
