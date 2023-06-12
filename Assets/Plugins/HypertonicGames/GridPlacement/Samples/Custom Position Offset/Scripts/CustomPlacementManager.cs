using Hypertonic.GridPlacement.GridObjectComponents;
using System.Collections;
using UnityEngine;

namespace Hypertonic.GridPlacement.Example.PositionOffsetDemo
{
    /// <summary>
    /// This class is designed to show how to use the SetCustomPositionOverride function on the CustomPosition Override component to
    /// achieve a simple animation by adjusting the position over x time.
    /// </summary>
    public class CustomPlacementManager : MonoBehaviour
    {
        public delegate void CustomPlacementManagerEvent();
        public static event CustomPlacementManagerEvent OnObjectPlacedOnGrid;

        [SerializeField]
        private AnimationCurve _heightCurve;

        [SerializeField]
        private AnimationCurve _xCurve;

        [SerializeField]
        private float _animationDuration = 1f;

        private void OnEnable()
        {
            GridControlManager.OnConfirmButtonPressed += HandleConfirmButtonPressed;
        }

        private void OnDisable()
        {
            GridControlManager.OnConfirmButtonPressed -= HandleConfirmButtonPressed;
        }

        private void HandleConfirmButtonPressed()
        {
            // Check if the placement is valid
            PlacementValidResponse placementValidResponse = GridManagerAccessor.GridManager.IsObjectPlacementValid();

            if(!placementValidResponse.Valid)
            {
                // If the placement isn't valid don't do anything
                return;
            }

            // Get the object currently being placed.
            GameObject objectBeingPlaced = GridManagerAccessor.GridManager.ObjectToPlace;

            if (objectBeingPlaced == null)
            {
                Debug.LogError("There is no object being placed.");
                return;
            }

            // Get the ExampleGridObject script from the object being placed. In this demo we know this will have the PlacementAnimationType property.
            ExampleGridObject exampleGridObject = objectBeingPlaced.GetComponent<ExampleGridObject>();

            if(exampleGridObject == null)
            {
                Debug.LogWarning("The object being placed does not have an ExampleGridObject script attached.");
                return;
            }

            if(exampleGridObject.PlacementAnimationType == PlacementAnimationType.ANIMATION_CURVE)
            {
                DoPlacementAnimation(objectBeingPlaced);
            }
            else
            {
                GridManagerAccessor.GridManager.ConfirmPlacement();
                OnObjectPlacedOnGrid?.Invoke();
            }
        }

        private void DoPlacementAnimation(GameObject objectBeingPlaced)
        {
            // Get the custom CustomPositionOffset component from the game object being placed
            CustomPositionOffset customPositionOffset = objectBeingPlaced.GetComponent<CustomPositionOffset>();

            if(customPositionOffset == null)
            {
                Debug.LogError("There is no CustomPositionOffset component attached to the object to place. Cannot perform placement animation");
                return;
            }

            StartCoroutine(PlacementAnimationCoroutine(customPositionOffset));
        }

        private IEnumerator PlacementAnimationCoroutine(CustomPositionOffset customPositionOffset)
        {
            // Animation curves have a range of 0 - 1. So over the course of _animationDuration we will update the position
            // to be the postion values on the animation curves.

            float progress = 0;
            float target = 1;
            float elapsedTime = 0;

            while(progress < target)
            {
                elapsedTime += Time.deltaTime;
                progress = elapsedTime / _animationDuration;

                float yPos = _heightCurve.Evaluate(progress);
                float xPos = _xCurve.Evaluate(progress);

                customPositionOffset.SetCustomPositionOverride(new Vector3(xPos, yPos, 0));

                yield return null;
            }

            GridManagerAccessor.GridManager.ConfirmPlacement();

            OnObjectPlacedOnGrid?.Invoke();
        }
    }

}
