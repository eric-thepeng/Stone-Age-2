//using UnityEditor.Experimental.GraphView;
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
    private ItemScriptableObject draggingIso = null;

    public void CreateDraggingISOIcon(ItemScriptableObject iso)
    {
        UI_FullScreenUIDragCollider.i.Open(this);
        draggingGameObject = Instantiate(template,transform);
        draggingGameObject.SetActive(true);
        draggingIso = iso;
        draggingGameObject.GetComponent<SpriteRenderer>().sprite = draggingIso.iconSprite;
    }

    public void DeleteDraggingISOIcon()
    {
        Destroy(draggingGameObject);
        draggingGameObject = null;
        UI_FullScreenUIDragCollider.i.Close();
        if (WorldUtility.TryMouseHitPoint(WorldUtility.LAYER.WORLD_INTERACTABLE, true))
        {
            WorldUtility.GetMouseHitObject(WorldUtility.LAYER.WORLD_INTERACTABLE, true).GetComponent<UI_ISOIconDisplayBox>()?.Display(draggingIso,true);
        }
        draggingIso = null;
    }

    public bool IsDragging()
    {
        return draggingGameObject != null;
    }
}
