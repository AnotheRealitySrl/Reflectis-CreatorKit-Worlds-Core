using Reflectis.SDK.ObjectSpawner;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Reflectis.SDK.CreatorKit
{
    public class SpawnableObjPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private SpawnableData spawnableData;

        public SpawnableData Data { get => spawnableData; }
    }
}
