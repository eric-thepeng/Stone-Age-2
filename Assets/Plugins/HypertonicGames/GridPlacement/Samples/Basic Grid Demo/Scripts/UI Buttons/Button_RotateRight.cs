using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.BasicDemo
{
    [RequireComponent(typeof(Button))]
    public class Button_RotateRight : MonoBehaviour
    {
        public static event System.Action OnRotateRightPressed;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnRotateRightPressed?.Invoke());
        }
    }
}