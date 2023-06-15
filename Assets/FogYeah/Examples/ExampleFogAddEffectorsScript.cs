using System.Collections.Generic;
using UnityEngine;

namespace FogSystems
{
    public class ExampleFogAddEffectorsScript : MonoBehaviour
    {
        /// <summary>
        /// This is the transform for the 'Player' object in the tutorial.
        /// </summary>
        [SerializeField]
        private List<Transform> PlayersToAddOnStart;

        /// <summary>
        /// This is the sight range of the 'Player' object in the tutorial.
        /// </summary>
        [SerializeField]
        private int SightRangeOfMovingEffectors = 500;

        /// <summary>
        /// Static effectors are any effectors that you want to add that do not change position. for example, Watch Towers, Buildings etc.
        /// </summary>
        [SerializeField]
        private List<Transform> StaticEffectorsToAddOnStart;

        /// <summary>
        /// All effectors can have individual sight ranges.
        /// </summary>
        [SerializeField]
        private int SightRangeOfStaticEffectors = 500;

        // Speed of the 'Player'
        [SerializeField]
        private int playerSpeed = 500;

        private void Start()
        {
            // Adding the example effectors in the start method.
            foreach (Transform t in PlayersToAddOnStart)
                FogSystem.Instance.AddEffector(t, SightRangeOfMovingEffectors);

            foreach (Transform t in StaticEffectorsToAddOnStart)
                FogSystem.Instance.AddEffector(t, SightRangeOfStaticEffectors);
        }

        private void RemoveRandomEffector()
        {
            // This method will demonstrate how to remove an effector from the list.
            if (FogSystem.Instance.Effectors.Count > 0)
            {
                // For the purposes of the tutorial, we will just select a random effector from the effectors that already exist, and then remove that using GetTransform().
                // In reality, you should use your own storage for your 'units'.
                Transform transformToRemove = FogSystem.Instance.Effectors[Random.Range(0, FogSystem.Instance.Effectors.Count)].GetTransform();

                // You can call RemoveEffector to remove the ability to reveal fog from any transform that has been added.
                // Will log an error if the transform was not found.
                FogSystem.Instance.RemoveEffector(transformToRemove);
            }
            else
                Debug.Log($"[{nameof(ExampleFogAddEffectorsScript)}] No effectors to remove. Use Q to create a new one.");
        }

        private void AddNewEffector()
        {
            // You can add any transform in your scene to the Effectors list in the system.
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            go.transform.localScale = new Vector3(50, 50, 50);

            // Just pass the transform to the AddEffector with the desired range you would like that unit to have.
            // Note: If the gameobject has 'Static' checked, this will create a Static effector. Read more about Static Effectors in the Readme.txt
            FogSystem.Instance.AddEffector(go.transform, 500);

            Debug.Log($"[{nameof(ExampleFogAddEffectorsScript)}] Created a sphere with 500 sight range at 0,0,0");
        }

        private void Update()
        {
            // Hitting E will trigger a 'Remove'
            if (Input.GetKeyDown(KeyCode.E))
                RemoveRandomEffector();

            // Hitting Q will trigger an 'Add'
            if (Input.GetKeyDown(KeyCode.Q))
                AddNewEffector();

            // Example movement for the 'Player' to move the transform around the map.
            Vector3 Movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (Movement.magnitude > 0)
                foreach (Transform player in PlayersToAddOnStart)
                {
                    player.position += Movement * playerSpeed * Time.deltaTime;
                }
        }
    }
}