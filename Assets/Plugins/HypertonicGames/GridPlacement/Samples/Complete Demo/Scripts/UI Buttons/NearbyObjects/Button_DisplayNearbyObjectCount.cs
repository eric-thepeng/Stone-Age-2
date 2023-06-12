using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.CompleteDemo.Buttons.NearbyObjectCount
{
    [RequireComponent(typeof(Button))]
    public class Button_DisplayNearbyObjectCount : MonoBehaviour
    {
        public static event System.Action OnDisplayNearybyObjectPressed;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnDisplayNearybyObjectPressed?.Invoke());
        }
    }
}