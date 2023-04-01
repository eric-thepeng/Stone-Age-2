using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;

public class CraftingManager : SerializedMonoBehaviour
{
    [SerializeField] public GameObject mergeProgressBar;
    [SerializeField] Transform inventoryFlyToTarget;
    [SerializeField] public Transform testtesttest;
    [SerializeField] List<ItemScriptableObject> startingTetris = new List<ItemScriptableObject>();

    public List<GameObject> allTetris = new List<GameObject>();
    float unitLength = 0.15f;

    public Transform PanelTransform, OpenPanelTransform, ClosePanelTransform;
    bool panelOpen = false;
    public AnimationCurve panelDisplayAC;

    public enum CreateFrom {DEBUG, INVENTORY, MERGE}

    static CraftingManager instance;
    public static CraftingManager i
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<CraftingManager>();
            }
            return instance;
        }
    }

    private void Start()
    {
        PanelTransform = transform.Find("Crafting Panel");
        OpenPanelTransform = transform.Find("Open Panel Transform");
        ClosePanelTransform = transform.Find("Close Panel Transform");
        foreach(ItemScriptableObject iso in startingTetris)
        {
            CreateTetris(iso, PanelTransform.position,CreateFrom.DEBUG);
        }
    }

    public void OpenPanel()
    {
        StartCoroutine(OpenPanelCor());
    }

    public void ClosePanel()
    {
        StartCoroutine(ClosePanelCor());
    }

    IEnumerator OpenPanelCor()
    {
        panelOpen = true;
        UI_InventoryPanel.i.OpenPanel();
        float timeNeed = 0.5f, timeCount = 0f;
        while (timeCount<timeNeed)
        {
            PanelTransform.localPosition = Vector3.Lerp(ClosePanelTransform.localPosition, OpenPanelTransform.localPosition, panelDisplayAC.Evaluate(Mathf.Clamp(timeCount/timeNeed,0f,1f)));
            timeCount += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ClosePanelCor()
    {
        panelOpen = false;
        UI_InventoryPanel.i.ClosePanel();
        float timeNeed = 0.5f, timeCount = 0f;
        while (timeCount < timeNeed)
        {
            PanelTransform.localPosition = Vector3.Lerp(OpenPanelTransform.localPosition, ClosePanelTransform.localPosition, panelDisplayAC.Evaluate(Mathf.Clamp(timeCount / timeNeed, 0f, 1f)));
            timeCount += Time.deltaTime;
            yield return null;
        }
        PutBackAllTetrisToInventory();
    }


    /*
    public void AddToCrafting(GameObject go)
    {
        GameObject newTetris = Instantiate(go,transform.Find("Crafting Panel"));
        newTetris.transform.localPosition= Vector3.zero;
        newTetris.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
        allTetris.Add(newTetris);
    }*/

    public bool TetrisInPresent(ItemScriptableObject iso)
    {
        foreach(GameObject go in allTetris)
        {
            if (go.GetComponent<Tetris>().itemSO == iso) return true;
        }
        return false;
    }

    public GameObject CreateTetris(ItemScriptableObject iso, Vector3 addPosition, CreateFrom createFrom)
    {
        GameObject newTetris = Instantiate(iso.myPrefab, transform.Find("Crafting Panel"));
        newTetris.transform.position = addPosition;
        newTetris.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        newTetris.name = newTetris.name + " " + Random.Range(1000, 9999);
        allTetris.Add(newTetris);
        if (createFrom == CreateFrom.DEBUG)
        {
            Inventory.i.AddInventoryItem(iso);
        }
        else if (createFrom == CreateFrom.INVENTORY)
        {
            Inventory.i.InUseItem(iso, true);
        }
        else //(createFrom == CreateFrom.MERGE)
        {
            Inventory.i.MergeCreateItem(iso);
        }
        
        return newTetris;
    }

    public void TetrisFlyToInventoryEffect(GameObject tetrisGO, float flyTime)
    {
        Tetris origionalTetris = tetrisGO.GetComponent<Tetris>();
        
        GameObject flyObject = Instantiate(new GameObject("flying " + tetrisGO),tetrisGO.transform.parent);
        flyObject.AddComponent<SpriteRenderer>();
        flyObject.GetComponent<SpriteRenderer>().sprite = tetrisGO.GetComponent<SpriteRenderer>().sprite;
        flyObject.GetComponent<SpriteRenderer>().sortingLayerID = tetrisGO.GetComponent<SpriteRenderer>().sortingLayerID;
        flyObject.GetComponent<SpriteRenderer>().sortingOrder = tetrisGO.GetComponent<SpriteRenderer>().sortingOrder;
        flyObject.transform.localScale = tetrisGO.transform.localScale;
        flyObject.transform.localPosition = tetrisGO.transform.localPosition;
        
        Tween t = flyObject.transform.DOMove(inventoryFlyToTarget.position, flyTime);
        t.onComplete = () => {
            Inventory.i.InUseItem(origionalTetris.itemSO, false);
            Destroy(flyObject); 
        };
    }

    public void RemoveFromTetrisList(GameObject go)
    {
        allTetris.Remove(go);
    }

    public void PutBackTetrisToInventory(GameObject go, bool playerDragBack = false)
    {
        RemoveFromTetrisList(go);
        if (playerDragBack)
        {
            TetrisFlyToInventoryEffect(go, 0.2f);
        }
        else
        {
            TetrisFlyToInventoryEffect(go, 0.4f);
        }
        Destroy(go);
    }

    public void PutBackAllTetrisToInventory()
    {
        for(int i = allTetris.Count-1; i>=0; i--)
        {
            allTetris[i].GetComponent<Tetris>().DestroyRC();
            if(! allTetris[i].GetComponent<Tetris>().isStaticCraftStation) PutBackTetrisToInventory(allTetris[i]);
        }
    }

    public int CheckAmountISO(ItemScriptableObject toCheck)
    {
        int output = 0;

        foreach(GameObject go in allTetris)
        {
            if (go.GetComponent<Tetris>().itemSO == toCheck) output += 1;
        }

        return output;
    }

    public void mouseEnterTetris(ItemScriptableObject iso)
    {
        Vector3 toSet = WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.UI_BACKGROUND, true); //new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, PanelTransform.Find("NameUI").transform.position.z);
        PanelTransform.Find("NameUI").transform.position = toSet + new Vector3(0,0.1f,0.1f);
        PanelTransform.Find("NameUI").gameObject.SetActive(true);
        PanelTransform.Find("NameUI").gameObject.GetComponentInChildren<TextMeshPro>().text = iso.tetrisHoverName;
        PanelTransform.Find("NameUI").gameObject.GetComponentInChildren<TextMeshPro>().sortingLayerID = PanelTransform.Find("NameUI").GetComponentInChildren<SpriteRenderer>().sortingLayerID;
        PanelTransform.Find("NameUI").gameObject.GetComponentInChildren<TextMeshPro>().sortingOrder = PanelTransform.Find("NameUI").GetComponentInChildren<SpriteRenderer>().sortingOrder+1;

    }
    public void mouseClickTetris()
    {
        PanelTransform.Find("NameUI").gameObject.SetActive(false);
    }
    public void mouseExitTetris()
    {
        PanelTransform.Find("NameUI").gameObject.SetActive(false);
    }

    public void mouseEnterInventoryBlock(UI_InventoryBlock uiib)
    {
        UI_Inventory.i.DisplayItemDetail(uiib.GetISO());
    }
    public void mouseExitInventoryBlock()
    {
        UI_Inventory.i.CancelDisplayItemDetail();
    }


    public bool isPanelOpen()
    {
        return panelOpen;
    }

    public GameObject CreateMergeWindow(RecipeCombinator rc)
    {
        Transform tf = transform.Find("Crafting Panel").Find("Merge Windows");

        GameObject newWindow = Instantiate(tf.Find("Merge Window Template").gameObject, tf);
        newWindow.SetActive(true);
        newWindow.transform.position = rc.CentralPosition();
        
        UnityEvent mergeEvent = new UnityEvent();
        mergeEvent.AddListener(rc.Merge);
        newWindow.transform.Find("Merge Button").gameObject.GetComponent<WorldSpaceButton>().SetClickEvent(mergeEvent);

        UnityEvent deleteEvent = new UnityEvent();
        deleteEvent.AddListener(rc.DeleteMergeWindow);
        newWindow.transform.Find("Delete Button").gameObject.GetComponent<WorldSpaceButton>().SetClickEvent(deleteEvent);


        newWindow.transform.Find("Preview").GetComponent<SpriteRenderer>().sprite = rc.GetMergeISO().iconSprite;
        newWindow.transform.Find("Preview Name").GetComponent<TextMeshPro>().text = rc.GetMergeISO().tetrisHoverName;

        return newWindow;
    }

}
