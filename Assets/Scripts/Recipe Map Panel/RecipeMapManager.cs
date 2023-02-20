using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RecipeMapManager : SerializedMonoBehaviour
{
    Transform PanelTransform, OpenPanelTransform, ClosePanelTransform;
    bool panelOpen = false;
    public AnimationCurve panelDisplayAC;

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

    private void Start()
    {
        PanelTransform = transform.Find("Recipe Map Panel");
        OpenPanelTransform = transform.Find("Open Panel Transform");
        ClosePanelTransform = transform.Find("Close Panel Transform");
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

    IEnumerator OpenPanel()
    {
        panelOpen = true;
        float timeNeed = 0.5f, timeCount = 0f;
        while (timeCount < timeNeed)
        {
            PanelTransform.localPosition = Vector3.Lerp(ClosePanelTransform.localPosition, OpenPanelTransform.localPosition, panelDisplayAC.Evaluate(Mathf.Clamp(timeCount / timeNeed, 0f, 1f)));
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
            PanelTransform.localPosition = Vector3.Lerp(OpenPanelTransform.localPosition, ClosePanelTransform.localPosition, panelDisplayAC.Evaluate(Mathf.Clamp(timeCount / timeNeed, 0f, 1f)));
            timeCount += Time.deltaTime;
            yield return null;
        }
    }
}
