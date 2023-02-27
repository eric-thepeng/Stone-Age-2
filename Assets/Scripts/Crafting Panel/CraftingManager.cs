using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class CraftingManager : SerializedMonoBehaviour
{
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

    private void Update()
    {
        //print(WorldUtility.GetMouseHitPoint());
        if (Input.GetKeyDown(KeyCode.Space))
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
        UI_InventoryPanel.i.OpenPanel();
        float timeNeed = 0.5f, timeCount = 0f;
        while (timeCount<timeNeed)
        {
            PanelTransform.localPosition = Vector3.Lerp(ClosePanelTransform.localPosition, OpenPanelTransform.localPosition, panelDisplayAC.Evaluate(Mathf.Clamp(timeCount/timeNeed,0f,1f)));
            timeCount += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ClosePanel()
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

    public GameObject CreateTetris(ItemScriptableObject iso, Vector3 addPosition, CreateFrom createFrom)
    {
        GameObject newTetris = Instantiate(iso.myPrefab, transform.Find("Crafting Panel"));
        newTetris.transform.position = addPosition;
        newTetris.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
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

    public void RemoveFromTetrisList(GameObject go)
    {
        allTetris.Remove(go);
    }

    public void PutBackToInventory(GameObject go)
    {
        RemoveFromTetrisList(go);
        Inventory.i.InUseItem(go.GetComponent<Tetris>().itemSO, false);
        Destroy(go);
    }

    public void PutBackAllTetrisToInventory()
    {
        for(int i = allTetris.Count-1; i>=0; i--)
        {
            PutBackToInventory(allTetris[i]);
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
    public bool isPanelOpen()
    {
        return panelOpen;
    }

}
