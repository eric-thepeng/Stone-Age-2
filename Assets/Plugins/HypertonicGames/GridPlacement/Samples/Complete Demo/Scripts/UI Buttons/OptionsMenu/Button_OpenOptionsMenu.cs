using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.CompleteDemo.Buttons.NearbyObjectCount
{
    [RequireComponent(typeof(Button))]
    public class Button_OpenOptionsMenu : MonoBehaviour
    {
        public static event System.Action OnOpenOptionsMenuPressed;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnOpenOptionsMenuPressed?.Invoke());
        }
    }
}