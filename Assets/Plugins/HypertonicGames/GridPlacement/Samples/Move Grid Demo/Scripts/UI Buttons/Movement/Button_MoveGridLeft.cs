using System;
using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.MoveGridDemo.Buttons.Movement
{
    public class Button_MoveGridLeft : MonoBehaviour
    {
        public static Action OnMoveLeftPresed;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnMoveLeftPresed?.Invoke());
        }
    }
}