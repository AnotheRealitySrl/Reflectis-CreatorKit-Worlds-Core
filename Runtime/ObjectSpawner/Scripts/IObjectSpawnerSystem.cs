using Reflectis.SDK.Core.SystemFramework;

using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ObjectSpawner
{
    public interface IObjectSpawnerSystem : ISystem
    {
        public enum EPrefabIdentifier
        {
            Downloaded3DModel,
            VideoPlayer,
            PresentationPlayer,
            ImagePlayer,
            BigScreen,
            DrawableBoard,
            Drawing,
            VideoChat,
            TextBox,
            GeneralContainer
        }

        GameObject InstantiateLocalObject(GameObject gameObject, object[] data = null, SpawnPosition spawnableData = null);
        GameObject InstantiateLocalObject(GameObject gameObject, Vector3 position, Quaternion rotation, object[] data = null);

        Task<GameObject> InstantiateObject(string objectKey, bool onNetwork = false, object[] data = null, SpawnPosition spawnableData = null, bool lookAtPlayer = true);
        Task<GameObject> InstantiateObject(string objectKey, Vector3 position, Quaternion rotation, bool onNetwork = true, object[] data = null);
        /// <summary>
        /// Instantiate an object in front of the player checking the first empty space
        /// </summary>
        /// <param name="prefabId"></param>
        /// <param name="onNetwork"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<GameObject> InstantiateObject(EPrefabIdentifier prefabId, bool onNetwork = true, object[] data = null, SpawnPosition spawnableData = null);
        Task<GameObject> InstantiateObject(EPrefabIdentifier prefabId, Vector3 position, Quaternion rotation, bool onNetwork = true, object[] data = null);
    }
}
