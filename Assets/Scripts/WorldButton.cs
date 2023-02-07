using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using  UnityEngine.SceneManagement;


public class WorldButton : MonoBehaviour
{
    [SerializeField] int ID;
    private void OnMouseEnter()
    {
        GetComponent<TextMeshPro>().color = new Color(0.7f, 0.7f, 0.7f);
    }

    private void OnMouseDown()
    {
        GetComponent<TextMeshPro>().color = new Color(0.4f, 0.4f, 0.4f);
    }

    private void OnMouseExit()
    {
        GetComponent<TextMeshPro>().color = new Color(1f, 1f, 1f);
    }
    private void OnMouseUpAsButton()
    {
        if(ID == 0)
        {
            SceneManager.LoadScene(sceneName: "Opening Anime");
        }
        else if(ID == 1)
        {
            Application.Quit();
        }
    }
}
