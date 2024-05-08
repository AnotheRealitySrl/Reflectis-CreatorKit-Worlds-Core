using Reflectis.SDK.ObjectSpawner;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class HelpSpawnObjectPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private SpawnableData spawnableData;

        public SpawnableData SpawnableData { get => spawnableData; }
    }
}
