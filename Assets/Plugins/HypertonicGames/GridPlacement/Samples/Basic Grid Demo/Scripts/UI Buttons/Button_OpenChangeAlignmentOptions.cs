using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.BasicDemo
{
    [RequireComponent(typeof(Button))]
    public class Button_OpenChangeAlignmentOptions : MonoBehaviour
    {
        public static event System.Action OnOpenChangeAlignmentOptionEvent;

        public void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnOpenChangeAlignmentOptionEvent?.Invoke());
        }
    }
}