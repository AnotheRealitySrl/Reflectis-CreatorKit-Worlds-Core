using Reflectis.SDK.Core.SystemFramework;

using System.Threading.Tasks;

using UnityEngine;

using static Reflectis.CreatorKit.Worlds.Core.Interaction.IContextualMenuManageable;

namespace Reflectis.CreatorKit.Worlds.Core.Interaction
{
    public interface IContextualMenuSystem : ISystem
    {
        IContextualMenuController ContextualMenuInstance { get; }
        IContextualMenuManageable SelectedContextualMenuInteractable { get; }
        IContextualMenuManageable AddContextualMenu(GameObject contextualMenuGameObject, EContextualMenuOption contextualMenuOptions);

        void CreateMenu(Transform parent = null);
        Task DestroyContextualMenu();

        Task ShowContextualMenu(IContextualMenuManageable manageable);
        Task HideContextualMenu();

        void ResetContextualMenuCache();
        void RefreshContextualMenu();
    }
}
