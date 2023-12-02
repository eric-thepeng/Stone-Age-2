using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 1f;

    public Vector3 hangOutAreaMin; // 挂出区域的最小边界
    public Vector3 hangOutAreaMax; // 挂出区域的最大边界
    public float hangOutWaitTime = 2f; // 停顿时间

    public GameObject sleepText;

    private Transform _visual;
    private CubismModel _model;
    private Animator _animator;
    private CubismParameter _leafShadow;
    Vector3 _originalScale;

    // ensure prewalk doesn't trigger when changing direction
    Vector3 _previousMovement;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _model = GetComponentInChildren<CubismModel>();
        _visual = _model.transform;
        _originalScale = _visual.transform.localScale;
        _leafShadow = _model.Parameters[8];
    }


    private Vector3 _targetPosition;
    private bool _isMovingToTarget = false;


    private bool _isHangingOut = false;
    private float _hangOutTimer = 0f;

    void Update()
    {
        if (_isHangingOut)
        {
            if (_isMovingToTarget)
            {
                MoveTowardsTarget();
            }
            else
            {
                _hangOutTimer -= Time.deltaTime;
                if (_hangOutTimer <= 0)
                {
                    SelectRandomTargetPosition();
                    _hangOutTimer = hangOutWaitTime;
                }
            }
        }
    }


    public void StartHangingOut()
    {
        _isHangingOut = true;
        SelectRandomTargetPosition();
        _hangOutTimer = hangOutWaitTime;
    }

    public void StopHangingOut()
    {
        _isHangingOut = false;
        _animator.SetBool("isWalking", false);
    }

    public void StartSleeping()
    {
        if (sleepText != null) sleepText.SetActive(true);
    }

    public void StopSleeping()
    {
        if (sleepText != null) sleepText.SetActive(false);
    }

    private void SelectRandomTargetPosition()
    {
        float randomX = Random.Range(hangOutAreaMin.x, hangOutAreaMax.x);
        float randomZ = Random.Range(hangOutAreaMin.z, hangOutAreaMax.z);
        _targetPosition = new Vector3(randomX, transform.position.y, randomZ);
        _isMovingToTarget = true;
    }

    public void MoveToPosition(Vector3 position)
    {
        _targetPosition = position;
        _isMovingToTarget = true;
    }

    private void MoveTowardsTarget()
    {
        if (Vector3.Distance(transform.position, _targetPosition) < 0.1f)
        {
            _animator.SetBool("isWalking", false);
            _isMovingToTarget = false;
            return;
        }

        _animator.SetBool("isWalking", true);

        Vector3 movementDirection = (_targetPosition - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, moveSpeed * Time.deltaTime);

        if (movementDirection.x < 0)
        {
            _leafShadow.Value = 1;
            _visual.localScale = new Vector3(-_originalScale.x, _originalScale.y, _originalScale.z);
        }
        else if (movementDirection.x > 0)
        {
            _leafShadow.Value = 0;
            _visual.localScale = new Vector3(_originalScale.x, _originalScale.y, _originalScale.z);
        }
    }

    //void Update()
    //{
    //    Vector3 movement = Vector3.zero;

    //    if (Input.GetKey(KeyCode.UpArrow))
    //    {
    //        movement.z += 1;
    //    }
    //    if (Input.GetKey(KeyCode.DownArrow))
    //    {
    //        movement.z -= 1;
    //    }
    //    if (Input.GetKey(KeyCode.LeftArrow))
    //    {
    //        movement.x -= 1;
    //    }
    //    if (Input.GetKey(KeyCode.RightArrow))
    //    {
    //        movement.x += 1;
    //    }

    //    if (movement == Vector3.zero && _previousMovement == Vector3.zero)
    //    {
    //        _animator.SetBool("isWalking", false);
    //    }
    //    else
    //    {
    //        _animator.SetBool("isWalking", true);
    //    }

    //    transform.position = transform.position + movement * Time.deltaTime * moveSpeed;

    //    if (movement.x < 0)
    //    {
    //        _leafShadow.Value = 1;
    //        //Debug.Log(_leafShadow.Value);
    //        _visual.transform.localScale = new Vector3(-_originalScale.x, _originalScale.y, _originalScale.z);
    //    }
    //    else if (movement.x > 0)
    //    {
    //        _leafShadow.Value = 0;
    //        //Debug.Log(_leafShadow.Value);
    //        _visual.transform.localScale = new Vector3(_originalScale.x, _originalScale.y, _originalScale.z);
    //    }

    //    _previousMovement = movement;
    //}
}
