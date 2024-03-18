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
    private float clickInterval;

    // [Header("Animation")]
    [SerializeField]
    private AnimationCurve animationCurve;
    [SerializeField]
    private float duration;

    private Vector3 originalScale;

    [Header("Click Cycle")]
    [SerializeField]
    private int maxClicks;
    private int currentClicks = 0;

    [SerializeField]
    private float countdownTime;
    private float currentTime;

    [Header("Icon Related")]
    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private Sprite clickableIcon;
    [SerializeField] private Sprite unclickableIcon;
    
    CharacterMovement _characterMovement;

    private bool _enabledRuaMode = true;

    void Start()
    {
        originalScale = transform.localScale;
        
        currentTime = countdownTime;

        SetCurrentInteraction(null);

        _characterMovement = GetComponent<CharacterMovement>();
        
        iconRenderer.gameObject.SetActive(false);
    }

    public void Update()
    {
        transform.rotation = new Quaternion();
        // 更新倒计时
        if (_enabledRuaMode)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                // 重置
                currentTime = countdownTime;
                currentClicks = 0;
            
                iconRenderer.sprite = clickableIcon;
                iconRenderer.gameObject.SetActive(true);
                SetCurrentInteraction(new WorldInteractable.InteractionType(WorldInteractable.InteractionType.TypeName.Click, AddPoint));

            }
        }

        // if (Input.GetKeyDown(KeyCode.H))
        // {
        //     DisableRuaCountdown();
        // }
        // if (Input.GetKeyDown(KeyCode.J))
        // {
        //     EnableRuaCountdown();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.K))
        // {
        //     EnterRuaState();
        // }
        
        
    }

    public void AddPoint()
    {
        
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

            
            currentClicks++;
            if (currentClicks >= maxClicks)
            {
                iconRenderer.sprite = unclickableIcon;
                iconRenderer.gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(waitForDuration(clickInterval));//particleSystem.main.duration));
            }
        }
    }

    private IEnumerator waitForDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        SetCurrentInteraction(new WorldInteractable.InteractionType(WorldInteractable.InteractionType.TypeName.Click, AddPoint));
    }
    
    public void EnableRuaCountdown()
    {
        // iconRenderer.sprite = clickableIcon;
        if (currentClicks < maxClicks)
        {
            SetCurrentInteraction(new WorldInteractable.InteractionType(WorldInteractable.InteractionType.TypeName.Click, AddPoint));
            iconRenderer.sprite = clickableIcon;
            iconRenderer.gameObject.SetActive(true);
        }
        _enabledRuaMode = true;
        // if ()
    }
    
    public void DisableRuaCountdown()
    {
            SetCurrentInteraction(null);
        
        _enabledRuaMode = false;
        iconRenderer.sprite = unclickableIcon;
        iconRenderer.gameObject.SetActive(false);
    }

    public void EnterRuaState()
    {
        if (!_enabledRuaMode)
        {
            EnableRuaCountdown();
        }
        
        SetCurrentInteraction(new WorldInteractable.InteractionType(WorldInteractable.InteractionType.TypeName.Click, AddPoint));
        
        currentTime = 0;
        iconRenderer.sprite = clickableIcon;
        iconRenderer.gameObject.SetActive(true);
    }

    public void Initialize(CharacterBasicStats initialStats)
    {
        maxClicks = initialStats.maxClicks;
        PointToAdd = initialStats.pointsToAdd;
        clickInterval = initialStats.clickInterval;
        countdownTime = initialStats.countdownTime;
    }

    private Vector3 _lastTargetPosition;
    protected override void BeginMouseHover()
    {
        base.BeginMouseHover();
        if (_enabledRuaMode)
        {
            _lastTargetPosition = _characterMovement.navMeshAgent.steeringTarget;
            _characterMovement.SetTargetPosition(transform.position);
            _characterMovement.StopHangingOut();
            
            // _characterMovement.navMeshAgent.enabled = false;
        }
    }

    protected override void EndMouseHover()
    {
        base.EndMouseHover();
        if (_enabledRuaMode)
        {
            _characterMovement.SetTargetPosition(_lastTargetPosition);
            _characterMovement.StartHangingOut();
            // _characterMovement.navMeshAgent.enabled = true;
        }
    }
}
