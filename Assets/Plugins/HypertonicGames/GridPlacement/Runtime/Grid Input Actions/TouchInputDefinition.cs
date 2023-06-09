using UnityEngine;

namespace Hypertonic.GridPlacement.GridInput
{
    [CreateAssetMenu(fileName = "Touch Input Definition", menuName = "Grid/Touch Input Definition")]
    public class TouchInputDefinition : GridInputDefinition
    {
        private void Awake()
        {
#if ENABLE_INPUT_SYSTEM
            UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
#endif
        }

        public override Vector3? InputPosition()
        {
#if ENABLE_INPUT_SYSTEM

            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count <= 0)
            {
                return null;
            }

            return UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition;
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
 if (Input.touchCount <= 0)
            {
                return null;
            }

            Touch touch = Input.GetTouch(0);

            return touch.position;
#endif
        }

        public override bool ShouldInteract()
        {
            return true;
        }
    }
}