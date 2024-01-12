using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHoverInfo : MonoBehaviour
{
    static InventoryHoverInfo instance;
    public static InventoryHoverInfo i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InventoryHoverInfo>();
            }
            return instance;
        }
    }


    class toolTip
    {
        public GameObject tipBoard;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
