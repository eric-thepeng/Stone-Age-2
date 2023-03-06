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

    List<HomeGrid> allHomeGrids = new List<HomeGrid>();

    public bool building = false;

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
    }

    public void AddHomeGrid(HomeGrid hg)
    {
        allHomeGrids.Add(hg);
    }
}
