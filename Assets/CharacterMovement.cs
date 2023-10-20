using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 1f;

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

    void Update()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            movement.z += 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movement.z -= 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement.x -= 1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movement.x += 1;
        }

        if (movement == Vector3.zero && _previousMovement == Vector3.zero)
        {
            _animator.SetBool("isWalking", false);
        }
        else
        {
            _animator.SetBool("isWalking", true);
        }

        transform.position = transform.position + movement * Time.deltaTime * moveSpeed;

        if (movement.x < 0)
        {
            _leafShadow.Value = 1;
            //Debug.Log(_leafShadow.Value);
            _visual.transform.localScale = new Vector3(-_originalScale.x, _originalScale.y, _originalScale.z);
        }
        else if (movement.x > 0)
        {
            _leafShadow.Value = 0;
            //Debug.Log(_leafShadow.Value);
            _visual.transform.localScale = new Vector3(_originalScale.x, _originalScale.y, _originalScale.z);
        }

        _previousMovement = movement;
    }
}
