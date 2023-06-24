using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.MoveGridDemo
{
    [RequireComponent(typeof(Button))]
    public class Button_Delete : MonoBehaviour
    {
        public static event System.Action OnDeletePressed;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnDeletePressed?.Invoke());
        }
    }
}
