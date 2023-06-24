using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.CustomPlacement
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