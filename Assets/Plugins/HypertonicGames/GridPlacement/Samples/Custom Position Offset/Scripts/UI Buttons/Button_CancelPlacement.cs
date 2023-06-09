using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.PositionOffsetDemo
{
    [RequireComponent(typeof(Button))]
    public class Button_CancelPlacement : MonoBehaviour
    {
        public static event System.Action OnCancelPlacementPressed;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnCancelPlacementPressed?.Invoke());
        }
    }
}
