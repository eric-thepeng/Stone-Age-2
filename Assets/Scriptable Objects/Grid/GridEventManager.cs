using UnityEngine;

    /// <summary>
    /// This class is required for the sample scenes. As the samples may be used in a project with either
    /// the new input system or the old input system the appropriate event system prefab should be added 
    /// to the scene.
    /// </summary>
public class GridEventManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _oldEventSystemPrefab;

    [SerializeField]
    private GameObject _newEventSystemPrefab;


    public void Awake()
    {
#if ENABLE_INPUT_SYSTEM
        SpawnNewEventSystem();
#elif ENABLE_LEGACY_INPUT_MANAGER
        SpawnOldEventSystem();
#endif
    }

    private void SpawnOldEventSystem()
    {
        Instantiate(_oldEventSystemPrefab);
    }

    private void SpawnNewEventSystem()
    {
        Instantiate(_newEventSystemPrefab);
    }
}
