using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.CompleteDemo
{
    [RequireComponent(typeof(Button))]
    public class Button_CloseOptionsMenu : MonoBehaviour
    {
        public static event System.Action OnCloseOptionsMenuPressed;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnCloseOptionsMenuPressed?.Invoke());
        }
    }
}
