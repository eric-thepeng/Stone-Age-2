using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.CustomPlacement
{
    [RequireComponent(typeof(Button))]
    public class Button_GridObjectSelectionOption : MonoBehaviour
    {
        public static event System.Action<GameObject> OnOptionSelected;

        [SerializeField] private GameObject _gridObjectToSpawnPrefab;

        private void Start()
        {
            Button button = GetComponent<Button>();

            if (button != null)
            {
                button.onClick.AddListener(HandleButtonClicked);
            }
        }

        private void HandleButtonClicked()
        {
            if (_gridObjectToSpawnPrefab == null)
            {
                Debug.LogError("Error. No prefab assigned to spawn on this selection option");
            }

            OnOptionSelected?.Invoke(_gridObjectToSpawnPrefab);
        }
    }
}