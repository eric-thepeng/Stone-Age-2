using UnityEngine;

namespace Hypertonic.GridPlacement.GridObjectComponents
{
    public class CustomValidator : MonoBehaviour
    {
        public delegate void CustomValidatorEvent(bool valid);
        public event CustomValidatorEvent OnValidationChanged;

        /// <summary>
        /// Sets the default custom validation state.
        /// </summary>
        [SerializeField]
        private bool _isValid = true;

        public bool IsValid { get => _isValid; }

        /// <summary>
        /// Sets the state of the custom validator.
        /// </summary>
        /// <param name="valid">If the placement of the object is valid </param>
        public void SetValidation(bool valid)
        {
            _isValid = valid;

            OnValidationChanged?.Invoke(_isValid);
        }    
    }
}