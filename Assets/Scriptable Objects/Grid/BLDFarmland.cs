using System.Collections;
using System.Collections.Generic;
using Hypertonic.GridPlacement;
using UnityEngine;

public class BLDFarmland : CropGrowth
{
    private bool isPlaced;

    [Header("Current State")]
    UnlockState unlockState;

    //[Header("Counting Info")]
    private bool isCountingDown;
    private float totalGrowthTime;
    private float elapsedTime;

    [Header("Progress Bar")]
    public GameObject Bar;
    public GameObject progress;
    public GameObject background;

    [Header("Particle Object")]
    public GameObject particlePrefab;

    private void Update()
    {
        isPlaced = !GridManagerAccessor.GridManager.IsPlacingGridObject;

        unlockState = GetCurrentUnlockState();

        isCountingDown = unlockState.IsCountingDown;
        totalGrowthTime = unlockState.TotalGrowthTime;
        elapsedTime = unlockState.ElapsedTime;

        if (isCountingDown)
        {
            Bar.SetActive(true);
            Vector3 _pos = progress.transform.localPosition;
            Vector3 _scale = progress.transform.localScale;
            _scale.x = elapsedTime / totalGrowthTime;

            float bgScale = background.transform.localScale.x;
            _pos.x = - bgScale / 2 + _scale.x/2;

            if (!float.IsNaN(_pos.x) && !float.IsInfinity(_pos.x)) progress.transform.localPosition = _pos;
            if (!float.IsNaN(_scale.x) && !float.IsInfinity(_scale.x)) progress.transform.localScale = _scale;
        } else
        {
            Bar.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        //if (WorldUtility.GetMouseHitObject(WorldUtility.LAYER.WORLD_INTERACTABLE, true).Equals(this) )
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject && isPlaced)
            {
                //Debug.Log("Touched Farmland!");
                if (Water())
                {
                    Debug.Log("Unlock to Next State");
                }
            }
        }

    }
    // added long-press

    public void Reward()
    {
        Debug.Log("Reward Executed!");
    }

    public void PlayParticle()
    {
        Vector3 position = transform.position;
        position.y += 0.5f;
        Instantiate(particlePrefab, position, Quaternion.identity);

    }
}
