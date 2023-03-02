using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniGridMap : GridMap<ItemScriptableObject>
{
    public UniGridMap(int width, int height, float cellSize, Vector3 origionPosition) : base(width, height, cellSize, origionPosition)
    {
    }

}
