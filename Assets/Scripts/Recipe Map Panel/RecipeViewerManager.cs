using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeViewerManager : MonoBehaviour
{
    // Transform Animation
    Transform PanelTransform, OpenPanelTransform, ClosePanelTransform, HidePanelTransform;
    bool panelOpen = false;
    public AnimationCurve panelDisplayAC;

    // Singleton
    static RecipeViewerManager instance;
    public static RecipeViewerManager i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RecipeViewerManager>();
            }
            return instance;
        }
    }

    void Start()
    {
        PanelTransform = transform.Find("Recipe Viewer Panel");
        OpenPanelTransform = transform.Find("Open Panel Transform");
        ClosePanelTransform = transform.Find("Close Panel Transform");
        HidePanelTransform = transform.Find("Hide Panel Transform");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            if (panelOpen)
            {
                ClosePanel();
            }
            else
            {
                OpenPanel();
            }
        }
    }

    public void OpenPanel()
    {
        StartCoroutine(ToOpenCor());
    }

    public void ClosePanel()
    {
        StartCoroutine(ToCloseCor());
    }

    public void HidePanel()
    {
        StartCoroutine(ToHideCor());
    }

    IEnumerator ToOpenCor()
    {
        Vector3 origionPosition = PanelTransform.position;
        panelOpen = true;
        float timeNeed = 0.5f, timeCount = 0f;
        while (timeCount < timeNeed)
        {
            PanelTransform.position = Vector3.Lerp(origionPosition, OpenPanelTransform.position, panelDisplayAC.Evaluate(Mathf.Clamp(timeCount / timeNeed, 0f, 1f)));
            timeCount += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ToCloseCor()
    {
        Vector3 origionPosition = PanelTransform.position;
        panelOpen = false;
        float timeNeed = 0.5f, timeCount = 0f;
        while (timeCount < timeNeed)
        {
            PanelTransform.position = Vector3.Lerp(origionPosition, ClosePanelTransform.position, panelDisplayAC.Evaluate(Mathf.Clamp(timeCount / timeNeed, 0f, 1f)));
            timeCount += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ToHideCor()
    {
        Vector3 origionPosition = PanelTransform.position;
        panelOpen = false;
        float timeNeed = 0.5f, timeCount = 0f;
        while (timeCount < timeNeed)
        {
            PanelTransform.position = Vector3.Lerp(origionPosition, HidePanelTransform.position, panelDisplayAC.Evaluate(Mathf.Clamp(timeCount / timeNeed, 0f, 1f)));
            timeCount += Time.deltaTime;
            yield return null;
        }
    }
}
