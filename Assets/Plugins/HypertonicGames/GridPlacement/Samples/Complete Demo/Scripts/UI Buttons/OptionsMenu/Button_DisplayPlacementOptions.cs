using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.CompleteDemo
{
    [RequireComponent(typeof(Button))]
    public class Button_DisplayPlacementOptions : MonoBehaviour
    {
        public static event System.Action OnDisplayPlacementOptionsPressed;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnDisplayPlacementOptionsPressed?.Invoke());
        }
    }
}
