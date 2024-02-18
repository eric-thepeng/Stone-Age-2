using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    static TooltipManager instance;
    public static TooltipManager i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TooltipManager>();
            }
            return instance;
        }
    }

    public class Tooltip
    {
        ItemScriptableObject iso;
        SO_ExploreSpotSetUpInfo es;
        public GameObject displayTip;
        public float tipWidth;
        public float tipHeight;
        private float textHeight;
        public Tooltip(ItemScriptableObject newIso, GameObject tip, ToolMode mode)
        {
            iso = newIso;
            displayTip = tip;
            string name = iso.tetrisHoverName;
            string description = iso.tetrisDescription;
            if (name == "not set") name = "";
            if (description == "not set") description = "";
            
            switch (mode)
            {
                //without description
                case ToolMode.INVENTORYHOME:
                    tip.transform.Find("Title").GetComponent<TextMeshPro>().text = name;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(tip.transform.Find("Title").GetComponent<RectTransform>());
                    tip.transform.Find("Background").transform.rotation = Quaternion.Euler(45, 0, 0);
                    tip.transform.Find("Title").transform.rotation = Quaternion.Euler(45, 0, 0);
                    textHeight = tip.transform.Find("Title").GetComponent<RectTransform>().rect.height;
                    break;
                    //with description
                case ToolMode.INVENTORYRECRAFT:
                    tip.transform.Find("Text").transform.Find("Title").GetComponent<TextMeshPro>().text = name;
                    GameObject Tetris = CraftingManager.i.CreateTetris(newIso, tip.transform.Find("Tetris Pic").transform.position, CraftingManager.CreateFrom.VISUAL_ONLY);
                    Tetris.transform.SetParent(tip.transform);
                    tip.transform.Find("Background").transform.rotation = Quaternion.Euler(45, 0, 0);
                    tip.transform.Find("Text").transform.rotation = Quaternion.Euler(45, 0, 0);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(tip.transform.Find("Text").transform.Find("Title").GetComponent<RectTransform>());
                    textHeight = tip.transform.Find("Text").transform.Find("Title").GetComponent<RectTransform>().rect.height + (newIso.Dimension.y) * 0.5f;
                    break;
                default:
                    break;
            }


            //keep
            tipWidth = tip.transform.Find("Background").GetComponent<Renderer>().bounds.size.x;
            tipHeight = tip.transform.Find("Background").GetComponent<Renderer>().bounds.size.y;

            Vector3 targetScale = tip.transform.Find("Background").transform.localScale;
            targetScale.y = (textHeight / tipHeight) * targetScale.y;
            tip.transform.Find("Background").transform.localScale = targetScale;
        }

        public Tooltip(SO_ExploreSpotSetUpInfo newES, GameObject tip)
        {
            es = newES;
            displayTip = tip;
            string name = es.spotHoverName;
            string description = es.spotHoverDescription;
            if (name == "not set") name = "";
            if (description == "not set") description = "";

            tip.transform.Find("Text").transform.Find("Title").GetComponent<TextMeshPro>().text = name;
            tip.transform.Find("Text").transform.Find("Description").GetComponent<TextMeshPro>().text = description;
            LayoutRebuilder.ForceRebuildLayoutImmediate(tip.transform.Find("Text").transform.Find("Description").GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(tip.transform.Find("Text").transform.Find("Title").GetComponent<RectTransform>());
            textHeight = 2 * (tip.transform.Find("Text").transform.Find("Title").GetComponent<RectTransform>().rect.height + tip.transform.Find("Text").transform.Find("Description").GetComponent<RectTransform>().rect.height);

            //keep
            tipWidth = tip.transform.Find("Background").GetComponent<Renderer>().bounds.size.x;
            tipHeight = tip.transform.Find("Background").GetComponent<Renderer>().bounds.size.y;

            Vector3 targetScale = tip.transform.Find("Background").transform.localScale;
            targetScale.y = (textHeight / tipHeight) * targetScale.y;
            tip.transform.Find("Background").transform.localScale = targetScale;

            //both not set
            if(name == "" && description == "")
            {
                DestroyDisplay();
            }
        }
        public void ChangePosition(Vector3 mousePosition)
        {
            if(displayTip != null)
            {
                displayTip.transform.position = mousePosition;
            }

        }
        public void DestroyDisplay()
        {
            Destroy(displayTip);
        }

        public void DisableDisplay()
        {
            displayTip.SetActive(false);
        }
    }

    [SerializeField] GameObject inventoryHomeTemplate;
    [SerializeField] GameObject inventoryCraftTemplate;
    [SerializeField] GameObject mapExploreSpotTemplate;
    GameObject currentTemplate;

    [SerializeField] float xOffset;
    [SerializeField] float yOffset;
    private Vector3 mousePos;
    Tooltip tip;
    private Vector3 lastMousePosition;
    private GameObject newDisplayTip;
    //public ItemScriptableObject ISO;

    public enum ToolMode
    {
        INVENTORYHOME,
        INVENTORYRECRAFT,
        MAPEXPLORESPOT
    }
    public enum MouseArea
    {
        TOPLEFT,
        TOPRIGHT,
        BOTTOMLEFT,
        BOTTOMRIGHT
    }
    
    public void ShowTip(ItemScriptableObject iso, ToolMode mode)
    {
        StartCoroutine(Wait(0.3f));
        if (isTipPresent()) tip.DestroyDisplay();
        switch (mode)
        {
            case ToolMode.INVENTORYHOME:
                currentTemplate = inventoryHomeTemplate;
                break;
            case ToolMode.INVENTORYRECRAFT:
                currentTemplate = inventoryCraftTemplate;
                break;
        }
        newDisplayTip = Instantiate(currentTemplate, this.transform);
        newDisplayTip.SetActive(true);
        tip = new Tooltip(iso, newDisplayTip, mode);
    }

    public void ShowMapTip(SO_ExploreSpotSetUpInfo es, ToolMode mode)
    {
        StartCoroutine(Wait(0.3f));
        if (isTipPresent()) tip.DestroyDisplay();
        currentTemplate = mapExploreSpotTemplate;
        Transform trans = this.transform;
        trans.position += new Vector3(0, 1, 0);
        newDisplayTip = Instantiate(currentTemplate, trans);
        newDisplayTip.SetActive(true);
        tip = new Tooltip(es, newDisplayTip);
    }

    public void UpdateTipPosition(Vector3 mousePos, MouseArea mouseArea)
    {
        StartCoroutine(Wait(0.3f));
        float _width = tip.tipWidth;
        float _height = tip.tipHeight;
        Vector3 newPosition = mousePos;
        switch (mouseArea)
        {
            case MouseArea.TOPLEFT:
                newPosition = new Vector3(mousePos.x + xOffset, mousePos.y + _height + yOffset, mousePos.z);
                break;
            case MouseArea.TOPRIGHT:
                newPosition = new Vector3(mousePos.x - xOffset, mousePos.y + _height + yOffset, mousePos.z);
                break;
            case MouseArea.BOTTOMLEFT:
                newPosition = new Vector3(mousePos.x + xOffset, mousePos.y, mousePos.z - _height + yOffset);
                break;
            case MouseArea.BOTTOMRIGHT:
                newPosition = new Vector3(mousePos.x - xOffset, mousePos.y, mousePos.z - _height + yOffset);
                break;
            default:
                newPosition = new Vector3(mousePos.x - xOffset, mousePos.y, mousePos.z);
                break;
        }
        if(tip!= null)
        {
            tip.ChangePosition(newPosition);
        }   
       
    }

    public bool isTipPresent()
    {
        if(tip == null)
        {
            return false;
        }
        return true;
    }

    public void DisableTip()
    {
        StartCoroutine(Wait(0.3f));
        if (tip != null) tip.DestroyDisplay();
        //tip.DisableDisplay();
    }

    void Start()
    {
        //ShowTip(ISO);
        lastMousePosition = Input.mousePosition;
    }

    //track tip location
    void Update()
    {
        int screenHeight = Screen.height;
        int screenWidth = Screen.width;
        RaycastHit hit;
        Vector3 _mousePosition = Input.mousePosition;
        MouseArea currentMouseArea = MouseArea.TOPRIGHT;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(_mousePosition), out hit))
        {
            mousePos = hit.point;
        }
        //change of mouse position
        if (_mousePosition != lastMousePosition)
        {
            if (_mousePosition.y < screenHeight / 3)
            {
                if (_mousePosition.x < screenWidth / 2)
                {
                    currentMouseArea = MouseArea.TOPLEFT;
                }
                else
                {
                    currentMouseArea = MouseArea.TOPRIGHT;
                }
            }
            else
            {
                if (_mousePosition.x < screenWidth / 2)
                {
                    currentMouseArea = MouseArea.BOTTOMLEFT;
                }
                else
                {
                    currentMouseArea = MouseArea.BOTTOMRIGHT;
                }

            }

            if(tip != null)
            { 
                UpdateTipPosition(mousePos, currentMouseArea);
            }
;
            lastMousePosition = _mousePosition;
        }
    

    }

    private IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }
}
