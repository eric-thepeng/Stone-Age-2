using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    static BuildingManager instance;
    public static BuildingManager i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BuildingManager>();
            }
            return instance;
        }
    }

    UI_InventoryBlock selectedUIIB = null;

    List<HomeGrid> allHomeGrids = new List<HomeGrid>();

    public bool building = false;
    public GameObject placeholdingBuilding;

    public void OpenBuilding()
    {
        building = true;
        foreach(HomeGrid hg in allHomeGrids)
        {
            hg.ShowGridLines();
        }
    }

    public void CloseBuilding()
    {
        building = false;
        foreach (HomeGrid hg in allHomeGrids)
        {
            hg.HideGridLines();
        }
        CancelSelectedBuidling();
    }

    public void AddHomeGrid(HomeGrid hg)
    {
        allHomeGrids.Add(hg);
    }

    public BuildingISO GetSelectedBuildingISO()
    {
        if (selectedUIIB == null) return null;
        ItemScriptableObject returnISO = selectedUIIB.GetISO();
        return returnISO is BuildingISO ? (BuildingISO)returnISO :null;
    }

    public void SetSelectedBuilding(UI_InventoryBlock uiib)
    {
        if(selectedUIIB != null)selectedUIIB.SetSelectedBackground(false);
        selectedUIIB = uiib;
        UI_BuildingPointer.i.SetUp((BuildingISO)uiib.GetISO());
    }

    public void CancelSelectedBuidling()
    {
        if(selectedUIIB !=null)selectedUIIB.SetSelectedBackground(false);
        selectedUIIB = null;
        UI_BuildingPointer.i.TurnOff();
    }
}
