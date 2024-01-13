using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public Tooltip(ItemScriptableObject newIso, GameObject tip)
        {
            iso = newIso;
            displayTip = tip;
        }

        public void changePosition(Vector3 mousePosition)
        {
            displayTip.transform.position = mousePosition;
        }
        public void DestroyDisplay()
        {
            Destroy(displayTip);
        }

    }
    [SerializeField] GameObject tipTemplate;
    private Vector3 mousePos;
    private Tooltip tip;
    private Vector3 lastMousePosition;
    private GameObject newDisplayTip;
    public ItemScriptableObject ISO;
    public void ShowTip(ItemScriptableObject iso)
    {
        newDisplayTip = Instantiate(tipTemplate, this.transform);
        newDisplayTip.SetActive(true);
        tip = new Tooltip(iso, newDisplayTip);
    }

    public void updateTipPosition(Vector3 mousePos)
    {
        tip.changePosition(mousePos);
    }

    public void DestroyTip()
    {
        tip.DestroyDisplay();
    }
    void Start()
    {
        ShowTip(ISO);
        lastMousePosition = Input.mousePosition;
    }
    void Update()
    {
        RaycastHit hit;
        Vector3 _mousePosition = Input.mousePosition;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(_mousePosition), out hit))
        {
            mousePos = hit.point;
        }

        if (_mousePosition != lastMousePosition)
        {
            updateTipPosition(mousePos);
            lastMousePosition = _mousePosition;
        }

    }
}
