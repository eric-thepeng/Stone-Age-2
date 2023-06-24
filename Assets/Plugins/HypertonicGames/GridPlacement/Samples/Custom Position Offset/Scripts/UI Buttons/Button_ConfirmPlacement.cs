using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.PositionOffsetDemo
{
    [RequireComponent(typeof(Button))]
    public class Button_ConfirmPlacement : MonoBehaviour
    {
        public static event System.Action OnConfirmPlacementPressed;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnConfirmPlacementPressed?.Invoke());
        }
    }
}