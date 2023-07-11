using Reflectis.SDK.ObjectSpawner;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Virtuademy.Placeholders
{
    public class SpawnableObjPlaceholder : SceneComponentPlaceholderBase
    {
        private SpawnableData spawnableData;

        public SpawnableData Data { get => spawnableData; }
    }
}
