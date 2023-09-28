using System.Collections;
using System.Collections.Generic;
using Hypertonic.GridPlacement;
using UnityEngine;

public class GridObjectTags : MonoBehaviour
{

    [SerializeField]
    private List<string> _gridObjectTags;

    [SerializeField]
    private BuildingISO BISO;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool containsTag(string ObjectTag)
    {
        if (_gridObjectTags.Contains(ObjectTag)) return true;
        else return false;
    }

    public BuildingISO GetBuildingISO()
    {
        return BISO;
    }
}
