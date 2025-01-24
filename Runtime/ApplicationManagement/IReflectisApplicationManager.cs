using Reflectis.CreatorKit.Worlds.Core.ClientModels;
using Reflectis.SDK.Core.ApplicationManagement;

using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core
{
    public interface IReflectisApplicationManager : IApplicationManager
    {
        static new IReflectisApplicationManager Instance { get; protected set; }
        EApplicationState State { get; }
        Task LoadDefaultEvent();
        Task InitializeObject(GameObject gameObject, bool initializeChildren = false);
        Task LoadEvent(CMEvent ev, CMShard shard = null, bool updateHistory = true, bool recoverFromDisconnection = false);
    }
}
