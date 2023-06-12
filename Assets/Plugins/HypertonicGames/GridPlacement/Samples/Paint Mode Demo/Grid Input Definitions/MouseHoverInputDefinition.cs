using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Hypertonic.GridPlacement.GridInput
{
    [CreateAssetMenu(fileName = "Mouse Hover Input Definition", menuName = "Grid/Mouse Hover Input Definition")]
    public class MouseHoverInputDefinition : GridInputDefinition
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
            return true;
        }
    }
}
