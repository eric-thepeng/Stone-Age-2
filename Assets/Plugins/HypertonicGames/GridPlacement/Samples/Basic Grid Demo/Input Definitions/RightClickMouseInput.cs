using Hypertonic.GridPlacement.GridInput;
using UnityEngine;

namespace Hypertonic.GridPlacement.Example.BasicDemo.InputActions
{

    [CreateAssetMenu(fileName = "Right Click Input Definition", menuName = "Grid/Right Click Input Definition")]
    public class RightClickMouseInput : GridInputDefinition
    {
        public override Vector3? InputPosition()
        {
#if ENABLE_INPUT_SYSTEM
            return UnityEngine.InputSystem.Mouse.current.position.ReadValue();
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.mousePosition;
#endif
        }

        public override bool ShouldInteract()
        {
#if ENABLE_INPUT_SYSTEM
            return UnityEngine.InputSystem.Mouse.current.rightButton.isPressed;
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKey(KeyCode.Mouse1);
#endif
        }
    }
}