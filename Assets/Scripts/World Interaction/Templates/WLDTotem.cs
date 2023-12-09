using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;
using UnityEngine;
using DG.Tweening;

public class WLDTotem : WorldInteractable
{
    [SerializeField, Header("DO NOT EDIT CurrentInteraction ABOVE")]
    private int PointToAdd = 1;
    [SerializeField]
    private VisualEffect VFXSystem;
    [SerializeField]
    private ParticleSystem particleSystem;

    [SerializeField]
    private float clickInterval = 0.1f;

    //[Header("Animation")]

    [SerializeField]
    private AnimationCurve animationCurve;
    [SerializeField]
    private float duration;

    private Vector3 originalScale;


    void Start()
    {
        originalScale = transform.localScale;
        SetCurrentInteraction(new InteractionType(InteractionType.TypeName.Click, AddPoint));
    }

    public void AddPoint()
    {
        SpiritPoint.i.Add(PointToAdd);

        if (particleSystem != null)
        {
            particleSystem.Play();
        }
        if (VFXSystem != null)
        {
            VFXSystem.Play();
        }
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
    }

    private IEnumerator waitForDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        SetCurrentInteraction(new InteractionType(InteractionType.TypeName.Click, AddPoint));
    }
}
