using UnityEngine;
using UnityEngine.InputSystem;

namespace Reflectis.SDK.CreatorKit
{
    public class AddOpenHelpOnActionPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private InputActionReference vrInput;
        [SerializeField]
        private InputActionReference desktopInput;

        public InputActionReference VrInput { get => vrInput; set => vrInput = value; }
        public InputActionReference DesktopInput { get => desktopInput; set => desktopInput = value; }
    }
}
