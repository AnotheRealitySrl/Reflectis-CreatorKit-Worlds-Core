using Reflectis.SDK.Core.SystemFramework;

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Reflectis.CreatorKit.Worlds.Core.Interaction
{
    public interface IVisualScriptingInteractionSystem : ISystem
    {
        IVisualScriptingInteractable SelectedInteractable { get; }
        Task SelectInteractable(IVisualScriptingInteractable interactableToDisable);
        Task UnselectCurrentInteractable(IVisualScriptingInteractable interactableToDisable);
        void UnselectCurrentInteractable();
        public UnityEvent<IVisualScriptingInteractable> OnSelectedInteractableChange { get; set; }

        public IVisualScriptingInteractable AddVisualScriptingInteractable(GameObject gameObject);
    }
}
