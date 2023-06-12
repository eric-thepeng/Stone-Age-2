using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.BasicDemo
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
