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


    public int collisionCount = 0;
    private bool wasOnTerrainLastFrame = false; // 用于跟踪上一帧的状态

    private void Update()
    {
        CheckTerrainCollision();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Obstacle>() != null || other is TerrainCollider)
        {
            collisionCount++;
            HandleEnteredWallArea();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Obstacle>() != null || other is TerrainCollider)
        {
            collisionCount--;
            if (collisionCount == 0)
            {
                HandleExitedWallArea();
            }
        }
    }


    public void HandleEnteredWallArea()
    {
        _customValidator.SetValidation(false);
    }

    public void HandleExitedWallArea()
    {
        _customValidator.SetValidation(true);
    }

    private void CheckTerrainCollision()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            if (hit.collider is TerrainCollider && !wasOnTerrainLastFrame)
            {
                HandleExitedWallArea();
                wasOnTerrainLastFrame = true;
                //Debug.Log("Exited the terrin collider");
            }
        }
        else if (wasOnTerrainLastFrame)
        {
            HandleEnteredWallArea();
            wasOnTerrainLastFrame = false;
            //Debug.Log("Entered the terrin collider");
        }
    }

}
