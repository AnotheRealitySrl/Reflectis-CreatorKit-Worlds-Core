using Reflectis.SDK.Core.SystemFramework;

using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.LockObject
{
    public interface ILockObjectSystem : ISystem
    {
        Task SetupLockObject(GameObject lockObject, bool isNetworked);
    }
}