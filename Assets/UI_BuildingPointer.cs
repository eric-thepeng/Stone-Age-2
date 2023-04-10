using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class UI_BuildingPointer : MonoBehaviour
{
    static UI_BuildingPointer instance;
    public static UI_BuildingPointer i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_BuildingPointer>();
            }
            return instance;
        }
    }

    bool displaying { get { return displayingISO != null; } }
    ItemScriptableObject displayingISO = null;
    Transform pointerObject;
    TextMeshPro remainAmountObject;

    private void Start()
    {
        pointerObject = transform.Find("Building Pointer UI");
        remainAmountObject = pointerObject.GetChild(0).GetChild(0).GetComponent<TextMeshPro>();
        TurnOff();
    }

    private void Update()
    {
        if (!displaying) return;
        if(!PlayerState.IsBuilding()) TurnOff();
        try { pointerObject.transform.position = WorldSpaceUI.i.GetMouseWorldPositionAtWorldSpaceUI(); }
        catch
        {
            //PlayerInputChannel.BuildingSystemOpenButton();
            //return;
        }

        UpdateDisplayAmount();
    }

    public void SetUp(BuildingISO biso)
    {
        displayingISO = biso;
        pointerObject.gameObject.SetActive(true);
        if(pointerObject.GetChild(0).childCount > 1)
        {
            Destroy(pointerObject.GetChild(0).GetChild(1).gameObject);
        }
        GameObject go = Instantiate(biso.buildingPrefab, pointerObject.GetChild(0));
        go.transform.Rotate(new Vector3(1,0,0),-45);
        go.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
    }

    private void UpdateDisplayAmount()
    {
        remainAmountObject.text = "" + Inventory.i.ItemInStockAmount(displayingISO);
    }

    public void TurnOff()
    {
        displayingISO = null;
        if (pointerObject.GetChild(0).childCount > 1)
        {
            Destroy(pointerObject.GetChild(0).GetChild(1).gameObject);
        }
        pointerObject.gameObject.SetActive(false);
    }
}
