using Reflectis.SDK.Core.SystemFramework;

using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.ModelExploder
{
    public interface IModelExploderSystem : ISystem
    {
        Task AssignModelExploder(GameObject obj, bool networkedContext = true);
    }
}
