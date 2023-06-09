using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.CustomPlacement
{
    [RequireComponent(typeof(Button))]
    public class Button_ConfirmPlacementSettings : MonoBehaviour
    {
        public static event System.Action OnConfirmPlacementPressed;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnConfirmPlacementPressed?.Invoke());
        }
    }
}