using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Hypertonic.GridPlacement.GridInput
{
    [CreateAssetMenu(fileName = "Mouse Input Definition", menuName = "Grid/Mouse Input Definition")]
    public class MouseInputDefinition : GridInputDefinition
    {
        public override Vector3? InputPosition()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current.position.ReadValue();
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.mousePosition;
#endif
        }

        public override bool ShouldInteract()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current.leftButton.isPressed;
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKey(KeyCode.Mouse0);
#endif
        }
    }
}