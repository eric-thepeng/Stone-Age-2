using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hypertonic.GridPlacement;
using System;

public class MaskGenerateManager : MonoBehaviour
{
    public GameObject defaultSizePrefab;

    private GridOperationManager gridOperationManager;
    private Sprite spriteToRender; // 拖拽你想渲染的Sprite到这里

    // Start is called before the first frame update
    void Start()
    {
        gridOperationManager = FindObjectOfType<GridOperationManager>();
        Invoke("SetupMask", 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetupMask()
    {
        gridOperationManager = FindObjectOfType<GridOperationManager>();

        int AmountOfCellsX = gridOperationManager._gridSettings.AmountOfCellsX;
        int AmountOfCellsY = gridOperationManager._gridSettings.AmountOfCellsY;

        //GameObject defaultSizeObject = Instantiate(defaultSizePrefab, transform);

        //for (int indX = 0; indX < AmountOfCellsX; indX++)
        //{
        //    for (int indY = 0; indY < AmountOfCellsY; indY++)
        //    {
        //        Debug.Log(indX + " " + indY);

        //        //GridUtilities.
        //        //if (!GridManagerAccessor.GridManager.CanAddObjectAtCell(defaultSizeObject, new Vector2Int(indX, indY)))
        //        //{

        //        //    float cellSize = GridUtilities.GetWorldSizeOfCell(gridOperationManager._gridSettings);
        //        //}
        //    }

        //}

        //Destroy(defaultSizeObject);
    }
}
