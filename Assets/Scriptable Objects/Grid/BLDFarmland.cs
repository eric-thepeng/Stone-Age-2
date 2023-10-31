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

    [Header("UI Sprite")]
    public Sprite needWaterIcon;
    public Sprite matureIcon;

    [Header("Progress Bar")]
    public GameObject Icon;
    public GameObject Bar;
    public GameObject progress;
    public GameObject background;

    [Header("Particle Object")]
    public GameObject particlePrefab;

    //[Header("Invoke Related")]
    private float timeToClear;


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
            Icon.GetComponent<SpriteRenderer>().sprite = null;
        }

        if (isCountingDown)
        {
            Icon.GetComponent<SpriteRenderer>().sprite = null;

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


    public void Reward()
    {
        //Debug.Log("Reward Executed!");
    }

    public void PlayParticle()
    {
        Vector3 position = transform.position;
        position.y += 0.5f;
        Instantiate(particlePrefab, position, Quaternion.identity);

    }


    protected override void onStateChange()
    {
        base.onStateChange();
        //Debug.LogWarning("State Change!");
        PlayParticle();
        goalSprite = needWaterIcon;
        
    }

    protected override void onCropMatured()
    {
        base.onCropMatured();
        //Debug.LogWarning("Matured!");
        PlayParticle();
        goalSprite = matureIcon;
    }

    //protected override void EndMouseHover()
    //{
    //    //if (state == State.Selected) ChangeStateTo(State.Idle);
    //    base.EndMouseHover();
    //}

    protected override void BeginMousePress()
    {
        base.BeginMousePress();
        Debug.Log("Mouse Press Begin");

        transform.DOShakePosition(0.3f, new Vector3(0.1f, 0, 0), 10, 0);
        //if (state == State.Idle) ChangeStateTo(State.Selected);
        //else if (state == State.Selected)logPressing = true;
    }

    bool logPressing;
    float pressedTime;

    Sprite goalSprite; 

    protected override void WhileMousePress()
    {
        base.WhileMousePress();
        Debug.Log("Mouse pressing");
        //if (logPressing)
        //{
            pressedTime += Time.deltaTime;
            //ui.SetProgress(Mathf.Clamp(pressedTime / timeToClear, 0, 1));
            //if (isPlaced & pressedTime > timeToClear)
            //{
            //    if (Water())
            //    {
            //        Debug.Log("Unlock to Next State");
            //    }
            //    pressedTime = 0;
            //    //logPressing = false;
            //}
        //}
    }


    protected override void EndMousePress()
    {
        base.EndMousePress();


        if (isPlaced & pressedTime > timeToClear)
        {
            if (Water())
            {
                Debug.Log("Unlock to Next State");
            }
            pressedTime = 0;
            //logPressing = false;
        }
        //if (isPlaced)
        //{
        //    if (timeToClear == 0)
        //    {
        //        //Debug.Log("Touched Farmland!");
        //        if (Water())
        //        {
        //            Debug.Log("Unlock to Next State");
        //        }
        //    }
        //}
    }
}
