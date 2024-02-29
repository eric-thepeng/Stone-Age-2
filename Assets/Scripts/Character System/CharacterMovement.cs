using System;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class CharacterMovement : MonoBehaviour
{
    [HideInInspector]
    public float moveSpeed = 1f;

    [HideInInspector]
    public Vector3 hangOutAreaMin; // 挂出区域的最小边界

    [HideInInspector]
    public Vector3 hangOutAreaMax; // 挂出区域的最大边界

    // [HideInInspector]
    // public float hangOutWaitTime = 2f; // 停顿时间

    public GameObject sleepText;

    private Transform _visual;
    private CubismModel _model;
    [HideInInspector]
    public Animator animator;
    [SerializeField]
    private CubismParameter _leafShadow;
    private float _leafValue = 0;
    Vector3 _originalScale;
    

    // ensure prewalk doesn't trigger when changing direction
    Vector3 _previousMovement;
    private NavMeshAgent _navMeshAgent;
    
    private CharacterBehaviors _characterBehavior;

    public CharacterBehaviors CharacterBehavior
    {
        get => _characterBehavior;
        set => _characterBehavior = value;
    }

    private void Awake()
    {
        _navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        _navMeshAgent.speed = moveSpeed;
        _model = GetComponentInChildren<CubismModel>();
        _visual = _model.transform;
        _originalScale = _visual.transform.localScale;
        // _leafShadow = _model.Parameters[8];

        // hangOutWaitTime = Random.Range(0, hangOutWaitTime);

        // SelectRandomTargetPosition();
        // StartHangingOut();
    }

    
    public Vector3 _targetPosition;
    // public bool _isMovingToTarget = false;


    public bool _isHangingOut = false;
    // public float _hangOutTimer = 0f;

    private bool _reachedTarget = false;
    
    void Update()
    {
        if (_isHangingOut)
        {
            if (!ReachedTarget())
            {
                // _characterBehavior.IsPendingTowardsTarget = true;
                MoveTowardsTarget();
                _reachedTarget = false;
            }
            else
            {
                if (!_reachedTarget)
                {
                    _characterBehavior.IsPendingTowardsTarget = false;
                    _reachedTarget = true;
                    Debug.Log("Target Stop Moving, Distance: " + Vector3.Distance(transform.position, _targetPosition));
                    if (Vector3.Distance(transform.position, _targetPosition) < 1f)
                    {
                        _characterBehavior.EnterWorkingState();
                        
                    }
                    else
                    {
                        // player pending = false, start working
                        // player reach object
                        // player pending = false, restart check state
                        _characterBehavior.CurrentState = CharacterBehaviors.HomeState.Unset;
                        // _reachedTarget = false;
                    }
                    _isHangingOut = false;
                
                }

            }
        }
    }


    public void StartHangingOut()
    {
        _isHangingOut = true;
        _reachedTarget = false;
        // SelectRandomTargetPosition();
        // _hangOutTimer = hangOutWaitTime;
    }

    public void StopHangingOut()
    {
        _isHangingOut = false;
        animator.SetBool("isWalking", false);
    }

    public void StartSleeping()
    {
        if (sleepText != null) sleepText.SetActive(true);
        animator.SetTrigger("Sit");
    }

    public void StopSleeping()
    {
        if (sleepText != null) sleepText.SetActive(false);
    }

    public void SelectRandomTargetPosition()
    {
        float randomX = Random.Range(hangOutAreaMin.x, hangOutAreaMax.x);
        float randomZ = Random.Range(hangOutAreaMin.z, hangOutAreaMax.z);
        _targetPosition = new Vector3(randomX, transform.position.y, randomZ);
        bool isPathValid = false;
        while (!isPathValid)
        {
            isPathValid = SetTargetPosition(_targetPosition);
        }
        
        // _navMeshAgent.SetDestination(new Vector3(randomX, 0, randomZ));
        // _isMovingToTarget = true;
    }

    public bool SetTargetPosition(Vector3 position)
    {
        _targetPosition = position;
        NavMeshPath path = new NavMeshPath();
        if (_navMeshAgent.CalculatePath(_targetPosition, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            _navMeshAgent.SetDestination(_targetPosition);
            // _isMovingToTarget = true;
            return true;
        }
        return false;
    }
    //
    // public void MoveToPosition(Vector3 position)
    // {
    //     _targetPosition = position;
    //     _isMovingToTarget = true;
    // }

    private bool ReachedTarget()
    {
        if (_navMeshAgent.velocity == new Vector3(0,0,0)
            )
        {
            animator.SetBool("isWalking", false);
            // _isMovingToTarget = false;
            return true;
        } else if (Vector3.Distance(transform.position, _targetPosition) < 1f)
        {
            return true;
        }

        return false;
    }
    
    private void MoveTowardsTarget()
    {
        // if (Vector3.Distance(transform.position, _targetPosition) < 0.1f)
        // {
        //     animator.SetBool("isWalking", false);
        //     // _isMovingToTarget = false;
        //     return;
        // }

        animator.SetBool("isWalking", true);

        // Vector3 movementDirection = (_targetPosition - transform.position).normalized;
        // transform.position = Vector3.MoveTowards(transform.position, _targetPosition, moveSpeed * Time.deltaTime);

            
        if (_navMeshAgent.velocity.x < 0)
        {
            _leafValue = 1;
            _visual.localScale = new Vector3(-_originalScale.x, _originalScale.y, _originalScale.z);
        }
        else if (_navMeshAgent.velocity.x > 0)
        {
            _leafValue = 0;
            _visual.localScale = new Vector3(_originalScale.x, _originalScale.y, _originalScale.z);
        }
        _leafShadow.Value = _leafValue;
    }

}
