using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.CompleteDemo.Buttons.PlacementSettings
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
