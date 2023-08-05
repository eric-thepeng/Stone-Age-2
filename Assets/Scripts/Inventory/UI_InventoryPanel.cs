using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_InventoryPanel : MonoBehaviour
{
    public AnimationCurve panelDisplayAC;

    //Dependencies
    [SerializeField, Header("---Dependencies, DO NOT CHANGE---")] Transform PanelTransform;
    [SerializeField] Transform OpenPanelTransform;
    [SerializeField] Transform ClosePanelTransform;
    
    //Private Variables
    bool panelOpen = false;
    
    static UI_InventoryPanel instance;
    public static UI_InventoryPanel i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_InventoryPanel>();
            }
            return instance;
        }
    }
    
    IEnumerator OpenPanelCor()
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

    IEnumerator ClosePanelCor()
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

    public void OpenPanel() 
    {
        if (panelOpen == false)
        {
            StartCoroutine(OpenPanelCor());
        }
    }

    public void ClosePanel()
    {
        if (panelOpen == true)
        {
            StartCoroutine(ClosePanelCor());
        }
    }

    public bool isPanelOpen()
    {
        return panelOpen;
    }
}
