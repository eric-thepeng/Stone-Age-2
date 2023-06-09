using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.CompleteDemo
{
    [RequireComponent(typeof(Button))]
    public class Button_DisplayChangeNearbyObjectsOptions : MonoBehaviour
    {
        public static event System.Action OnDisplayChangeNearbyObjectsOptions;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnDisplayChangeNearbyObjectsOptions?.Invoke());
        }
    }
}
