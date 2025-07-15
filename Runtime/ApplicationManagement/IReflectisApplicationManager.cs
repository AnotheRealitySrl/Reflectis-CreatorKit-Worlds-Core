using Reflectis.CreatorKit.Worlds.Core.ClientModels;
using Reflectis.SDK.Core.ApplicationManagement;

using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ApplicationManagement
{
    public interface IReflectisApplicationManager : IApplicationManager
    {
        static new IReflectisApplicationManager Instance { get; protected set; }

        EApplicationState State { get; }
        Task InitializeObject(GameObject gameObject, bool initializeChildren = false);
        Task<bool> JoinSession(int eventId, int? shard = null, bool updateHistory = true, bool recoverFromDisconnection = false);

        Task<bool> JoinExperience(CMExperience experience, bool multiplayer, int? shard = null, bool updateHistory = true, bool recoverFromDisconnection = false);

        void EnableOtherAvatars(bool enable, List<GameObject> except = null);
        void EnableSpawnedObjects(bool enable, List<GameObject> except = null);
        void EnableSpawnedNetworkObjects(bool enable, List<GameObject> except = null);
        void EnableSpawnedLocalObjects(bool enable, List<GameObject> except = null);
        void EnableEnvironment(bool enable, List<GameObject> except = null);
        void AddNetworkSpawnedObject(GameObject gameObject);
        void AddAvatar(GameObject avatar);
        void AddLocalSpawnedObject(GameObject localObject);
        void AddEnvironmentObject(GameObject environmentObject);
    }
}
