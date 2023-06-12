using System;
using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.CompleteDemo.Buttons.Movement
{
    public class Button_RotateGridRight : MonoBehaviour
    {
        public static Action OnRotateRightPressed;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnRotateRightPressed?.Invoke());
        }
    }
}