using Reflectis.SDK.ObjectSpawner;
using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class SpawnableObj : MonoBehaviour, IRuntimeComponent
    {
        private SpawnableData data;

        public SpawnableData Data { get => data; }

        public void Init(SceneComponentPlaceholderBase placeholder)
        {
            SpawnableObjPlaceholder spawnableObjPlaceholder = placeholder as SpawnableObjPlaceholder;
            data = spawnableObjPlaceholder.Data;
        }
    }
}
