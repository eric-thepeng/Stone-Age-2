using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.CompleteDemo
{
    [RequireComponent(typeof(Button))]
    public class Button_CloseNearbyObjectOptions : MonoBehaviour
    {
        public static event System.Action OnCloseMenuPressed;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnCloseMenuPressed?.Invoke());
        }
    }
}