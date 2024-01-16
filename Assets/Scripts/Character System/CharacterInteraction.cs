using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;
using Quaternion = UnityEngine.Quaternion;

public class CharacterInteraction : WorldInteractable
{
    [SerializeField, Header("DO NOT EDIT CurrentInteraction ABOVE")]
    private int PointToAdd = 1;
    // [SerializeField]
    // private VisualEffect VFXSystem;
    // [SerializeField]
    // private ParticleSystem particleSystem;

    [SerializeField]
    private float clickInterval = 0.1f;

    [Header("Animation")]

    [SerializeField]
    private AnimationCurve animationCurve;
    [SerializeField]
    private float duration;

    private Vector3 originalScale;

    [Header("Click Cycle")]
    [SerializeField]
    private int maxClicks = 5;
    [SerializeField]
    private int currentClicks = 0;

    [SerializeField]
    private float countdownTime = 10f;
    [SerializeField]
    private float currentTime;

    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private Sprite clickableIcon;
    [SerializeField] private Sprite unclickableIcon;

    void Start()
    {
        originalScale = transform.localScale;
        SetCurrentInteraction(new WorldInteractable.InteractionType(WorldInteractable.InteractionType.TypeName.Click, AddPoint));
        
        currentTime = countdownTime;
        iconRenderer.sprite = clickableIcon;
    }

    public void Update()
    {
        transform.rotation = new Quaternion();
        // 更新倒计时
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            // 重置
            currentTime = countdownTime;
            currentClicks = 0;
            
            iconRenderer.sprite = clickableIcon;
            SetCurrentInteraction(new WorldInteractable.InteractionType(WorldInteractable.InteractionType.TypeName.Click, AddPoint));

        }
    }

    public void AddPoint()
    {
        Debug.Log("trying to click");
        if (currentClicks < maxClicks)
        {
                

            SpiritPoint.i.Add(PointToAdd);

            // if (particleSystem != null)
            // {
            //     particleSystem.Play();
            // }
            // if (VFXSystem != null)
            // {
            //     VFXSystem.Play();
            // }
            // 保存原始尺寸

            // 使用DOVirtual.Float根据动画曲线缩放物体
            DOVirtual.Float(0f, 1f, duration, (float value) =>
            {
                float scaleValue = animationCurve.Evaluate(value);
                Vector3 newScale = transform.localScale;
                newScale.y = originalScale.y * scaleValue;
                transform.localScale = newScale;
            }).OnComplete(() =>
            {
                // 动画完成后恢复原始尺寸
                transform.localScale = originalScale;
            });

            StartCoroutine(waitForDuration(clickInterval));//particleSystem.main.duration));
            
            currentClicks++;
            if (currentClicks == maxClicks) iconRenderer.sprite = unclickableIcon;
        }
    }

    private IEnumerator waitForDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        SetCurrentInteraction(new WorldInteractable.InteractionType(WorldInteractable.InteractionType.TypeName.Click, AddPoint));
    }
}
