using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.MoveGridDemo
{
    [RequireComponent(typeof(Button))]
    public class Button_SaveGridObjects : MonoBehaviour
    {
        public static event System.Action OnSaveGridObjectsEvent;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnSaveGridObjectsEvent?.Invoke());
        }
    }
}