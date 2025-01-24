using Reflectis.SDK.Core;

using System.Collections.Generic;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.SceneHierarchy
{
    public interface ISceneObjectsSystem : ISystem
    {
        public void EnableOtherAvatars(bool enable, List<GameObject> except = null);

        public void EnableSpawnedObjects(bool enable, List<GameObject> except = null);

        public void EnableSpawnedNetworkObjects(bool enable, List<GameObject> except = null);

        public void EnableSpawnedLocalObjects(bool enable, List<GameObject> except = null);

        public void EnableEnvironment(bool enable, List<GameObject> except = null);

        public void AddNetworkSpawnedObject(GameObject gameObject);

        public void AddAvatar(GameObject avatar);

        public void AddLocalSpawnedObject(GameObject localObject);

        public void AddEnvironmentObject(GameObject environmentObject);
    }
}