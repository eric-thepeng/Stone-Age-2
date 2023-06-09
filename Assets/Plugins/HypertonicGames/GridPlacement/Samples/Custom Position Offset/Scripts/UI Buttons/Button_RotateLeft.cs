using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.PositionOffsetDemo
{
    [RequireComponent(typeof(Button))]
    public class Button_RotateLeft : MonoBehaviour
    {
        public static event System.Action OnRotateLeftPressed;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnRotateLeftPressed?.Invoke());
        }
    }
}