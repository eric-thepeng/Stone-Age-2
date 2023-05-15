using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class UI_Quest : MonoBehaviour
{

    static UI_Quest instance;
    public static UI_Quest i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_Quest>();
            }
            return instance;
        }
    }

    TextMeshPro questNameText;
    TextMeshPro questDescriptionText;
    Transform background;

    private void Start()
    {
        questNameText = transform.Find("Quest Name").GetComponent<TextMeshPro>();
        questDescriptionText = transform.Find("Quest Description").GetComponent<TextMeshPro>();
        background = transform.Find("Quest UI Background");
    }

    public void SetUpDisplay(Quest q)
    {
        questNameText.text = q.GetName();
        questDescriptionText.text = q.GetDescription();
        OpenAll();
    }

    public void CloseDisplay(Quest q)
    {
        CloseAll();
    }

    void OpenAll()
    {
        questNameText.gameObject.SetActive(true);
        questDescriptionText.gameObject.SetActive(true);
        background.gameObject.SetActive(true);
    }

    void CloseAll()
    {
        questNameText.gameObject.SetActive(false);
        questDescriptionText.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
    }
}
