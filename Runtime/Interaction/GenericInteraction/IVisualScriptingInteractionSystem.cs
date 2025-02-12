using Reflectis.SDK.Core.SystemFramework;

using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;

namespace Reflectis.CreatorKit.Worlds.Core.Interaction
{
    public interface IVisualScriptingInteractionSystem : ISystem
    {
        InteractableBehaviourBase SetupInteractableBehaviour(GameObject obj);
        Task SelectInteractable(IVisualScriptingInteractable interactableToDisable);
        Task UnselectCurrentInteractable(IVisualScriptingInteractable interactableToDisable);
        void UnselectCurrentInteractable();
        public UnityEvent<IVisualScriptingInteractable> OnSelectedInteractableChange { get; set; }
    }
}
