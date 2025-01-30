using Reflectis.SDK.Core.SystemFramework;

using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;

namespace Reflectis.CreatorKit.Worlds.Core.Interaction
{
    public interface IVisualScriptingInteractionSystem : ISystem
    {
        InteractableBehaviourBase SetupInteractableBehaviour(GameObject obj);
        Task UnselectCurrentInteractable(IVisualScriptingInteractable interactableToDisable);
        public UnityEvent<IVisualScriptingInteractable> OnSelectedInteractableChange { get; set; }
    }
}
