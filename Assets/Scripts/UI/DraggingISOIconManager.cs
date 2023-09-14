using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggingISOIconManager : MonoBehaviour
{
    static DraggingISOIconManager instance;
    public static DraggingISOIconManager i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DraggingISOIconManager>();
            }
            return instance;
        }
    }
    
    [SerializeField]private GameObject template;
    private GameObject draggingGameObject = null;

    public void CreateDraggingISOIcon(ItemScriptableObject iso)
    {
        UI_FullScreenUIDragCollider.i.Open(this);
        draggingGameObject = Instantiate(template,transform);
        draggingGameObject.SetActive(true);
        draggingGameObject.GetComponent<SpriteRenderer>().sprite = iso.iconSprite;
    }

    public void DeleteDraggingISOIcon()
    {
        UI_FullScreenUIDragCollider.i.Close();
        draggingGameObject = null;
    }

    public bool IsDragging()
    {
        return draggingGameObject != null;
    }
}
