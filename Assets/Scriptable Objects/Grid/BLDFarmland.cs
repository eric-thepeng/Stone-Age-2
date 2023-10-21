using System.Collections;
using System.Collections.Generic;
using Hypertonic.GridPlacement;
using UnityEngine;

public class BLDFarmland : CropGrowth
{
    public bool isPlaced;

    public GameObject particlePrefab;

    [Header("Counting Info")]
    public bool isCountingDown;
    public float totalGrowthTime;
    public float remainingTime;

    private void Update()
    {
        isPlaced = !GridManagerAccessor.GridManager.IsPlacingGridObject;

        UnlockState unlockState = GetCurrentUnlockState();
        isCountingDown = unlockState.IsCountingDown;
        totalGrowthTime = unlockState.TotalGrowthTime;
        remainingTime = unlockState.RemainingTime;
    }

    private void OnMouseDown()
    {


        //if (WorldUtility.GetMouseHitObject(WorldUtility.LAYER.WORLD_INTERACTABLE, true).Equals(this) )
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 23))
        {
            Debug.Log("Mouse Clicked!");
            if (hit.collider.gameObject == gameObject && isPlaced)
            {
                Debug.Log("Touched Farmland!");
                if (UnlockToNextState())
                {
                    Instantiate(particlePrefab);
                }
            }
        }

    }

    public void Reward()
    {
        Debug.Log("Reward Executed!");
    }
}
