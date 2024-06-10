using Reflectis.SDK.ObjectSpawner;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Reflectis.SDK.CreatorKit
{
    public class ObjectSpawnerPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private SpawnableData spawnableData;
        [SerializeField]
        private InputActionReference vrInput;
        [SerializeField]
        private InputActionReference desktopInput;

        public InputActionReference VrInput { get => vrInput; }
        public InputActionReference DesktopInput { get => desktopInput; }
        public SpawnableData Data { get => spawnableData; }
    }
}
