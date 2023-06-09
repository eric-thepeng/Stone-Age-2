using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.MoveGridDemo
{
    [RequireComponent(typeof(Button))]
    public class Button_CloseObjectSelection : MonoBehaviour
    {
        public static event System.Action OnCloseButtonPressed;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnCloseButtonPressed?.Invoke());
        }
    }
}