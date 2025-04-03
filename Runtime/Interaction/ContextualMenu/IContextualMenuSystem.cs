using Reflectis.SDK.Core.SystemFramework;

using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.Interaction
{
    public interface IContextualMenuSystem : ISystem
    {
        IContextualMenuController ContextualMenuInstance { get; }
        IContextualMenuManageable SelectedContextualMenuInteractable { get; }
        IContextualMenuManageable AddContextualMenu(GameObject contextualMenuGameObject);
        void CreateMenu(Transform parent = null);
        Task DestroyContextualMenu();

        Task ShowContextualMenu(IContextualMenuManageable manageable);
        Task HideContextualMenu();

        void ResetContextualMenuCache();
        void RefreshContextualMenu();
    }
}
