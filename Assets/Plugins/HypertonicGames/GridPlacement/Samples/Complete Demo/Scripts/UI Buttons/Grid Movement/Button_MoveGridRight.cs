using System;
using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.CompleteDemo.Buttons.Movement
{
    public class Button_MoveGridRight : MonoBehaviour
    {
        public static Action OnMoveRightPressed;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnMoveRightPressed?.Invoke());
        }
    }
}