using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ExploreMapPanel : PanelSystem
{
    static ExploreMapPanel instance=null;
    public static ExploreMapPanel i
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<ExploreMapPanel>();
            }
            return instance;
        }
    }
}
