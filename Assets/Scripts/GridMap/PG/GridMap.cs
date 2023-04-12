using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GridMap<T>
{
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private T[,] gridArray;

    public GridMap(int width, int height, float cellSize, Vector3 origionPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = origionPosition;

        gridArray = new T[width, height];

    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize + originPosition;
    }

    public void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public Vector3 GetWorldPositionFromPosition(Vector3 origionPosition)
    {
        //int x, z;
        GetXZ(origionPosition, out int x, out int z);
        return GetWorldPosition(x, z);
    }

    public void SetValue(int x, int z, T value)
    {
        if (x >= 0 && z >= 0 && x < width &&z < height)
        {
            gridArray[x, z] = value;
            //if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
        }
    }

    public void SetValue(Vector3 worldPosition, T value)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        SetValue(x, z, value);
    }

    public T GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(T); //TODO: change this shit
        }
    }

    public T GetValue(Vector3 worldPosition)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return GetValue(x, z);
    }

    public void TriggerGridObjectChanged(int x, int z)
    {

    }
}
