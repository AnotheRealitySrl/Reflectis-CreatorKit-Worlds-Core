using UnityEngine;
using UnityEngine.InputSystem;

namespace Reflectis.CreatorKit.Core
{
    [RequireComponent(typeof(SpawnObjectData))]
    public class ObjectSpawnerPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private InputActionReference vrInput;
        [SerializeField]
        private InputActionReference desktopInput;

        public InputActionReference VrInput { get => vrInput; }
        public InputActionReference DesktopInput { get => desktopInput; }
    }
}
