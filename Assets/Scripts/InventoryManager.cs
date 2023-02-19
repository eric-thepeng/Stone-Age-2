using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    Transform PanelTransform, OpenPanelTransform, ClosePanelTransform;
    bool panelOpen = false;
    public AnimationCurve panelDisplayAC;

    static InventoryManager instance;
    public static InventoryManager i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InventoryManager>();
            }
            return instance;
        }
    }

    private void Start()
    {
        PanelTransform = transform.Find("Inventory Panel");
        OpenPanelTransform = transform.Find("Open Panel Transform");
        ClosePanelTransform = transform.Find("Close Panel Transform");
    }


    private void Update()
    {
        //print(WorldUtility.GetMouseHitPoint());
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (panelOpen)
            {
                if(!CraftingManager.i.isPanelOpen()) StartCoroutine(ClosePanel());
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

    public void OpenPanelIfNot() 
    {
        if (!panelOpen)
        {
            StartCoroutine(OpenPanel());
        }
    }

    public bool isPanelOpen()
    {
        return panelOpen;
    }
}
