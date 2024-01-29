using System.Collections;
using System.Collections.Generic;
using Hypertonic.GridPlacement;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridTilemapManager : MonoBehaviour
{
    private Tilemap typeTilemap;

    public Tilemap TypeTilemap
    {
        get => typeTilemap;
        set => typeTilemap = value;
    }

    // Start is called before the first frame update
    void Awake()
    {
        // Tilemap typeTilemap = GridManagerAccessor.GridManager.GridSettings.RegionTypePresetTilemap.GetComponent<Tilemap>();
        typeTilemap = FindObjectOfType<GridDisplayManager>().GenerateCurrentGridBackground(transform);

        // typeTilemap = Find
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
