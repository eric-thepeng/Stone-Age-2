using Hypertonic.GridPlacement.Example.MoveGridDemo.Buttons.Movement;
using UnityEngine;


namespace Hypertonic.GridPlacement.Example.MoveGridDemo
{
    public class GridMovementManager : MonoBehaviour
    {
        [SerializeField]
        private float _moveAmount = 1f;

        [SerializeField]
        private float _rotationAmount = 45f;

        private void OnEnable()
        {
            Button_MoveGridLeft.OnMoveLeftPresed += HandleMoveLeftPressed;
            Button_MoveGridRight.OnMoveRightPressed += HandleMoveRightPressed;

            Button_RotateGridLeft.OnRotateLeftPressed += HandleRotateLeftPressed;
            Button_RotateGridRight.OnRotateRightPressed += HandleRotateRightPressed;
        }

        private void OnDisable()
        {
            Button_MoveGridLeft.OnMoveLeftPresed -= HandleMoveLeftPressed;
            Button_MoveGridRight.OnMoveRightPressed -= HandleMoveRightPressed;

            Button_RotateLeft.OnRotateLeftPressed -= HandleRotateLeftPressed;
            Button_RotateRight.OnRotateRightPressed -= HandleRotateRightPressed;
        }

        private void HandleMoveLeftPressed()
        {
            Vector3 currentGridPosition = GridManagerAccessor.GridManager.RuntimeGridPosition;
            Vector3 moveIncrement = new Vector3(-_moveAmount, 0, 0);
            GridManagerAccessor.GridManager.MoveGridTo(currentGridPosition + moveIncrement);
        }

        private void HandleMoveRightPressed()
        {
            Vector3 currentGridPosition = GridManagerAccessor.GridManager.RuntimeGridPosition;
            Vector3 moveIncrement = new Vector3(_moveAmount, 0, 0);
            GridManagerAccessor.GridManager.MoveGridTo(currentGridPosition + moveIncrement);
        }

        private void HandleRotateLeftPressed()
        {
            float currentGridRotation = GridManagerAccessor.GridManager.RuntimeGridRotation;
            GridManagerAccessor.GridManager.RotateGridTo(currentGridRotation - _rotationAmount);
        }

        private void HandleRotateRightPressed()
        {
            float currentGridRotation = GridManagerAccessor.GridManager.RuntimeGridRotation;
            GridManagerAccessor.GridManager.RotateGridTo(currentGridRotation + _rotationAmount);
        }
    }
}