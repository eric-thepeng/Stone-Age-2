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

    class Tooltip
    {
        ItemScriptableObject iso;
        public GameObject displayTip;
        public float tipWidth;
        public float tipHeight;
        private float textHeight;
        public Tooltip(ItemScriptableObject newIso, GameObject tip)
        {
            iso = newIso;
            displayTip = tip;
            tip.transform.Find("Text").transform.Find("Title").GetComponent<TextMeshPro>().text = iso.tetrisHoverName;
            tip.transform.Find("Text").transform.Find("Description").GetComponent<TextMeshPro>().text = iso.tetrisDescription;
            LayoutRebuilder.ForceRebuildLayoutImmediate(tip.transform.Find("Text").transform.Find("Description").GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(tip.transform.Find("Text").transform.Find("Title").GetComponent<RectTransform>());
            textHeight = tip.transform.Find("Text").transform.Find("Title").GetComponent<RectTransform>().rect.height + tip.transform.Find("Text").transform.Find("Description").GetComponent<RectTransform>().rect.height;

            tipWidth = tip.transform.Find("Background").GetComponent<Renderer>().bounds.size.x;
            tipHeight = tip.transform.Find("Background").GetComponent<Renderer>().bounds.size.y;


            Vector3 targetScale = tip.transform.Find("Background").transform.localScale;
            targetScale.y = (textHeight / tipHeight) * targetScale.y;
            tip.transform.Find("Background").transform.localScale = targetScale;
        }
        public void changePosition(Vector3 mousePosition)
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
    }

    [SerializeField] GameObject tipTemplate;
    [SerializeField] float offset;
    private Vector3 mousePos;
    Tooltip tip;
    private Vector3 lastMousePosition;
    private GameObject newDisplayTip;
    //public ItemScriptableObject ISO;

    public enum MouseArea
    {
        TOPLEFT,
        TOPRIGHT,
        BOTTOMLEFT,
        BOTTOMRIGHT
    }
    
    public void ShowTip(ItemScriptableObject iso)
    {
        newDisplayTip = Instantiate(tipTemplate, this.transform);
        newDisplayTip.SetActive(true);
        tip = new Tooltip(iso, newDisplayTip);
    }


    public void UpdateTipPosition(Vector3 mousePos, MouseArea mouseArea)
    {
        float _width = tip.tipWidth;
        float _height = tip.tipHeight;
        Vector3 newPosition = mousePos;
        switch (mouseArea)
        {
            case MouseArea.TOPLEFT:
                newPosition = new Vector3(mousePos.x + offset, mousePos.y - offset, mousePos.z);
                break;
            case MouseArea.TOPRIGHT:
                newPosition = new Vector3(mousePos.x - offset, mousePos.y - offset, mousePos.z);
                break;
            case MouseArea.BOTTOMLEFT:
                newPosition = new Vector3(mousePos.x + offset, mousePos.y + offset, mousePos.z);
                break;
            case MouseArea.BOTTOMRIGHT:
                newPosition = new Vector3(mousePos.x - offset, mousePos.y + offset, mousePos.z);
                break;
            default:
                newPosition = new Vector3(mousePos.x - offset, mousePos.y - offset, mousePos.z);
                break;
        }
        if(tip!= null)
        {
            tip.changePosition(newPosition);
        }   
       
    }

    public void DestroyTip()
    {
        tip.DestroyDisplay();
    }

    void Start()
    {
        //ShowTip(ISO);
        lastMousePosition = Input.mousePosition;
    }

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

        //change of mousse position
        if (_mousePosition != lastMousePosition)
        {
            if (_mousePosition.y > screenHeight / 2)
            {
                if (_mousePosition.x < screenWidth / 2)
                {
                    currentMouseArea = MouseArea.TOPLEFT;
                    //Debug.Log("Mouse is in the top left.");
                }
                else
                {
                    currentMouseArea = MouseArea.TOPRIGHT;
                    //Debug.Log("Mouse is in the top right.");
                }
            }
            else
            {
                if (_mousePosition.x < screenWidth / 2)
                {
                    currentMouseArea = MouseArea.BOTTOMLEFT;
                    //Debug.Log("Mouse is in the bottom left.");
                }
                else
                {
                    currentMouseArea = MouseArea.BOTTOMRIGHT;
                    //Debug.Log("Mouse is in the bottom right.");
                }

            }

            if(tip != null)
            {
                UpdateTipPosition(mousePos, currentMouseArea);
            }
            //UpdateTipPosition(mousePos, currentMouseArea);
            lastMousePosition = _mousePosition;
        }
    

    }
}
