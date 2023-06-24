using System;
using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.MoveGridDemo.Buttons.Movement
{
    public class Button_RotateGridLeft : MonoBehaviour
    {
        public static Action OnRotateLeftPressed;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnRotateLeftPressed?.Invoke());
        }
    }
}