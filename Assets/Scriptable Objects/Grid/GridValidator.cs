using Hypertonic.GridPlacement.GridObjectComponents;
using UnityEngine;

    /// <summary>
    /// This is an example of a validator you may add to the gameobject you're placing.
    /// This example component is being used to detect if the object is colliding with the wall gameobjects 
    /// in the scene. If it is, it will mark the component as having an invalid placement.
    /// </summary>
    [RequireComponent(typeof(CustomValidator))]
    public class GridValidator : MonoBehaviour
    {
        private CustomValidator _customValidator;

        private void Awake()
        {
            _customValidator = GetComponent<CustomValidator>();
        }

        /// <summary>
        /// We will check what object we hit. If it a wall object we'll set the 
        /// custom validation to be invalid.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            // To avoid adding a "wall" tag to the package, we will check the object
            // we've collided with based on if it has the demo wall component. You could also check 
            // which object it is by checking the name or however else you wish.
            if(other.gameObject.GetComponent<Obstacle>() != null)
            {
                HandleEnteredWallArea();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<Obstacle>() != null)
            {
                HandleExitedWallArea();
            }
        }

        private void HandleEnteredWallArea()
        {
            _customValidator.SetValidation(false);
        }

        private void HandleExitedWallArea()
        {
            _customValidator.SetValidation(true);
        }
    }
