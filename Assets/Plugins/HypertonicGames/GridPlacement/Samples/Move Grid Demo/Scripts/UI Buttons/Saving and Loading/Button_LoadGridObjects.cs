using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.MoveGridDemo
{
    [RequireComponent(typeof(Button))]
    public class Button_LoadGridObjects : MonoBehaviour
    {
        public static event System.Action OnLoadGridObjectsEvent;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnLoadGridObjectsEvent?.Invoke());
        }
    }
}