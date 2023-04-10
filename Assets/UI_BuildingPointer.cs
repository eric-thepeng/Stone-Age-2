using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    bool displaying = false;
    Transform pointerObject;

    private void Start()
    {
        pointerObject = transform.Find("Building Pointer UI");
        TurnOff();
    }

    private void Update()
    {
        if (!displaying) return;
        if(!PlayerState.IsBuilding()) TurnOff();
        try { pointerObject.transform.position = WorldSpaceUI.i.GetMouseWorldPositionAtWorldSpaceUI(); }
        catch
        {
            TurnOff();
        }

    }

    public void SetUp(BuildingISO biso)
    {
        displaying = true;
        pointerObject.gameObject.SetActive(true);
        if(pointerObject.GetChild(0).childCount > 1)
        {
            Destroy(pointerObject.GetChild(0).GetChild(1).gameObject);
        }
        GameObject go = Instantiate(biso.buildingPrefab, pointerObject.GetChild(0));
        go.transform.Rotate(new Vector3(1,0,0),-45);
        go.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
        //go.transform.eulerAngles = new Vector3(-22.5f, 0, 0);
    }

    public void TurnOff()
    {
        displaying = false;
        Destroy(pointerObject.GetChild(0).GetChild(1).gameObject);
        pointerObject.gameObject.SetActive(false);
    }
}
