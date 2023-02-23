using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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
    public static RecipeMapManager instance;

    // Colors
    public Color32 unlockedColor;
    public Color32 lockedColor;
    public Color32 unknownColor;
    public Color32 unlockedPathColor;
    public Color32 lockedPathColor;
    public Color32 unknownPathColor;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (panelOpen)
            {
                StartCoroutine(ClosePanel());
            }
            else
            {
                StartCoroutine(OpenPanel());
            }
        }
    }

    private void MoveToMiddle() {
        PanelTransform.localPosition = MiddlePanelTransform.localPosition;
    }
    private void MoveToClose()
    {
        PanelTransform.localPosition = ClosePanelTransform.localPosition;
    }

    IEnumerator OpenPanel()
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

    IEnumerator ClosePanel()
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
