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
    private bool _collisionStay = false;

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
        if (other.gameObject.GetComponent<Obstacle>() != null || other is TerrainCollider)
        {
            HandleEnteredWallArea();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Obstacle>() != null || other is TerrainCollider)
        {
            if (!_collisionStay)
            HandleExitedWallArea();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Obstacle>() != null || other is TerrainCollider)
        {
            _collisionStay = true;
        } else
        {
            _collisionStay = false;
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
