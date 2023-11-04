using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Hypertonic.GridPlacement;
using Unity.Burst.CompilerServices;
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

    [Header("UI Objects - Icon")]
    public GameObject Icon;

    [Header("UI Objects - Progress Bar")]
    public bool Visibility;
    public GameObject Bar;
    public GameObject progress;
    public GameObject background;

    //[Header("Invoke Related")]
    private float timeToClear;

    Sprite goalSprite;

    private void Update()
    {
        isPlaced = !GridManagerAccessor.GridManager.IsPlacingGridObject;

        unlockState = GetCurrentUnlockState();

        isCountingDown = unlockState.IsCountingDown;
        totalGrowthTime = unlockState.TotalGrowthTime;
        elapsedTime = unlockState.ElapsedTime;

        timeToClear = unlockState.timeToClear;

        if (isPlaced)
        {
            Icon.GetComponent<SpriteRenderer>().sprite = goalSprite;
        }
        else
        {
            Icon.SetActive(false);
            //Icon.GetComponent<SpriteRenderer>().sprite = null;
        }

        if (isCountingDown)
        {
            Icon.SetActive(false);
            //Icon.GetComponent<SpriteRenderer>().sprite = null;

            if (Visibility)
            {
                Bar.SetActive(true);
                Vector3 _pos = progress.transform.localPosition;
                Vector3 _scale = progress.transform.localScale;
                _scale.x = elapsedTime / totalGrowthTime;

                float bgScale = background.transform.localScale.x;
                _pos.x = -bgScale / 2 + _scale.x / 2;

                if (!float.IsNaN(_pos.x) && !float.IsInfinity(_pos.x)) progress.transform.localPosition = _pos;
                if (!float.IsNaN(_scale.x) && !float.IsInfinity(_scale.x)) progress.transform.localScale = _scale;
            }
        }
        else
        {
            Bar.SetActive(false);
        }

    }


    public void Reward()
    {
        //Debug.Log("Reward Executed!");
    }

    public void InteractFinished()
    {
        if (Water())
        {
            Debug.Log("Unlock to Next State");
        }
    }

    protected override void onStateChange()
    {
        base.onStateChange();
        //Debug.LogWarning("State Change --!");
        //PlayParticle();
        goalSprite = GetCurrentUnlockState().interactableIcon;

    }

    protected override void onCropMatured()
    {
        base.onCropMatured();
        //Debug.LogWarning("Matured!");
        //PlayParticle();
        //goalSprite = matureIcon;
        goalSprite = GetCurrentUnlockState().interactableIcon;
    }

    private GameObject waterObject;

    public void PlayWaterEffect(GameObject particlePrefab)
    {

        if (particlePrefab != null)
        {
            Vector3 position = transform.position;
            position.y += 0.5f;
            waterObject = Instantiate(particlePrefab, position, Quaternion.identity);
        }
    }

    protected override void BeginMousePress()
    {
        base.BeginMousePress();

        PlayWaterEffect(GetCurrentUnlockState().overtimeParticle);

        transform.DOShakePosition(0.3f, new Vector3(0.1f, 0, 0), 10, 0);
    }

    //bool logPressing;
    //float pressedTime;


    //protected override void WhileMousePress()
    //{
    //    base.WhileMousePress();

    //    pressedTime += Time.deltaTime;
    //}


    protected override void EndMousePress()
    {
        Destroy(waterObject);

        base.EndMousePress();



        //    if (isPlaced & pressedTime > timeToClear)
        //    {
        //        if (Water())
        //        {
        //            Debug.Log("Unlock to Next State");
        //        }
        //        pressedTime = 0;
        //    }
        //}
    }
}