using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    static CanvasManager instance;
    public static CanvasManager i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CanvasManager>();
            }
            return instance;
        }
    }

    private void Start()
    {
        CloseTargetInfo();
    }

    public void CloseTargetInfo()
    {
        transform.Find("TargetInfo").gameObject.SetActive(false);
    }
    public void SetUpTargetInfo(string info)
    {
        transform.Find("TargetInfo").gameObject.SetActive(true);
        transform.Find("TargetInfo").Find("Text").GetComponent<TMP_Text>().text = info;
    }

    public void TutorialUIDisplay(bool state)
    {
        if(state == true)
        {
            transform.Find("UI_Tutorial").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("UI_Tutorial").gameObject.SetActive(false);
        }
    }
}
