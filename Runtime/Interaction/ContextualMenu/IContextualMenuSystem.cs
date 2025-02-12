using Reflectis.SDK.Core.SystemFramework;

using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.Interaction
{
    public interface IContextualMenuSystem : ISystem
    {
        IContextualMenuController ContextualMenuInstance { get; }
        IContextualMenuManageable SelectedInteractable { get; }

        void CreateMenu(Transform parent = null);
        Task DestroyContextualMenu();

        Task ShowContextualMenu(IContextualMenuManageable manageable);
        Task HideContextualMenu();
        Task HideContextualMenu(IContextualMenuController contextualMenu);

        void ResetContextualMenuCache();
    }
}
