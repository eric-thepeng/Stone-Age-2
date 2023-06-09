using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.CustomPlacement
{
    [RequireComponent(typeof(Button))]
    public class Button_OpenPlacementSettings : MonoBehaviour
    {
        public static event System.Action OnOpenSettingsPressed;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnOpenSettingsPressed?.Invoke());
        }
    }
}