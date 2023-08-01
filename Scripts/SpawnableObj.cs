using Reflectis.SDK.ObjectSpawner;

using System.Threading.Tasks;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class SpawnableObj : MonoBehaviour, IRuntimeComponent
    {
        private SpawnableData data;

        public SpawnableData Data { get => data; }

        public Task Init(SceneComponentPlaceholderBase placeholder)
        {
            SpawnableObjPlaceholder spawnableObjPlaceholder = placeholder as SpawnableObjPlaceholder;
            data = spawnableObjPlaceholder.Data;

            return Task.CompletedTask;
        }
    }
}
